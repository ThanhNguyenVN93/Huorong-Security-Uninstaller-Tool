using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frm_HIS_uninstaller_tool
{
    public partial class Form3 : Form
    {
        // Tìm trong data value — chỉ cần 2 keyword chính
        private static readonly string[] SearchTerms = { "Huorong", "火绒" };

        // Phạm vi quét — bỏ HKU vì HKCU đã cover user hiện tại, tránh trùng lặp
        private static readonly string[] Scopes = {
            "HKLM\\SOFTWARE",
            "HKLM\\SYSTEM",
            "HKCU\\SOFTWARE",
        };

        public Form3() => InitializeComponent();

        private void Form3_Load(object sender, EventArgs e) { }

        // ── Quét ─────────────────────────────────────────────────────────────

        private async void btnDeepScan_Click(object sender, EventArgs e)
        {
            lstDeepScan.Items.Clear();
            SetScanState(scanning: true);
            Log("══════════════════════════════════════");
            Log("  Deep Scan — tìm trong data value");
            Log("══════════════════════════════════════");

            var results = await Task.Run(() => RunDeepScan());

            if (IsDisposed) return; // form đóng trong khi đang quét

            foreach (var r in results)
            {
                bool safe = IsSafeToDelete(r);
                string dataCol = Truncate(r.Data, 120);
                if (IsKernelProtected(r))
                    dataCol = "⚠ Kernel lock — tự xóa sau reboot";
                else if (r.ValueName.IndexOf("AppCompatCache", StringComparison.OrdinalIgnoreCase) >= 0)
                    dataCol = "⚠ AppCompatCache — Windows rebuild sau reboot";
                else if (r.Type == "REG_BINARY")
                    dataCol = "REG_BINARY — không xóa value binary từng phần";

                var item = new ListViewItem(new[] { r.Key, r.ValueName, r.Type, dataCol });
                item.Tag = r;
                item.Checked = safe;
                if (!safe) item.ForeColor = Color.DarkOrange;
                lstDeepScan.Items.Add(item);
            }

            lblDeepCount.Text = $"{results.Count} mục tìm thấy";
            SetScanState(scanning: false);
            btnDeleteDeep.Enabled = results.Count > 0;
            btnCheckAll.Enabled = btnUncheckAll.Enabled = results.Count > 0;
            Log($"[✔] Hoàn tất: {results.Count} mục.");
        }

        private List<DeepResult> RunDeepScan()
        {
            // Tạo tất cả tổ hợp (term × scope) rồi chạy song song
            var combos = new List<(string scope, string term)>();
            foreach (string term in SearchTerms)
                foreach (string scope in Scopes)
                    combos.Add((scope, term));

            var bag = new ConcurrentBag<DeepResult>();

            Parallel.ForEach(combos, combo =>
            {
                InvokeLog($"[*] Quét {combo.scope} /f \"{combo.term}\"...");
                var items = QueryRegData(combo.scope, combo.term);
                foreach (var r in items) bag.Add(r);
                InvokeLog($"    [+] {combo.scope}: {items.Count} kết quả.");
            });

            // Gộp và loại trùng
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var results = new List<DeepResult>();
            foreach (var r in bag)
                if (seen.Add(r.Key + "|" + r.ValueName))
                    results.Add(r);

            return results;
        }

        private static List<DeepResult> QueryRegData(string scope, string term)
        {
            var results = new List<DeepResult>();
            var psi = new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = $"query \"{ExpandHive(scope)}\" /f \"{term}\" /s /d",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.Default,
            };

            string output;
            try
            {
                using (var p = Process.Start(psi))
                {
                    output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(120000);
                }
            }
            catch { return results; }

            // Phân tích output:
            //   HKEY_LOCAL_MACHINE\SOFTWARE\SomeKey
            //       ValueName    REG_SZ    Data
            string currentKey = null;
            foreach (string rawLine in output.Split('\n'))
            {
                string line = rawLine.TrimEnd('\r');
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("End of search", StringComparison.OrdinalIgnoreCase)) continue;

                if (!line.StartsWith(" "))
                {
                    string t = line.Trim();
                    if (t.StartsWith("HKEY_", StringComparison.OrdinalIgnoreCase))
                        currentKey = CompressHive(t);
                }
                else if (currentKey != null)
                {
                    // Fields separated by 2+ whitespace: Name    REG_TYPE    Data
                    var m = Regex.Match(line.TrimStart(), @"^(.*?)\s{2,}(REG_\w+)(?:\s{2,}(.*))?$");
                    if (m.Success)
                    {
                        results.Add(new DeepResult
                        {
                            Key       = currentKey,
                            ValueName = m.Groups[1].Value.Trim(),
                            Type      = m.Groups[2].Value,
                            Data      = m.Groups[3].Value.Trim(),
                        });
                    }
                }
            }

            return results;
        }

        // ── Xóa ──────────────────────────────────────────────────────────────

        private void btnDeleteDeep_Click(object sender, EventArgs e)
        {
            var targets = new List<DeepResult>();
            foreach (ListViewItem item in lstDeepScan.Items)
                if (item.Checked && item.Tag is DeepResult r) targets.Add(r);

            if (targets.Count == 0) { MessageBox.Show("Chưa chọn mục nào."); return; }

            var confirm = MessageBox.Show(
                $"Sẽ backup registry liên quan trước, rồi xóa {targets.Count} data value.\nTiếp tục?",
                "Xác nhận Deep Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            BackupAffectedKeys(targets);

            int deleted = 0, failed = 0;
            foreach (var r in targets)
            {
                try
                {
                    DeleteValue(r);
                    Log($"[✔] Xóa value: {r.Key}  →  {r.ValueName}");
                    deleted++;
                }
                catch (Exception ex)
                {
                    Log($"[✘] Lỗi: {r.Key} → {r.ValueName}: {ex.Message}");
                    failed++;
                }
            }

            Log($"Xóa xong: {deleted} thành công, {failed} thất bại.");
            btnDeepScan.PerformClick(); // quét lại
        }

        private static void DeleteValue(DeepResult r)
        {
            (RegistryKey hive, string subPath) = ParsePath(r.Key);
            if (hive == null) throw new Exception("Không nhận dạng hive: " + r.Key);

            using (RegistryKey key = hive.OpenSubKey(subPath, writable: true))
            {
                if (key == null) return;
                string vname = r.ValueName == "(Default)" ? "" : r.ValueName;
                key.DeleteValue(vname, throwOnMissingValue: false);
            }
        }

        // ── Backup ───────────────────────────────────────────────────────────

        private void BackupAffectedKeys(List<DeepResult> targets)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string backupDir = Path.Combine(desktop,
                $"HIS_DeepBackup_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(backupDir);
            Log($"[*] Backup vào: {backupDir}");

            var done = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in targets)
            {
                if (!done.Add(r.Key)) continue;
                string safeName = Regex.Replace(r.Key, @"[\\:*?""<>|]", "_") + ".reg";
                string outFile  = Path.Combine(backupDir, safeName);
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName  = "reg.exe",
                        Arguments = $"export \"{ExpandHive(r.Key)}\" \"{outFile}\" /y",
                        UseShellExecute    = false,
                        CreateNoWindow     = true,
                    };
                    using (var p = Process.Start(psi)) p?.WaitForExit(10000);
                    Log($"[✔] Backup: {r.Key}");
                }
                catch (Exception ex) { Log($"[!] Backup lỗi: {ex.Message}"); }
            }

            MessageBox.Show($"Đã backup {done.Count} key vào:\n{backupDir}",
                "Backup hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstDeepScan.Items) item.Checked = true;
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstDeepScan.Items) item.Checked = false;
        }

        private void btnCloseDeep_Click(object sender, EventArgs e) => this.Close();

        private void SetScanState(bool scanning)
        {
            btnDeepScan.Enabled   = !scanning;
            btnDeleteDeep.Enabled = false;
            btnCheckAll.Enabled   = !scanning;
            btnUncheckAll.Enabled = !scanning;
            progressDeep.Style = scanning
                ? ProgressBarStyle.Marquee
                : ProgressBarStyle.Blocks;
            lblDeepCount.Text = scanning ? "Đang quét..." : lblDeepCount.Text;
        }

        private static bool IsSafeToDelete(DeepResult r)
        {
            // Binary data — không sửa được surgically
            if (r.Type == "REG_BINARY") return false;
            // AppCompatCache — Windows tự rebuild nhưng không nên xóa tự động
            if (r.ValueName.IndexOf("AppCompatCache", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            // Enum\Root\LEGACY_* — kernel đang giữ lock, không xóa được khi đang chạy
            if (IsKernelProtected(r)) return false;
            return true;
        }

        private static bool IsKernelProtected(DeepResult r)
            => r.Key.IndexOf("\\Enum\\Root\\LEGACY_", StringComparison.OrdinalIgnoreCase) >= 0;

        private static string Truncate(string s, int max)
            => s != null && s.Length > max ? s.Substring(0, max) + "…" : s ?? "";

        private static string ExpandHive(string path)
        {
            if (path.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase))
                return "HKEY_LOCAL_MACHINE\\" + path.Substring(5);
            if (path.StartsWith("HKCU\\", StringComparison.OrdinalIgnoreCase))
                return "HKEY_CURRENT_USER\\" + path.Substring(5);
            if (path.Equals("HKU", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("HKU\\", StringComparison.OrdinalIgnoreCase))
                return "HKEY_USERS" + path.Substring(3);
            if (path.StartsWith("HKCR\\", StringComparison.OrdinalIgnoreCase))
                return "HKEY_CLASSES_ROOT\\" + path.Substring(5);
            return path;
        }

        private static string CompressHive(string path)
        {
            if (path.StartsWith("HKEY_LOCAL_MACHINE\\", StringComparison.OrdinalIgnoreCase))
                return "HKLM\\" + path.Substring(19);
            if (path.StartsWith("HKEY_CURRENT_USER\\", StringComparison.OrdinalIgnoreCase))
                return "HKCU\\" + path.Substring(18);
            if (path.StartsWith("HKEY_USERS\\", StringComparison.OrdinalIgnoreCase))
                return "HKU\\" + path.Substring(11);
            if (path.StartsWith("HKEY_CLASSES_ROOT\\", StringComparison.OrdinalIgnoreCase))
                return "HKCR\\" + path.Substring(18);
            return path;
        }

        private static (RegistryKey, string) ParsePath(string fullPath)
        {
            if (fullPath.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.LocalMachine, fullPath.Substring(5));
            if (fullPath.StartsWith("HKCU\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.CurrentUser, fullPath.Substring(5));
            if (fullPath.StartsWith("HKU\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.Users, fullPath.Substring(4));
            if (fullPath.StartsWith("HKCR\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.ClassesRoot, fullPath.Substring(5));
            return (null, null);
        }

        private void InvokeLog(string msg)
        {
            if (IsDisposed || txtDeepLog.IsDisposed) return;
            if (txtDeepLog.InvokeRequired) txtDeepLog.Invoke(new Action(() => Log(msg)));
            else Log(msg);
        }

        private void Log(string msg)
        {
            if (IsDisposed || txtDeepLog.IsDisposed) return;
            if (txtDeepLog.InvokeRequired) { txtDeepLog.Invoke(new Action(() => Log(msg))); return; }
            txtDeepLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            txtDeepLog.ScrollToCaret();
        }
    }

    internal class DeepResult
    {
        public string Key;
        public string ValueName;
        public string Type;
        public string Data;
    }
}
