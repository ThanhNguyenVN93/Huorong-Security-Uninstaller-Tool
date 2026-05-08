using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace frm_HIS_uninstaller_tool
{
    internal class LangManager
    {
        private readonly Dictionary<string, string> _strings
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static LangManager _instance;
        public static LangManager L => _instance ?? (_instance = Create());

        // Set by Form1's language toggle; persists for the session
        public static string OverrideLanguage { get; set; }

        public static void Reset() => _instance = null;

        private static LangManager Create()
        {
            string lang = OverrideLanguage ?? CleanupConfig.Instance.Language;
            string alt  = lang == "en" ? "vi" : "en";
            return TryLoadXml(lang) ?? TryLoadXml(alt) ?? BuiltInEnglish();
        }

        private static LangManager TryLoadXml(string lang)
        {
            XmlDocument doc = null;

            // External file overrides embedded resource
            string dir  = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            string path = Path.Combine(dir, $"lang_{lang}.xml");
            if (File.Exists(path))
            {
                try { var d = new XmlDocument(); d.Load(path); doc = d; }
                catch { }
            }

            // Embedded resource fallback
            if (doc == null)
                doc = ResourceHelper.LoadXml($"lang_{lang}.xml");

            if (doc == null) return null;
            try
            {
                var mgr = new LangManager();
                foreach (XmlNode n in doc.SelectNodes("//string"))
                {
                    string key = n.Attributes?["key"]?.Value;
                    if (!string.IsNullOrEmpty(key))
                        mgr._strings[key] = n.InnerText;
                }
                return mgr;
            }
            catch { return null; }
        }

        public string Get(string key, params object[] args)
        {
            if (!_strings.TryGetValue(key, out string val)) val = key;
            return args.Length > 0 ? string.Format(val, args) : val;
        }

        private static LangManager BuiltInEnglish()
        {
            var m = new LangManager();
            var d = m._strings;

            // Form1
            d["form1.title"]               = "HIS Uninstaller Tool";
            d["form1.group.uninstaller"]   = "Huorong Uninstaller";
            d["form1.group.sysinfo"]       = "System Information";
            d["form1.group.progress"]      = "Progress";
            d["form1.btn.uninstall"]       = "Uninstall";
            d["form1.status.idle"]         = "Waiting...";
            d["form1.status.uninstalling"] = "Uninstalling...";
            d["form1.status.done"]         = "Complete!";
            d["form1.status.error"]        = "Error";
            d["form1.label.found"]         = "Found";
            d["form1.label.notfound"]      = "Not installed on this system";
            d["form1.dlg.nopath"]          = "Uninstaller path not found.";
            d["form1.dlg.nopath.title"]    = "Error";
            d["form1.dlg.complete"]        = "Uninstallation successful!\n\nThe cleanup tool will run automatically after you restart.\nPlease select Reboot on the Huorong screen.";
            d["form1.dlg.complete.title"]  = "Complete";
            d["form1.tooltip.donate"]      = "Donate";

            // Form2
            d["form2.title"]               = "HIS Cleanup Tool";
            d["form2.maintitle"]           = "HIS Cleanup Tool";
            d["form2.subtitle"]            = "Scan and remove leftover files/registry after uninstalling Huorong";
            d["form2.hdr.files"]           = "📁  Leftover Files / Folders";
            d["form2.hdr.registry"]        = "🔑  Leftover Registry Entries";
            d["form2.warn.backup"]         = "⚠  System registry will be backed up to Desktop before deletion";
            d["form2.btn.deletefile"]      = "🗑  Delete Selected";
            d["form2.btn.deletereg"]       = "💾  Backup & Delete Selected";
            d["form2.btn.deepscan"]        = "🔎  Deep Scan...";
            d["form2.btn.close"]           = "Close";
            d["form2.dlg.nosel"]           = "No items selected.";
            d["form2.dlg.confirmfile"]     = "Huorong services will be stopped, then {0} items will be deleted.\nThis cannot be undone!\nContinue?";
            d["form2.dlg.confirmfile.title"] = "Confirm Delete";
            d["form2.dlg.confirmreg"]      = "System registry will be backed up, then {0} items will be deleted.\nContinue?";
            d["form2.dlg.confirmreg.title"] = "Confirm";
            d["form2.dlg.done"]            = "Backed up to:\n{0}\n\nDeleted: {1} successful, {2} failed.\n\nReport saved to:\n{3}";
            d["form2.dlg.done.title"]      = "Complete";
            d["form2.dlg.wsearch"]         = "Windows Search entries were removed — WSearch service will be restarted to rebuild the index.";
            d["form2.dlg.wsearch.title"]   = "Windows Search";

            // Form3
            d["form3.title"]               = "Deep Scan — Find data values containing Huorong";
            d["form3.hdr.title"]           = "🔎  Deep Scan — Search in data values";
            d["form3.subtitle"]            = "Search \"Huorong\" / \"火绒\" in all values under HKLM\\SOFTWARE, HKLM\\SYSTEM, HKCU";
            d["form3.btn.scan"]            = "🔍  Start Scan";
            d["form3.btn.stop"]            = "■ Stop";
            d["form3.btn.checkall"]        = "✔ Check All";
            d["form3.btn.uncheckall"]      = "✘ Uncheck All";
            d["form3.btn.delete"]          = "💾  Backup & Delete Selected";
            d["form3.btn.close"]           = "Close";
            d["form3.col.risk"]            = "Risk";
            d["form3.col.key"]             = "Registry Key";
            d["form3.col.name"]            = "Value Name";
            d["form3.col.type"]            = "Type";
            d["form3.col.data"]            = "Data (contains Huorong)";
            d["form3.risk.safe"]           = "Safe";
            d["form3.risk.warning"]        = "Warning";
            d["form3.risk.dangerous"]      = "Dangerous";
            d["form3.scan.notrun"]         = "Not scanned";
            d["form3.scan.running"]        = "Scanning...";
            d["form3.scan.found"]          = "{0} items found";
            d["form3.dlg.nosel"]           = "No items selected.";
            d["form3.dlg.confirm"]         = "Relevant registry keys will be backed up, then {0} data values will be deleted:\n\n  • Safe      : {1}\n  • Warning   : {2}\n  • Dangerous : {3}\n\nContinue?";
            d["form3.dlg.confirm.title"]   = "Confirm Deep Delete";
            d["form3.dlg.done"]            = "Deleted: {0} successful, {1} failed.\n\nReport saved to:\n{2}";
            d["form3.dlg.done.title"]      = "Complete";
            d["form3.integrity.ok"]        = "✅ Registry integrity verified — no issues detected.";
            d["form3.integrity.warn"]      = "⚠ Some critical keys are missing — System Restore is recommended.";

            // Form4
            d["form4.title"]               = "Donate";
            d["form4.maintitle"]           = "Donate";
            d["form4.subtitle"]            = "Support the developer";
            d["form4.label.tcb"]           = "Techcombank (TCB)";
            d["form4.label.momo"]          = "MoMo";
            d["form4.btn.close"]           = "Close";

            return m;
        }
    }
}
