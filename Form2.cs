using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frm_HIS_uninstaller_tool
{
    public partial class Form2 : Form
    {
        // ── Thư mục cần quét file thừa ───────────────────────────────────
        private static readonly string[] FileScanRoots = {
            @"C:\Program Files\Huorong",
            @"C:\Program Files (x86)\Huorong",
            @"C:\ProgramData\Huorong",
            @"C:\ProgramData\HuorongSysdiag",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Huorong"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Huorong"),
            @"C:\Windows\System32\drivers\sysdiag.sys",
            @"C:\Windows\SysWOW64\drivers\sysdiag.sys",
        };

        // ── Từ khoá registry cần quét ─────────────────────────────────────
        private static readonly string[] RegistryKeywords = {
            "Huorong", "HuorongSysdiag", "Sysdiag", "HipsDaemon", "HipsMain",
            "HipsSvc", "HipsFilter", "HuorongAV", "火绒"
        };

        // Các hive gốc để tìm kiếm
        private static readonly (RegistryKey Hive, string HiveName)[] RegistryHives = {
            (Registry.LocalMachine, @"HKLM"),
            (Registry.CurrentUser,  @"HKCU"),
        };

        // Các đường dẫn cố định cần quét (key hoặc Run-value)
        private static readonly string[] FixedScanPaths = {
            @"SOFTWARE\Huorong",
            @"SOFTWARE\WOW6432Node\Huorong",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag",
            @"SYSTEM\CurrentControlSet\Services\sysdiag",
            @"SYSTEM\CurrentControlSet\Services\HipsDaemon",
            @"SYSTEM\CurrentControlSet\Services\HipsMain",
            @"SYSTEM\CurrentControlSet\Services\HipsSvc",
            @"SYSTEM\CurrentControlSet\Services\HipsFilter",
            @"SYSTEM\CurrentControlSet\Services\HuorongAV",
            @"SYSTEM\CurrentControlSet\Services\HuorongAgent",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",   // filter by value name
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", // filter by value name
        };

        // Các Services sub-paths cần quét động để tìm thêm key còn sót
        private static readonly string[] DynamicServiceRoots = {
            @"SYSTEM\CurrentControlSet\Services",
            @"SYSTEM\ControlSet001\Services",
            @"SYSTEM\ControlSet002\Services",
        };

        // Driver device entries (Enum\Root\LEGACY_*) — không có tên "Huorong" nhưng là driver còn sót
        private static readonly string[] LegacyDriverKeywords = {
            "LEGACY_SYSDIAG", "LEGACY_HRWFPDRV", "LEGACY_HIPSDAEMON",
            "LEGACY_HIPSMAIN", "LEGACY_HIPSSVC", "LEGACY_HIPSFILTER",
        };

        // Gốc Enum\Root cần quét theo ControlSet
        private static readonly string[] EnumRootPaths = {
            @"SYSTEM\CurrentControlSet\Enum\Root",
            @"SYSTEM\ControlSet001\Enum\Root",
            @"SYSTEM\ControlSet002\Enum\Root",
        };

        // Các path chứa data value (REG_BINARY / REG_SZ) có liên quan Huorong
        private static readonly string[] DataScanRoots = {
            @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
            @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths",
            @"HKLM\SOFTWARE\Microsoft\Windows Search\CrawlScopeManager",
            @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager",
            @"HKLM\SYSTEM\ControlSet001\Control\Session Manager",
            @"HKLM\SYSTEM\ControlSet002\Control\Session Manager",
        };

        // "Huorong" ASCII: H u o r o n g
        private static readonly byte[] HuorongAscii  = { 0x48, 0x75, 0x6F, 0x72, 0x6F, 0x6E, 0x67 };
        // "Huorong" UTF-16LE
        private static readonly byte[] HuorongUtf16  = { 0x48,0x00,0x75,0x00,0x6F,0x00,0x72,0x00,0x6F,0x00,0x6E,0x00,0x67,0x00 };

        // Registry key quan trọng của hệ thống cần backup trước khi xóa
        private static readonly string[] CriticalBackupPaths = {
            @"HKLM\SYSTEM\CurrentControlSet\Services",
            @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
        };

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DeleteStartupTask();
            Log("Form2 khởi động — tự động quét...");
            ScanFiles();
            ScanRegistry();
        }

        private static void DeleteStartupTask()
        {
            // Xóa scheduled task
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = "/Delete /TN \"HuorongCleanupTool\" /F",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi)?.WaitForExit(5000);
            }
            catch { }

            // Xóa fallback RunOnce key (nếu đã ghi)
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", writable: true))
                    key?.DeleteValue("HuorongCleanupTool", throwOnMissingValue: false);
            }
            catch { }
        }

        // ─────────────────────────────────────────────────────────────────
        //  QUÉT FILE
        // ─────────────────────────────────────────────────────────────────

        private void ScanFiles()
        {
            if (lstFiles.InvokeRequired) { lstFiles.Invoke(new Action(ScanFiles)); return; }

            lstFiles.Items.Clear();
            btnDeleteFiles.Enabled = false;
            Log("Đang quét file/thư mục...");
            int count = 0;

            foreach (string path in FileScanRoots)
            {
                // Kiểm tra thư mục
                if (Directory.Exists(path))
                {
                    AddFileItem(path, isDirectory: true);
                    count++;

                    // Liệt kê file con (1 cấp) để người dùng xem rõ
                    try
                    {
                        foreach (string f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                        {
                            AddFileItem(f, isDirectory: false);
                            count++;
                        }
                    }
                    catch { }
                }
                else if (File.Exists(path))
                {
                    AddFileItem(path, isDirectory: false);
                    count++;
                }
            }

            lblFilesCount.Text = $"{count} mục tìm thấy";
            if (count > 0) btnDeleteFiles.Enabled = true;
            Log($"Quét file xong: {count} mục.");
        }

        private void AddFileItem(string path, bool isDirectory)
        {
            string name = isDirectory ? Path.GetFileName(path) + "\\" : Path.GetFileName(path);
            string size = "";
            if (!isDirectory)
            {
                try
                {
                    long bytes = new FileInfo(path).Length;
                    size = bytes < 1024 ? $"{bytes} B"
                         : bytes < 1024 * 1024 ? $"{bytes / 1024} KB"
                         : $"{bytes / 1024 / 1024} MB";
                }
                catch { size = "?"; }
            }
            else
            {
                name = "[Thư mục] " + name;
            }

            var item = new ListViewItem(new[] { name, path, size });
            item.Tag = path;
            item.Checked = true;
            if (isDirectory) item.ForeColor = Color.DarkBlue;
            lstFiles.Items.Add(item);
        }

        // ─────────────────────────────────────────────────────────────────
        //  XÓA FILE
        // ─────────────────────────────────────────────────────────────────
        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            var selected = new List<string>();
            foreach (ListViewItem item in lstFiles.Items)
                if (item.Checked) selected.Add(item.Tag?.ToString());

            if (selected.Count == 0) { MessageBox.Show("Chưa chọn mục nào."); return; }

            var confirm = MessageBox.Show(
                $"Sẽ xóa {selected.Count} mục. Không thể khôi phục!\nTiếp tục?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            int deleted = 0, failed = 0;
            foreach (string path in selected)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, recursive: true);
                        Log($"[✔] Xóa thư mục: {path}");
                        deleted++;
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        Log($"[✔] Xóa file: {path}");
                        deleted++;
                    }
                }
                catch (Exception ex)
                {
                    Log($"[✘] Không xóa được {path}: {ex.Message}");
                    failed++;
                }
            }

            Log($"Xóa file xong: {deleted} thành công, {failed} thất bại.");
            ScanFiles(); // Quét lại
        }

        // ─────────────────────────────────────────────────────────────────
        //  QUÉT REGISTRY
        // ─────────────────────────────────────────────────────────────────

        private void ScanRegistry()
        {
            if (lstRegistry.InvokeRequired) { lstRegistry.Invoke(new Action(ScanRegistry)); return; }

            lstRegistry.Items.Clear();
            btnDeleteReg.Enabled = false;
            Log("Đang quét registry...");
            int count = 0;

            // 1. Fixed known paths — scan on HKLM only (Services/Software)
            //    and also HKCU for Software/Run paths
            foreach (var (hive, hiveName) in RegistryHives)
            {
                foreach (string subPath in FixedScanPaths)
                {
                    // Services paths only make sense in HKLM
                    if (subPath.StartsWith("SYSTEM\\", StringComparison.OrdinalIgnoreCase) &&
                        hive != Registry.LocalMachine) continue;

                    try
                    {
                        using (RegistryKey key = hive.OpenSubKey(subPath, writable: false))
                        {
                            if (key == null) continue;
                            string fullPath = $"{hiveName}\\{subPath}";

                            bool isValueFilter = subPath.EndsWith("\\Run", StringComparison.OrdinalIgnoreCase)
                                             || subPath.EndsWith("\\RunOnce", StringComparison.OrdinalIgnoreCase);

                            if (isValueFilter)
                            {
                                foreach (string valueName in key.GetValueNames())
                                {
                                    if (ContainsHuorongKeyword(valueName) ||
                                        ContainsHuorongKeyword(key.GetValue(valueName)?.ToString()))
                                    {
                                        AddRegItem($"{fullPath} → {valueName}",
                                            key.GetValue(valueName)?.ToString(), fullPath, valueName);
                                        count++;
                                    }
                                }
                            }
                            else
                            {
                                AddRegItem(fullPath, "(key)", fullPath, null);
                                count++;
                            }
                        }
                    }
                    catch { }
                }
            }

            // 2. Dynamic scan: enumerate Services sub-keys for any Huorong remnants
            foreach (string servicesPath in DynamicServiceRoots)
            {
                try
                {
                    using (RegistryKey servicesKey = Registry.LocalMachine.OpenSubKey(servicesPath, writable: false))
                    {
                        if (servicesKey == null) continue;
                        foreach (string subName in servicesKey.GetSubKeyNames())
                        {
                            if (!ContainsHuorongKeyword(subName)) continue;
                            string fullPath = $"HKLM\\{servicesPath}\\{subName}";
                            if (IsAlreadyListed(fullPath)) continue;
                            AddRegItem(fullPath, "(service key)", fullPath, null);
                            count++;
                        }
                    }
                }
                catch { }
            }

            // 3. Enum\Root scan: LEGACY_SYSDIAG / LEGACY_HRWFPDRV etc. — kernel driver device entries
            foreach (string enumPath in EnumRootPaths)
            {
                try
                {
                    using (RegistryKey enumKey = Registry.LocalMachine.OpenSubKey(enumPath, writable: false))
                    {
                        if (enumKey == null) continue;
                        foreach (string subName in enumKey.GetSubKeyNames())
                        {
                            bool match = false;
                            foreach (string kw in LegacyDriverKeywords)
                                if (subName.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                                { match = true; break; }
                            if (!match) continue;

                            string fullPath = $"HKLM\\{enumPath}\\{subName}";
                            if (IsAlreadyListed(fullPath)) continue;
                            var legacyItem = new ListViewItem(new[] {
                                fullPath,
                                "⚠ driver device — kernel lock, tự xóa sau reboot"
                            });
                            legacyItem.Tag = new RegTarget { KeyPath = fullPath, ValueName = null };
                            legacyItem.Checked = false;
                            legacyItem.ForeColor = Color.DarkOrange;
                            lstRegistry.Items.Add(legacyItem);
                            count++;
                        }
                    }
                }
                catch { }
            }

            // 4. Quét data value (REG_BINARY / REG_SZ) chứa "Huorong" trong các path cụ thể
            var dataSeen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string root in DataScanRoots)
            {
                (RegistryKey hive, string subPath) = ParseRegistryPath(root);
                if (hive == null) continue;
                string hiveName = root.Substring(0, root.IndexOf('\\'));
                ScanDataValuesRecursive(hive, subPath, hiveName, ref count, dataSeen, depth: 0);
            }

            lblRegCount.Text = $"{count} mục tìm thấy";
            if (count > 0) btnDeleteReg.Enabled = true;
            Log($"Quét registry xong: {count} mục.");
        }

        private void ScanDataValuesRecursive(RegistryKey hive, string subPath, string hiveName,
            ref int count, System.Collections.Generic.HashSet<string> seen, int depth)
        {
            if (depth > 5) return;
            try
            {
                using (RegistryKey key = hive.OpenSubKey(subPath, writable: false))
                {
                    if (key == null) return;
                    string fullKeyPath = $"{hiveName}\\{subPath}";

                    foreach (string valueName in key.GetValueNames())
                    {
                        string dedup = fullKeyPath + "|" + valueName;
                        if (!seen.Add(dedup)) continue;

                        RegistryValueKind kind;
                        object val;
                        try
                        {
                            kind = key.GetValueKind(valueName);
                            val  = key.GetValue(valueName, null,
                                RegistryValueOptions.DoNotExpandEnvironmentNames);
                        }
                        catch { continue; }

                        bool match = false;
                        string preview = "";
                        bool isCritical = false;

                        if (kind == RegistryValueKind.Binary && val is byte[] bytes)
                        {
                            match = ContainsByteSequence(bytes, HuorongAscii)
                                 || ContainsByteSequence(bytes, HuorongUtf16);
                            if (match)
                            {
                                preview = "REG_BINARY (chứa 'Huorong')";
                                // AppCompatCache — Windows tự rebuild sau reboot, an toàn xóa nhưng đánh dấu rõ
                                if (valueName.IndexOf("AppCompatCache", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    preview = "⚠ AppCompatCache — xóa sẽ reset app-compat cache";
                                    isCritical = true;
                                }
                            }
                        }
                        else if (kind == RegistryValueKind.String
                              || kind == RegistryValueKind.ExpandString)
                        {
                            string s = val?.ToString() ?? "";
                            match = s.IndexOf("Huorong", StringComparison.OrdinalIgnoreCase) >= 0;
                            if (match) preview = s.Length > 80 ? s.Substring(0, 80) + "…" : s;
                        }
                        else if (kind == RegistryValueKind.MultiString && val is string[] arr)
                        {
                            foreach (string s in arr)
                                if (s.IndexOf("Huorong", StringComparison.OrdinalIgnoreCase) >= 0)
                                { match = true; preview = s; break; }
                        }

                        if (!match) continue;

                        string display = $"{fullKeyPath} → {(string.IsNullOrEmpty(valueName) ? "(Default)" : valueName)}";
                        var item = new ListViewItem(new[] { display, preview });
                        item.Tag = new RegTarget { KeyPath = fullKeyPath, ValueName = valueName };
                        item.Checked = !isCritical;
                        if (isCritical) item.ForeColor = Color.DarkOrange;
                        lstRegistry.Items.Add(item);
                        count++;
                    }

                    foreach (string sub in key.GetSubKeyNames())
                        ScanDataValuesRecursive(hive, $"{subPath}\\{sub}", hiveName,
                            ref count, seen, depth + 1);
                }
            }
            catch { }
        }

        private static bool ContainsByteSequence(byte[] data, byte[] pattern)
        {
            int limit = data.Length - pattern.Length;
            for (int i = 0; i <= limit; i++)
            {
                bool ok = true;
                for (int j = 0; j < pattern.Length; j++)
                    if (data[i + j] != pattern[j]) { ok = false; break; }
                if (ok) return true;
            }
            return false;
        }

        private bool IsAlreadyListed(string fullPath)
        {
            foreach (ListViewItem existing in lstRegistry.Items)
                if (existing.Tag is RegTarget rt &&
                    string.Equals(rt.KeyPath, fullPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static bool ContainsHuorongKeyword(string s)
        {
            if (s == null) return false;
            foreach (string kw in RegistryKeywords)
                if (s.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        private void AddRegItem(string keyDisplay, string value, string keyPath, string valueName)
        {
            var item = new ListViewItem(new[] { keyDisplay, value ?? "" });
            item.Tag = new RegTarget { KeyPath = keyPath, ValueName = valueName };
            item.Checked = true;
            lstRegistry.Items.Add(item);
        }

        private class RegTarget
        {
            public string KeyPath;   // e.g. "HKLM\SOFTWARE\Huorong"
            public string ValueName; // null = xóa cả key
        }

        // ─────────────────────────────────────────────────────────────────
        //  XÓA REGISTRY (có backup trước)
        // ─────────────────────────────────────────────────────────────────
        private async void btnDeleteReg_Click(object sender, EventArgs e)
        {
            var targets = new List<RegTarget>();
            foreach (ListViewItem item in lstRegistry.Items)
                if (item.Checked && item.Tag is RegTarget rt) targets.Add(rt);

            if (targets.Count == 0) { MessageBox.Show("Chưa chọn mục nào."); return; }

            var confirm = MessageBox.Show(
                $"Sẽ backup registry hệ thống trước, rồi xóa {targets.Count} mục.\nTiếp tục?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            btnDeleteReg.Enabled = false;
            Log("[*] Đang backup và xóa...");

            int deleted = 0, failed = 0;
            string backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"HIS_RegBackup_{DateTime.Now:yyyyMMdd_HHmmss}");

            // Chạy backup + xóa trên background thread — UI không bị đơ
            await Task.Run(() =>
            {
                BackupSystemRegistry(backupDir);
                foreach (var rt in targets)
                {
                    try
                    {
                        DeleteRegistryTarget(rt);
                        Log($"[✔] Xóa: {rt.KeyPath}" + (rt.ValueName != null ? $" → {rt.ValueName}" : ""));
                        deleted++;
                    }
                    catch (Exception ex)
                    {
                        Log($"[✘] Lỗi {rt.KeyPath}: {ex.Message}");
                        failed++;
                    }
                }
            });

            // Trở về UI thread — hiện kết quả và quét lại
            Log($"Xóa xong: {deleted} thành công, {failed} thất bại." +
                (failed > 0 ? " (Key được kernel bảo vệ — reboot để xóa sạch.)" : ""));
            MessageBox.Show($"Đã backup vào:\n{backupDir}\n\nXóa: {deleted} thành công, {failed} thất bại.",
                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ScanRegistry(); // chạy trên UI thread — cập nhật list ngay lập tức
        }

        private void DeleteRegistryTarget(RegTarget rt)
        {
            (RegistryKey hive, string subPath) = ParseRegistryPath(rt.KeyPath);
            if (hive == null) throw new Exception($"Không nhận dạng được hive: {rt.KeyPath}");

            if (rt.ValueName != null)
            {
                using (RegistryKey key = hive.OpenSubKey(subPath, writable: true))
                {
                    if (key == null) return; // already gone
                    key.DeleteValue(rt.ValueName, throwOnMissingValue: false);
                }
                return;
            }

            // Delete entire key tree
            int lastSlash = subPath.LastIndexOf('\\');
            if (lastSlash < 0)
            {
                // Direct child of hive root — rare but handle it
                hive.DeleteSubKeyTree(subPath, throwOnMissingSubKey: false);
            }
            else
            {
                string parentPath = subPath.Substring(0, lastSlash);
                string keyName    = subPath.Substring(lastSlash + 1);
                using (RegistryKey parent = hive.OpenSubKey(parentPath, writable: true))
                {
                    if (parent == null) return; // parent already gone
                    parent.DeleteSubKeyTree(keyName, throwOnMissingSubKey: false);
                }
            }
        }

        private (RegistryKey hive, string subPath) ParseRegistryPath(string fullPath)
        {
            if (fullPath.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.LocalMachine, fullPath.Substring(5));
            if (fullPath.StartsWith("HKCU\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.CurrentUser, fullPath.Substring(5));
            if (fullPath.StartsWith("HKU\\", StringComparison.OrdinalIgnoreCase))
                return (Registry.Users, fullPath.Substring(4));
            return (null, null);
        }

        // ─────────────────────────────────────────────────────────────────
        //  BACKUP REGISTRY HỆ THỐNG (dùng reg.exe export)
        // ─────────────────────────────────────────────────────────────────
        private void BackupSystemRegistry(string backupDir)
        {
            Directory.CreateDirectory(backupDir);
            Log($"[*] Backup vào: {backupDir}");

            foreach (string regPath in CriticalBackupPaths)
            {
                string safeName = regPath.Replace('\\', '_').Replace(':', '_') + ".reg";
                string outFile = Path.Combine(backupDir, safeName);
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = $"export \"{regPath}\" \"{outFile}\" /y",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    using (var p = Process.Start(psi)) p.WaitForExit(15000);
                    Log($"[✔] Backup: {regPath}");
                }
                catch (Exception ex) { Log($"[!] Backup lỗi {regPath}: {ex.Message}"); }
            }

            // Backup SOFTWARE\Huorong nếu còn tồn tại
            try
            {
                var psi2 = new ProcessStartInfo
                {
                    FileName = "reg.exe",
                    Arguments = $"export \"HKLM\\SOFTWARE\\Huorong\" \"{Path.Combine(backupDir, "Huorong_Software.reg")}\" /y",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var p = Process.Start(psi2)) p.WaitForExit(5000);
            }
            catch { }

            Log("[✔] Backup hoàn tất.");
        }

        // ─────────────────────────────────────────────────────────────────
        //  ĐÓNG FORM — xóa startup key
        // ─────────────────────────────────────────────────────────────────
        private void btnDeepScan_Click(object sender, EventArgs e)
        {
            using (Form3 f3 = new Form3())
                f3.ShowDialog(this);
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        // ─────────────────────────────────────────────────────────────────
        //  LOG (thread-safe)
        // ─────────────────────────────────────────────────────────────────
        private void Log(string msg)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => Log(msg))); return; }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            txtLog.ScrollToCaret();
        }
    }
}
