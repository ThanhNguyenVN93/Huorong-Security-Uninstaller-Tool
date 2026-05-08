using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace frm_HIS_uninstaller_tool
{
    /// <summary>Thu thập log trong một phiên xóa và xuất báo cáo .txt ra Desktop.</summary>
    internal class CleanupLogger
    {
        private readonly List<(DateTime ts, string msg)> _entries
            = new List<(DateTime, string)>();

        public void Record(string message) =>
            _entries.Add((DateTime.Now, message));

        /// <summary>Xuất báo cáo và trả về đường dẫn file đã tạo.</summary>
        public string ExportToDesktop(string operationTitle)
        {
            string desktop  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"HIS_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string path     = Path.Combine(desktop, fileName);

            int deleted = 0, failed = 0;
            foreach (var (_, msg) in _entries)
            {
                if (msg.StartsWith("[✔]")) deleted++;
                else if (msg.StartsWith("[✘]")) failed++;
            }

            var sb = new StringBuilder();
            sb.AppendLine("══════════════════════════════════════════════════");
            sb.AppendLine("  Huorong Security Uninstaller Tool — Báo cáo");
            sb.AppendLine($"  Thao tác : {operationTitle}");
            sb.AppendLine($"  Thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("══════════════════════════════════════════════════");
            sb.AppendLine();
            foreach (var (ts, msg) in _entries)
                sb.AppendLine($"[{ts:HH:mm:ss}] {msg}");
            sb.AppendLine();
            sb.AppendLine("── Tóm tắt ───────────────────────────────────────");
            sb.AppendLine($"  Thành công : {deleted}");
            sb.AppendLine($"  Thất bại   : {failed}");
            sb.AppendLine($"  Tổng cộng  : {_entries.Count}");
            if (failed > 0)
                sb.AppendLine("  Lưu ý     : Mục thất bại có thể cần reboot để xóa sạch.");
            sb.AppendLine("══════════════════════════════════════════════════");

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            return path;
        }
    }
}
