using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frm_HIS_uninstaller_tool
{
    public partial class Form2 : Form
    {
        // Byte pattern của "Huorong" — ASCII và UTF-16LE
        private static readonly byte[] HuorongAscii = { 0x48, 0x75, 0x6F, 0x72, 0x6F, 0x6E, 0x67 };
        private static readonly byte[] HuorongUtf16 = { 0x48,0x00,0x75,0x00,0x6F,0x00,0x72,0x00,0x6F,0x00,0x6E,0x00,0x67,0x00 };

        // Hive gốc để duyệt
        private static readonly (RegistryKey Hive, string HiveName)[] RegistryHives = {
            (Registry.LocalMachine, "HKLM"),
            (Registry.CurrentUser,  "HKCU"),
        };

        public Form2() => InitializeComponent();

        private void Form2_Load(object sender, EventArgs e)
        {
            ApplyLanguage();
            var donateImg = ResourceHelper.LoadImage("donate.jpg");
            if (donateImg != null) pbDonate.Image = donateImg;
            DeleteStartupTask();
            Log("Form2 khởi động — tự động quét...");
            ScanFiles();
            ScanRegistry();
        }

        private void ApplyLanguage()
        {
            var L = LangManager.L;
            this.Text           = L.Get("form2.title");
            lblTitle.Text       = L.Get("form2.maintitle");
            lblSubtitle.Text    = L.Get("form2.subtitle");
            lblFileHeader.Text  = L.Get("form2.hdr.files");
            lblRegHeader.Text   = L.Get("form2.hdr.registry");
            lblRegWarning.Text  = L.Get("form2.warn.backup");
            btnDeleteFiles.Text = L.Get("form2.btn.deletefile");
            btnDeleteReg.Text   = L.Get("form2.btn.deletereg");
            btnDeepScan.Text    = L.Get("form2.btn.deepscan");
            btnClose.Text       = L.Get("form2.btn.close");
        }

        private static void DeleteStartupTask()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = "/Delete /TN \"HuorongCleanupTool\" /F",
                    UseShellExecute = false,
                    CreateNoWindow  = true,
                };
                Process.Start(psi)?.WaitForExit(5000);
            }
            catch { }
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", writable: true))
                    key?.DeleteValue("HuorongCleanupTool", throwOnMissingValue: false);
            }
            catch { }
        }

        // ─────────────────────────────────────────────────────────────────
        //  DỪNG SERVICE / DRIVER TRƯỚC KHI XÓA FILE
        // ─────────────────────────────────────────────────────────────────

        private void StopHuorongServices()
        {
            string[] services = CleanupConfig.Instance.HuorongServices;
            Log("[*] Đang dừng các service / driver của Huorong...");

            foreach (string svc in services)
            {
                RunSc("stop",   svc);
                RunSc("delete", svc);
                Log($"    [sc] stop + delete: {svc}");
            }
        }

        private static void RunSc(string verb, string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName  = "sc.exe",
                    Arguments = $"{verb} \"{serviceName}\"",
                    UseShellExecute = false,
                    CreateNoWindow  = true,
                };
                Process.Start(psi)?.WaitForExit(5000);
            }
            catch { }
        }

        private static void RestartWindowsSearch()
        {
            RunSc("stop", "WSearch");
            System.Threading.Thread.Sleep(1500);
            RunSc("start", "WSearch");
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

            // 1. Các đường dẫn cố định từ config
            foreach (string path in CleanupConfig.Instance.FileScanRoots)
            {
                if (Directory.Exists(path))
                {
                    AddFileItem(path, isDirectory: true);
                    count++;
                    try
                    {
                        foreach (string sub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                        { AddFileItem(sub, isDirectory: true); count++; }
                    }
                    catch { }
                    try
                    {
                        foreach (string f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                        { AddFileItem(f, isDirectory: false); count++; }
                    }
                    catch { }
                }
                else if (File.Exists(path))
                { AddFileItem(path, isDirectory: false); count++; }
            }

            // 2. Quét DriverStore\FileRepository — lọc theo keyword
            count += ScanDriverStore();

            lblFilesCount.Text = $"{count} mục tìm thấy";
            if (count > 0) btnDeleteFiles.Enabled = true;
            Log($"Quét file xong: {count} mục.");
        }

        /// <summary>
        /// Tìm các thư mục trong DriverStore\FileRepository liên quan đến Huorong.
        /// Pass 1: khớp theo tên thư mục.
        /// Pass 2: với thư mục không khớp tên (tên ngẫu nhiên như oem12.inf_x64_...),
        ///         đọc nội dung file .inf bên trong để tìm từ khoá.
        /// </summary>
        private int ScanDriverStore()
        {
            int found = 0;
            string[] keywords = CleanupConfig.Instance.RegistryKeywords;

            foreach (string driverStoreRoot in CleanupConfig.Instance.DriverStoreRoots)
            {
                if (!Directory.Exists(driverStoreRoot)) continue;

                try
                {
                    foreach (string subDir in Directory.GetDirectories(driverStoreRoot))
                    {
                        string dirName = Path.GetFileName(subDir);

                        // Pass 1: khớp tên thư mục
                        bool nameMatch = false;
                        foreach (string kw in keywords)
                            if (dirName.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                            { nameMatch = true; break; }

                        // Pass 2: quét nội dung .inf nếu tên không khớp
                        bool infMatch = false;
                        if (!nameMatch)
                            infMatch = InfContainsHuorong(subDir, keywords);

                        if (!nameMatch && !infMatch) continue;

                        string tag = infMatch && !nameMatch ? " [qua .inf]" : "";
                        Log($"[*] DriverStore{tag}: {dirName}");

                        AddFileItem(subDir, isDirectory: true);
                        found++;
                        try
                        {
                            foreach (string f in Directory.GetFiles(subDir, "*", SearchOption.AllDirectories))
                            { AddFileItem(f, isDirectory: false); found++; }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            if (found > 0) Log($"[*] DriverStore: {found} mục liên quan Huorong.");
            return found;
        }

        /// <summary>Đọc các file .inf trong subDir (không đệ quy) và tìm từ khoá.</summary>
        private static bool InfContainsHuorong(string subDir, string[] keywords)
        {
            try
            {
                foreach (string inf in Directory.GetFiles(subDir, "*.inf", SearchOption.TopDirectoryOnly))
                {
                    string content;
                    try { content = File.ReadAllText(inf, Encoding.Default); }
                    catch { continue; }

                    foreach (string kw in keywords)
                        if (content.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                }
            }
            catch { }
            return false;
        }

        private void AddFileItem(string path, bool isDirectory)
        {
            string name = isDirectory
                ? "[Thư mục] " + Path.GetFileName(path) + "\\"
                : Path.GetFileName(path);

            string size = "";
            if (!isDirectory)
            {
                try
                {
                    long bytes = new FileInfo(path).Length;
                    size = bytes < 1024          ? $"{bytes} B"
                         : bytes < 1024 * 1024   ? $"{bytes / 1024} KB"
                                                 : $"{bytes / 1024 / 1024} MB";
                }
                catch { size = "?"; }
            }

            var item = new ListViewItem(new[] { name, path, size });
            item.Tag     = path;
            item.Checked = true;
            if (isDirectory) item.ForeColor = Color.DarkBlue;
            lstFiles.Items.Add(item);
        }

        // ─────────────────────────────────────────────────────────────────
        //  XÓA FILE  (dừng service → xóa → TakeOwnership khi cần)
        // ─────────────────────────────────────────────────────────────────

        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            var L = LangManager.L;
            var selected = new List<string>();
            foreach (ListViewItem item in lstFiles.Items)
            {
                if (!item.Checked) continue;
                string p = item.Tag?.ToString();
                if (!string.IsNullOrEmpty(p)) selected.Add(p);
            }

            if (selected.Count == 0) { MessageBox.Show(L.Get("form2.dlg.nosel")); return; }

            var confirm = MessageBox.Show(
                L.Get("form2.dlg.confirmfile", selected.Count),
                L.Get("form2.dlg.confirmfile.title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            var logger = new CleanupLogger();
            btnDeleteFiles.Enabled = false;
            StopHuorongServices();

            // Chỉ xử lý "root" — bỏ qua con của item cha đã được chọn
            var selectedSet = new HashSet<string>(selected, StringComparer.OrdinalIgnoreCase);
            var roots = selected.Where(p => !HasSelectedAncestor(p, selectedSet)).ToList();

            int deleted = 0, failed = 0;
            foreach (string path in roots)
            {
                if (Directory.Exists(path))
                    DeleteDirectoryClean(path, logger, ref deleted, ref failed);
                else if (File.Exists(path))
                    TryDeleteFile(path, logger, ref deleted, ref failed);
                // đã biến mất → bỏ qua
            }

            string reportPath = logger.ExportToDesktop("Xóa file / thư mục");
            Log($"Xóa file xong: {deleted} thành công, {failed} thất bại.");

            var LD = LangManager.L;
            MessageBox.Show(
                $"Xóa: {deleted} thành công, {failed} thất bại.\n\nBáo cáo đã lưu tại:\n{reportPath}",
                LD.Get("form2.dlg.done.title"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            ScanFiles();
        }

        // Tìm xem path có tổ tiên nào cũng nằm trong danh sách được chọn không
        private static bool HasSelectedAncestor(string path, HashSet<string> selectedSet)
        {
            string current = Path.GetDirectoryName(path);
            while (!string.IsNullOrEmpty(current))
            {
                if (selectedSet.Contains(current)) return true;
                string parent = Path.GetDirectoryName(current);
                if (parent == current) break; // đã tới root ổ đĩa
                current = parent;
            }
            return false;
        }

        // Xóa thư mục theo thứ tự: file → subdir sâu nhất trước → thư mục gốc cuối cùng
        private void DeleteDirectoryClean(string dirPath, CleanupLogger logger, ref int deleted, ref int failed)
        {
            if (!Directory.Exists(dirPath)) return;

            // Bước 1: xóa hết file bên trong (mọi cấp)
            try
            {
                foreach (string file in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                    TryDeleteFile(file, logger, ref deleted, ref failed);
            }
            catch { }

            // Bước 2: xóa subdir từ sâu nhất lên (path dài hơn = sâu hơn)
            try
            {
                var subdirs = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories)
                    .OrderByDescending(d => d.Length);
                foreach (string sub in subdirs)
                {
                    if (!Directory.Exists(sub)) continue;
                    TryDeleteEmptyDir(sub, logger, ref deleted, ref failed);
                }
            }
            catch { }

            // Bước 3: xóa thư mục gốc (lúc này đã rỗng hoặc gần rỗng)
            if (!Directory.Exists(dirPath)) return;
            TryDeleteEmptyDir(dirPath, logger, ref deleted, ref failed);
        }

        private void TryDeleteEmptyDir(string dirPath, CleanupLogger logger, ref int deleted, ref int failed)
        {
            try
            {
                Directory.Delete(dirPath, recursive: false);
                Log($"[✔] Xóa thư mục: {dirPath}");
                logger.Record($"[✔] Xóa thư mục: {dirPath}");
                deleted++;
            }
            catch (UnauthorizedAccessException)
            {
                // Last resort: cmd rmdir
                Log($"[!] Bị khóa — thử cmd rmdir: {dirPath}");
                if (TryForceDeleteDir(dirPath))
                {
                    Log($"[✔] Xóa thư mục (cmd): {dirPath}");
                    logger.Record($"[✔] Xóa thư mục (cmd): {dirPath}");
                    deleted++;
                }
                else
                {
                    Log($"[✘] Không xóa được thư mục: {dirPath}");
                    logger.Record($"[✘] Thất bại (thư mục): {dirPath}");
                    failed++;
                }
            }
            catch (IOException)
            {
                // Vẫn còn nội dung (file bị khóa bởi process khác) → cmd rmdir
                Log($"[!] Còn nội dung bị khóa — thử cmd rmdir: {dirPath}");
                if (TryForceDeleteDir(dirPath))
                {
                    Log($"[✔] Xóa thư mục (cmd): {dirPath}");
                    logger.Record($"[✔] Xóa thư mục (cmd): {dirPath}");
                    deleted++;
                }
                else
                {
                    Log($"[✘] Không xóa được thư mục: {dirPath}");
                    logger.Record($"[✘] Thất bại (thư mục): {dirPath}");
                    failed++;
                }
            }
            catch (Exception ex)
            {
                Log($"[✘] Lỗi xóa thư mục {dirPath}: {ex.Message}");
                logger.Record($"[✘] Thất bại: {dirPath}");
                failed++;
            }
        }

        private void TryDeleteFile(string filePath, CleanupLogger logger, ref int deleted, ref int failed)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                File.Delete(filePath);
                Log($"[✔] Xóa file: {filePath}");
                logger.Record($"[✔] Xóa file: {filePath}");
                deleted++;
            }
            catch (UnauthorizedAccessException)
            {
                Log($"[!] Không có quyền — thử TakeOwnership: {filePath}");
                if (PrivilegeHelper.TakeOwnershipOfFile(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        Log($"[✔] Xóa sau TakeOwnership: {filePath}");
                        logger.Record($"[✔] Xóa (sau TakeOwnership): {filePath}");
                        deleted++;
                        return;
                    }
                    catch { }
                }
                Log($"[✘] Không xóa được (cần reboot): {filePath}");
                logger.Record($"[✘] Thất bại (cần reboot): {filePath}");
                failed++;
            }
            catch (Exception ex)
            {
                Log($"[✘] Không xóa được {filePath}: {ex.Message}");
                logger.Record($"[✘] Thất bại: {filePath} — {ex.Message}");
                failed++;
            }
        }

        private static bool TryForceDeleteDir(string dirPath)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c rmdir /s /q \"{dirPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow  = true,
                };
                Process.Start(psi)?.WaitForExit(8000);
                return !Directory.Exists(dirPath);
            }
            catch { return false; }
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

            var cfg = CleanupConfig.Instance;

            // 1. Fixed known paths
            foreach (var (hive, hiveName) in RegistryHives)
            {
                foreach (string subPath in cfg.FixedScanPaths)
                {
                    if (subPath.StartsWith("SYSTEM\\", StringComparison.OrdinalIgnoreCase) &&
                        hive != Registry.LocalMachine) continue;

                    try
                    {
                        using (RegistryKey key = hive.OpenSubKey(subPath, writable: false))
                        {
                            if (key == null) continue;
                            string fullPath = $"{hiveName}\\{subPath}";

                            bool isRunFilter = subPath.EndsWith("\\Run", StringComparison.OrdinalIgnoreCase)
                                           || subPath.EndsWith("\\RunOnce", StringComparison.OrdinalIgnoreCase);

                            if (isRunFilter)
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

            // 2. Dynamic scan: Services sub-keys
            foreach (string servicesPath in cfg.DynamicServiceRoots)
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

            // 3. Enum\Root (LEGACY driver entries)
            foreach (string enumPath in cfg.EnumRootPaths)
            {
                try
                {
                    using (RegistryKey enumKey = Registry.LocalMachine.OpenSubKey(enumPath, writable: false))
                    {
                        if (enumKey == null) continue;
                        foreach (string subName in enumKey.GetSubKeyNames())
                        {
                            bool match = false;
                            foreach (string kw in cfg.LegacyDriverKeywords)
                                if (subName.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                                { match = true; break; }
                            if (!match) continue;

                            string fullPath = $"HKLM\\{enumPath}\\{subName}";
                            if (IsAlreadyListed(fullPath)) continue;

                            var legacyItem = new ListViewItem(new[] {
                                fullPath,
                                "⚠ driver device — kernel lock, tự xóa sau reboot"
                            });
                            legacyItem.Tag     = new RegTarget { KeyPath = fullPath, ValueName = null };
                            legacyItem.Checked = false;
                            legacyItem.ForeColor = Color.DarkOrange;
                            lstRegistry.Items.Add(legacyItem);
                            count++;
                        }
                    }
                }
                catch { }
            }

            // 4. Data value scan (REG_BINARY / REG_SZ chứa "Huorong")
            var dataSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string root in cfg.DataScanRoots)
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
            ref int count, HashSet<string> seen, int depth)
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

                        bool   match     = false;
                        string preview   = "";
                        bool   isCritical = false;

                        if (kind == RegistryValueKind.Binary && val is byte[] bytes)
                        {
                            match = ContainsByteSequence(bytes, HuorongAscii)
                                 || ContainsByteSequence(bytes, HuorongUtf16);
                            if (match)
                            {
                                preview = "REG_BINARY (chứa 'Huorong')";
                                if (valueName.IndexOf("AppCompatCache", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    preview   = "⚠ AppCompatCache — xóa sẽ reset app-compat cache";
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
                        item.Tag     = new RegTarget { KeyPath = fullKeyPath, ValueName = valueName };
                        item.Checked = !isCritical;
                        if (isCritical) item.ForeColor = Color.DarkOrange;
                        lstRegistry.Items.Add(item);
                        count++;
                    }

                    foreach (string sub in key.GetSubKeyNames())
                        ScanDataValuesRecursive(hive, $"{subPath}\\{sub}", hiveName, ref count, seen, depth + 1);
                }
            }
            catch { }
        }

        // ─────────────────────────────────────────────────────────────────
        //  XÓA REGISTRY  (backup → TakeOwnership khi cần → báo cáo)
        // ─────────────────────────────────────────────────────────────────

        private async void btnDeleteReg_Click(object sender, EventArgs e)
        {
            var L = LangManager.L;
            var targets = new List<RegTarget>();
            foreach (ListViewItem item in lstRegistry.Items)
                if (item.Checked && item.Tag is RegTarget rt) targets.Add(rt);

            if (targets.Count == 0) { MessageBox.Show(L.Get("form2.dlg.nosel")); return; }

            var confirm = MessageBox.Show(
                L.Get("form2.dlg.confirmreg", targets.Count),
                L.Get("form2.dlg.confirmreg.title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            btnDeleteReg.Enabled = false;
            Log("[*] Đang backup và xóa...");

            var logger = new CleanupLogger();
            int deleted = 0, failed = 0;
            string backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"HIS_RegBackup_{DateTime.Now:yyyyMMdd_HHmmss}");

            await Task.Run(() =>
            {
                BackupSystemRegistry(backupDir);
                foreach (var rt in targets)
                {
                    try
                    {
                        DeleteRegistryTarget(rt);
                        string entry = $"[✔] Xóa: {rt.KeyPath}" +
                            (rt.ValueName != null ? $" → {rt.ValueName}" : "");
                        Log(entry);
                        logger.Record(entry);
                        deleted++;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Thử TakeOwnership nếu là HKLM key
                        bool retried = false;
                        if (rt.KeyPath.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase))
                        {
                            string sub = rt.KeyPath.Substring(5);
                            if (PrivilegeHelper.TakeOwnershipOfRegistryKey(sub))
                            {
                                try
                                {
                                    DeleteRegistryTarget(rt);
                                    string entry = $"[✔] Xóa (sau TakeOwnership): {rt.KeyPath}";
                                    Log(entry);
                                    logger.Record(entry);
                                    deleted++;
                                    retried = true;
                                }
                                catch { }
                            }
                        }
                        if (!retried)
                        {
                            string entry = $"[✘] Không có quyền (kernel lock): {rt.KeyPath}";
                            Log(entry);
                            logger.Record(entry);
                            failed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        string entry = $"[✘] Lỗi {rt.KeyPath}: {ex.Message}";
                        Log(entry);
                        logger.Record(entry);
                        failed++;
                    }
                }
            });

            string reportPath = logger.ExportToDesktop("Xóa registry");
            Log($"Xóa xong: {deleted} thành công, {failed} thất bại." +
                (failed > 0 ? " (Key kernel-protected → reboot để sạch hoàn toàn.)" : ""));

            // Khởi động lại Windows Search nếu đã xóa CrawlScopeManager
            bool affectsCrawl = targets.Exists(rt =>
                rt.KeyPath.IndexOf("CrawlScopeManager", StringComparison.OrdinalIgnoreCase) >= 0);
            if (affectsCrawl)
            {
                var LW = LangManager.L;
                MessageBox.Show(LW.Get("form2.dlg.wsearch"), LW.Get("form2.dlg.wsearch.title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log("[*] Khởi động lại Windows Search (WSearch)...");
                RestartWindowsSearch();
                Log("[✔] WSearch đã khởi động lại.");
            }

            var LD2 = LangManager.L;
            MessageBox.Show(
                LD2.Get("form2.dlg.done", backupDir, deleted, failed, reportPath),
                LD2.Get("form2.dlg.done.title"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            ScanRegistry();
        }

        private void DeleteRegistryTarget(RegTarget rt)
        {
            (RegistryKey hive, string subPath) = ParseRegistryPath(rt.KeyPath);
            if (hive == null) throw new Exception($"Không nhận dạng được hive: {rt.KeyPath}");

            if (rt.ValueName != null)
            {
                using (RegistryKey key = hive.OpenSubKey(subPath, writable: true))
                {
                    if (key == null) return;
                    key.DeleteValue(rt.ValueName, throwOnMissingValue: false);
                }
                return;
            }

            int lastSlash = subPath.LastIndexOf('\\');
            if (lastSlash < 0)
            {
                hive.DeleteSubKeyTree(subPath, throwOnMissingSubKey: false);
            }
            else
            {
                string parentPath = subPath.Substring(0, lastSlash);
                string keyName    = subPath.Substring(lastSlash + 1);
                using (RegistryKey parent = hive.OpenSubKey(parentPath, writable: true))
                {
                    if (parent == null) return;
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
        //  BACKUP REGISTRY (reg.exe export)
        // ─────────────────────────────────────────────────────────────────

        private void BackupSystemRegistry(string backupDir)
        {
            Directory.CreateDirectory(backupDir);
            Log($"[*] Backup vào: {backupDir}");

            foreach (string regPath in CleanupConfig.Instance.CriticalBackupPaths)
            {
                string safeName = regPath.Replace('\\', '_').Replace(':', '_') + ".reg";
                string outFile  = Path.Combine(backupDir, safeName);
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName  = "reg.exe",
                        Arguments = $"export \"{regPath}\" \"{outFile}\" /y",
                        UseShellExecute = false,
                        CreateNoWindow  = true,
                    };
                    using (var p = Process.Start(psi)) p.WaitForExit(15000);
                    Log($"[✔] Backup: {regPath}");
                }
                catch (Exception ex) { Log($"[!] Backup lỗi {regPath}: {ex.Message}"); }
            }

            try
            {
                var psi2 = new ProcessStartInfo
                {
                    FileName  = "reg.exe",
                    Arguments = $"export \"HKLM\\SOFTWARE\\Huorong\" \"{Path.Combine(backupDir, "Huorong_Software.reg")}\" /y",
                    UseShellExecute = false,
                    CreateNoWindow  = true,
                };
                using (var p = Process.Start(psi2)) p.WaitForExit(5000);
            }
            catch { }

            Log("[✔] Backup hoàn tất.");
        }

        // ─────────────────────────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────────────────────────

        private void AddRegItem(string keyDisplay, string value, string keyPath, string valueName)
        {
            var item = new ListViewItem(new[] { keyDisplay, value ?? "" });
            item.Tag     = new RegTarget { KeyPath = keyPath, ValueName = valueName };
            item.Checked = true;
            lstRegistry.Items.Add(item);
        }

        private bool IsAlreadyListed(string fullPath)
        {
            foreach (ListViewItem existing in lstRegistry.Items)
                if (existing.Tag is RegTarget rt &&
                    string.Equals(rt.KeyPath, fullPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private bool ContainsHuorongKeyword(string s)
        {
            if (s == null) return false;
            foreach (string kw in CleanupConfig.Instance.RegistryKeywords)
                if (s.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
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

        private class RegTarget
        {
            public string KeyPath;
            public string ValueName;
        }

        // ─────────────────────────────────────────────────────────────────
        //  BUTTONS
        // ─────────────────────────────────────────────────────────────────

        private void btnDeepScan_Click(object sender, EventArgs e)
        {
            using (Form3 f3 = new Form3())
                f3.ShowDialog(this);
        }

        private void pbDonate_Click(object sender, EventArgs e)
        {
            using (Form4 f4 = new Form4())
                f4.ShowDialog(this);
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        private void Log(string msg)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => Log(msg))); return; }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            txtLog.ScrollToCaret();
        }
    }
}
