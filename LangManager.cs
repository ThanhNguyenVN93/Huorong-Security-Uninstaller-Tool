using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace frm_HIS_uninstaller_tool
{
    /// <summary>
    /// Tải chuỗi ngôn ngữ từ lang_{code}.xml đặt cạnh exe.
    /// Ngôn ngữ mặc định: giá trị <Language> trong cleanup_config.xml (mặc định "vi").
    /// </summary>
    internal class LangManager
    {
        private readonly Dictionary<string, string> _strings
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static LangManager _instance;
        public static LangManager L => _instance ?? (_instance = Create());

        // Gọi khi Language trong config thay đổi
        public static void Reset() => _instance = null;

        private static LangManager Create()
        {
            string lang = CleanupConfig.Instance.Language;
            return TryLoadXml(lang) ?? TryLoadXml("vi") ?? BuiltInVietnamese();
        }

        private static LangManager TryLoadXml(string lang)
        {
            string dir  = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            string path = Path.Combine(dir, $"lang_{lang}.xml");
            if (!File.Exists(path)) return null;

            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
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

        /// <summary>
        /// Lấy chuỗi theo key, tuỳ chọn format với args.
        /// Nếu không tìm thấy key → trả về key itself (dễ debug).
        /// </summary>
        public string Get(string key, params object[] args)
        {
            if (!_strings.TryGetValue(key, out string val)) val = key;
            return args.Length > 0 ? string.Format(val, args) : val;
        }

        // ── Built-in Vietnamese fallback (không cần file XML) ─────────────
        private static LangManager BuiltInVietnamese()
        {
            var m = new LangManager();
            var d = m._strings;

            // Form1
            d["form1.title"]               = "HIS Uninstaller Tool";
            d["form1.group.uninstaller"]   = "Huorong Uninstaller";
            d["form1.group.sysinfo"]       = "Thông tin hệ thống";
            d["form1.group.progress"]      = "Tiến trình";
            d["form1.btn.uninstall"]       = "Gỡ cài đặt";
            d["form1.status.idle"]         = "Chờ...";
            d["form1.status.uninstalling"] = "Đang gỡ cài đặt...";
            d["form1.status.done"]         = "Hoàn tất!";
            d["form1.status.error"]        = "Lỗi";
            d["form1.label.found"]         = "Đã tìm thấy";
            d["form1.label.notfound"]      = "Không tồn tại trên hệ thống";
            d["form1.dlg.nopath"]          = "Không tìm thấy đường dẫn uninstaller.";
            d["form1.dlg.nopath.title"]    = "Lỗi";
            d["form1.dlg.complete"]        = "Gỡ cài đặt thành công!\n\nCông cụ dọn dẹp sẽ tự chạy sau khi bạn khởi động lại.\nVui lòng chọn Reboot trên màn hình Huorong.";
            d["form1.dlg.complete.title"]  = "Hoàn tất";

            // Form2
            d["form2.title"]               = "HIS Cleanup Tool";
            d["form2.maintitle"]           = "HIS Cleanup Tool";
            d["form2.subtitle"]            = "Quét và dọn dẹp file/registry còn sót sau khi gỡ Huorong";
            d["form2.hdr.files"]           = "📁  File / Thư mục còn sót";
            d["form2.hdr.registry"]        = "🔑  Registry còn sót";
            d["form2.warn.backup"]         = "⚠  Registry hệ thống sẽ được backup vào Desktop trước khi xóa";
            d["form2.btn.deletefile"]      = "🗑  Xóa mục đã chọn";
            d["form2.btn.deletereg"]       = "💾  Backup & Xóa đã chọn";
            d["form2.btn.deepscan"]        = "🔎  Deep Scan...";
            d["form2.btn.close"]           = "Đóng";
            d["form2.dlg.nosel"]           = "Chưa chọn mục nào.";
            d["form2.dlg.confirmfile"]     = "Sẽ dừng service Huorong, sau đó xóa {0} mục.\nKhông thể khôi phục!\nTiếp tục?";
            d["form2.dlg.confirmfile.title"] = "Xác nhận xóa";
            d["form2.dlg.confirmreg"]      = "Sẽ backup registry hệ thống trước, rồi xóa {0} mục.\nTiếp tục?";
            d["form2.dlg.confirmreg.title"] = "Xác nhận";
            d["form2.dlg.done"]            = "Đã backup vào:\n{0}\n\nXóa: {1} thành công, {2} thất bại.\n\nBáo cáo đã lưu tại:\n{3}";
            d["form2.dlg.done.title"]      = "Hoàn tất";
            d["form2.dlg.wsearch"]         = "Đã xóa mục Windows Search — dịch vụ WSearch sẽ được khởi động lại để cập nhật chỉ mục.";
            d["form2.dlg.wsearch.title"]   = "Windows Search";

            // Form3
            d["form3.title"]               = "Deep Scan — Tìm data value chứa Huorong";
            d["form3.hdr.title"]           = "🔎  Deep Scan — Tìm trong data value";
            d["form3.subtitle"]            = "Quét \"Huorong\" / \"火绒\" trong data của mọi value thuộc HKLM\\SOFTWARE, HKLM\\SYSTEM, HKCU";
            d["form3.btn.scan"]            = "🔍  Bắt đầu quét";
            d["form3.btn.stop"]            = "■ Dừng";
            d["form3.btn.checkall"]        = "✔ Chọn tất cả";
            d["form3.btn.uncheckall"]      = "✘ Bỏ chọn tất";
            d["form3.btn.delete"]          = "💾  Backup & Xóa đã chọn";
            d["form3.btn.close"]           = "Đóng";
            d["form3.col.risk"]            = "Mức độ";
            d["form3.col.key"]             = "Registry Key";
            d["form3.col.name"]            = "Value Name";
            d["form3.col.type"]            = "Type";
            d["form3.col.data"]            = "Data (chứa Huorong)";
            d["form3.risk.safe"]           = "An toàn";
            d["form3.risk.warning"]        = "Cảnh báo";
            d["form3.risk.dangerous"]      = "Nguy hiểm";
            d["form3.scan.notrun"]         = "Chưa quét";
            d["form3.scan.running"]        = "Đang quét...";
            d["form3.scan.found"]          = "{0} mục tìm thấy";
            d["form3.dlg.nosel"]           = "Chưa chọn mục nào.";
            d["form3.dlg.confirm"]         = "Sẽ backup registry liên quan trước, rồi xóa {0} data value:\n\n  • An toàn   : {1}\n  • Cảnh báo  : {2}\n  • Nguy hiểm : {3}\n\nTiếp tục?";
            d["form3.dlg.confirm.title"]   = "Xác nhận Deep Delete";
            d["form3.dlg.done"]            = "Xóa: {0} thành công, {1} thất bại.\n\nBáo cáo đã lưu tại:\n{2}";
            d["form3.dlg.done.title"]      = "Hoàn tất";
            d["form3.integrity.ok"]        = "✅ Registry toàn vẹn — không phát hiện vấn đề.";
            d["form3.integrity.warn"]      = "⚠ Một số key quan trọng bị thiếu — khuyến nghị chạy System Restore.";

            return m;
        }
    }
}
