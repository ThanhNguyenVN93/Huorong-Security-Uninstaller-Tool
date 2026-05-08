using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace frm_HIS_uninstaller_tool
{
    /// <summary>
    /// Tải cấu hình từ cleanup_config.xml đặt cạnh exe.
    /// Nếu file không tồn tại hoặc lỗi XML → dùng giá trị mặc định tích hợp sẵn.
    /// </summary>
    internal class CleanupConfig
    {
        // ── Danh sách cấu hình ─────────────────────────────────────────────
        public string[] FileScanRoots        { get; private set; }
        public string[] DriverStoreRoots     { get; private set; }
        public string[] RegistryKeywords     { get; private set; }
        public string[] HuorongServices      { get; private set; }
        public string[] FixedScanPaths       { get; private set; }
        public string[] LegacyDriverKeywords { get; private set; }
        public string[] DynamicServiceRoots  { get; private set; }
        public string[] EnumRootPaths        { get; private set; }
        public string[] DataScanRoots        { get; private set; }
        public string[] CriticalBackupPaths  { get; private set; }
        public string[] DeepScanScopes       { get; private set; }
        public string[] DeepScanTerms        { get; private set; }

        // ── Singleton ──────────────────────────────────────────────────────
        private static CleanupConfig _instance;
        public static CleanupConfig Instance => _instance ?? (_instance = Load());

        private static CleanupConfig Load()
        {
            string dir = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            string configPath = Path.Combine(dir, "cleanup_config.xml");

            if (File.Exists(configPath))
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(configPath);
                    return Parse(doc);
                }
                catch { /* XML lỗi → dùng default */ }
            }
            return Default();
        }

        private static CleanupConfig Parse(XmlDocument doc) => new CleanupConfig
        {
            FileScanRoots        = GetTexts(doc, "//FileScanRoots/Path"),
            DriverStoreRoots     = GetTexts(doc, "//DriverStoreRoots/Path"),
            RegistryKeywords     = GetTexts(doc, "//RegistryKeywords/Keyword"),
            HuorongServices      = GetTexts(doc, "//HuorongServices/Service"),
            FixedScanPaths       = GetTexts(doc, "//FixedScanPaths/Path"),
            LegacyDriverKeywords = GetTexts(doc, "//LegacyDriverKeywords/Keyword"),
            DynamicServiceRoots  = GetTexts(doc, "//DynamicServiceRoots/Path"),
            EnumRootPaths        = GetTexts(doc, "//EnumRootPaths/Path"),
            DataScanRoots        = GetTexts(doc, "//DataScanRoots/Path"),
            CriticalBackupPaths  = GetTexts(doc, "//CriticalBackupPaths/Path"),
            DeepScanScopes       = GetTexts(doc, "//DeepScanScopes/Scope"),
            DeepScanTerms        = GetTexts(doc, "//DeepScanTerms/Term"),
        };

        // Đọc text nodes, expand biến môi trường (%LOCALAPPDATA% v.v.)
        private static string[] GetTexts(XmlDocument doc, string xpath)
        {
            var nodes = doc.SelectNodes(xpath);
            if (nodes == null) return new string[0];
            var list = new List<string>();
            foreach (XmlNode n in nodes)
            {
                string s = n.InnerText?.Trim();
                if (!string.IsNullOrEmpty(s))
                    list.Add(Environment.ExpandEnvironmentVariables(s));
            }
            return list.ToArray();
        }

        // ── Giá trị mặc định ──────────────────────────────────────────────
        private static CleanupConfig Default()
        {
            string localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string roaming  = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            return new CleanupConfig
            {
                FileScanRoots = new[]
                {
                    @"C:\Program Files\Huorong",
                    @"C:\Program Files (x86)\Huorong",
                    @"C:\ProgramData\Huorong",
                    @"C:\ProgramData\HuorongSysdiag",
                    Path.Combine(localApp, "Huorong"),
                    Path.Combine(roaming,  "Huorong"),
                    @"C:\Windows\System32\drivers\sysdiag.sys",
                    @"C:\Windows\SysWOW64\drivers\sysdiag.sys",
                },

                DriverStoreRoots = new[]
                {
                    @"C:\Windows\System32\DriverStore\FileRepository",
                    @"C:\Windows\SysWOW64\DriverStore\FileRepository",
                },

                RegistryKeywords = new[]
                {
                    "Huorong", "HuorongSysdiag", "Sysdiag", "HipsDaemon",
                    "HipsMain", "HipsSvc", "HipsFilter", "HuorongAV", "火绒",
                },

                HuorongServices = new[]
                {
                    "sysdiag", "HipsDaemon", "HipsMain",
                    "HipsSvc", "HipsFilter", "HuorongAV", "HuorongAgent",
                },

                FixedScanPaths = new[]
                {
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
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
                },

                LegacyDriverKeywords = new[]
                {
                    "LEGACY_SYSDIAG", "LEGACY_HRWFPDRV", "LEGACY_HIPSDAEMON",
                    "LEGACY_HIPSMAIN", "LEGACY_HIPSSVC", "LEGACY_HIPSFILTER",
                },

                DynamicServiceRoots = new[]
                {
                    @"SYSTEM\CurrentControlSet\Services",
                    @"SYSTEM\ControlSet001\Services",
                    @"SYSTEM\ControlSet002\Services",
                },

                EnumRootPaths = new[]
                {
                    @"SYSTEM\CurrentControlSet\Enum\Root",
                    @"SYSTEM\ControlSet001\Enum\Root",
                    @"SYSTEM\ControlSet002\Enum\Root",
                },

                DataScanRoots = new[]
                {
                    @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
                    @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths",
                    @"HKLM\SOFTWARE\Microsoft\Windows Search\CrawlScopeManager",
                    @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager",
                    @"HKLM\SYSTEM\ControlSet001\Control\Session Manager",
                    @"HKLM\SYSTEM\ControlSet002\Control\Session Manager",
                },

                CriticalBackupPaths = new[]
                {
                    @"HKLM\SYSTEM\CurrentControlSet\Services",
                    @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                },

                DeepScanScopes = new[]
                {
                    @"HKLM\SOFTWARE",
                    @"HKLM\SYSTEM",
                    @"HKCU\SOFTWARE",
                },

                DeepScanTerms = new[] { "Huorong", "火绒" },
            };
        }
    }
}
