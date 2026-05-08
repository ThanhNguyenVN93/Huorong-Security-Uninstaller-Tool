using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frm_HIS_uninstaller_tool
{
    public partial class Form1 : Form
    {
        #region Win32

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder sb, int maxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private const uint BM_CLICK = 0x00F5;

        #endregion

        private static readonly string[] HuorongWindowTitles = {
            "Huorong Internet Security", "火绒互联网安全软件", "Huorong"
        };

        private static readonly string[][] ButtonSequenceTemplate = {
            new[] { "Uninstall", "卸载" },
            new[] { "OK", "确定" },
            new[] { "Reboot later", "稍后重启" }
        };

        private const string StartupValueName = "HuorongCleanupTool";

        // Timer thay thế async polling — chạy trên UI thread, không cần marshal
        private Timer _clickTimer;   // 500ms: auto-click dialog Huorong
        private Timer _checkTimer;   // 30s:   kiểm tra registry xem đã gỡ xong chưa

        private int _clickStep;
        private int _pollsWithoutWindow;
        private DateTime _lastClick;

        public Form1() => InitializeComponent();

        private void Form1_Load_1(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            bool afterReboot = Array.Exists(args,
                a => a.Equals("/afterreboot", StringComparison.OrdinalIgnoreCase));

            if (afterReboot)
            {
                this.Hide();
                using (Form2 f2 = new Form2())
                    f2.ShowDialog();
                Application.Exit();
                return;
            }

            ApplyLanguage();
            CheckHuorong();
            UpdateOSInfo();
        }

        private void ApplyLanguage()
        {
            var L = LangManager.L;
            this.Text              = L.Get("form1.title");
            groupBox1.Text         = L.Get("form1.group.uninstaller");
            groupBox2.Text         = L.Get("form1.group.sysinfo");
            groupBox3.Text         = L.Get("form1.group.progress");
            btnunistaller.Text     = L.Get("form1.btn.uninstall");
            lblstatus.Text         = L.Get("form1.status.idle");
        }

        // ── Tìm đường dẫn uninstaller ─────────────────────────────────────

        private void CheckHuorong()
        {
            string result = GetUninstallString(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag", use64: true)
                ?? GetUninstallString(
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag", use64: false);

            var L = LangManager.L;
            if (result != null)
            {
                label1.Text = L.Get("form1.label.found");
                txtunipath.Text = result;
            }
            else
            {
                label1.Text = L.Get("form1.label.notfound");
                txtunipath.Text = "Not Found";
                btnunistaller.Enabled = false;
            }
        }

        private static string GetUninstallString(string subKey, bool use64)
        {
            var view = use64 ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            using (RegistryKey key = hklm.OpenSubKey(subKey))
                return key?.GetValue("UninstallString")?.ToString();
        }

        // ── Thông tin hệ điều hành ────────────────────────────────────────

        public void UpdateOSInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Caption, OSArchitecture FROM Win32_OperatingSystem"))
                using (var info = searcher.Get())
                {
                    foreach (ManagementObject obj in info)
                    {
                        string osName = (obj["Caption"]?.ToString() ?? "Unknown OS")
                            .Replace("NT 5.1.2600", "XP").Replace("NT 5.2.3790", "Server 2003");
                        string osArch = (obj["OSArchitecture"]?.ToString() ?? "Unknown")
                            .Replace("32-bit", "x86").Replace("64-bit", "x64");
                        lblwin.Text = osName;
                        lblbit.Text = osArch;
                    }
                }
            }
            catch (Exception ex)
            {
                lblwin.Text = "Error!";
                lblbit.Text = ex.Message;
            }
        }

        // ── Nút gỡ cài đặt ───────────────────────────────────────────────

        private void btnunistaller_Click(object sender, EventArgs e)
        {
            var L = LangManager.L;
            string path = txtunipath.Text?.Trim();
            if (string.IsNullOrEmpty(path) || path == "Not Found")
            {
                MessageBox.Show(L.Get("form1.dlg.nopath"), L.Get("form1.dlg.nopath.title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnunistaller.Enabled = false;
            txtprocess.Clear();
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 25;
            lblstatus.Text = L.Get("form1.status.uninstalling");
            lblstatus.ForeColor = Color.DarkOrange;

            AppendLog("══════════════════════════════════════");
            AppendLog("  HIS Uninstaller Tool — bắt đầu");
            AppendLog("  " + path);
            AppendLog("══════════════════════════════════════");

            Process proc;
            try
            {
                proc = Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "/S",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                AppendLog("[!] Không thể khởi chạy: " + ex.Message);
                ResetUI();
                return;
            }

            if (proc == null) { AppendLog("[!] Process null."); ResetUI(); return; }

            AppendLog("[*] PID=" + proc.Id + " — đang giám sát...");

            _clickStep = 0;
            _pollsWithoutWindow = 0;
            _lastClick = DateTime.MinValue;

            // Timer 1: auto-click dialog mỗi 500ms (chạy trên UI thread)
            _clickTimer = new Timer { Interval = 500 };
            _clickTimer.Tick += OnClickTimerTick;
            _clickTimer.Start();

            // Timer 2: check registry mỗi 30s — tín hiệu hoàn tất đáng tin cậy
            _checkTimer = new Timer { Interval = 30000 };
            _checkTimer.Tick += OnCheckTimerTick;
            _checkTimer.Start();

            AppendLog("[*] Giám sát dialog... (kiểm tra mỗi 30s)");
        }

        // ── Timer 1: auto-click dialog Huorong ───────────────────────────

        private void OnClickTimerTick(object sender, EventArgs e)
        {
            if (_clickStep >= ButtonSequenceTemplate.Length) return;
            if ((DateTime.Now - _lastClick).TotalSeconds < 1.5) return;

            IntPtr hwnd = FindHuorongWindow(ref _pollsWithoutWindow);
            if (hwnd == IntPtr.Zero) return;

            for (int s = _clickStep; s < ButtonSequenceTemplate.Length; s++)
            {
                if (TryClickButton(hwnd, ButtonSequenceTemplate[s]))
                {
                    AppendLog("[✔] Đã click: \"" + ButtonSequenceTemplate[s][0] + "\"");
                    _lastClick = DateTime.Now;
                    _clickStep = s + 1;
                    break;
                }
            }
        }

        // ── Timer 2: kiểm tra registry mỗi 30s ───────────────────────────

        private void OnCheckTimerTick(object sender, EventArgs e)
        {
            string result = GetUninstallString(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag", use64: true)
                ?? GetUninstallString(
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\HuorongSysdiag", use64: false);

            if (result != null)
            {
                AppendLog("[*] Kiểm tra: Huorong vẫn còn trong registry, chờ tiếp...");
            }
            else
            {
                AppendLog("[*] Registry đã xóa → gỡ cài đặt hoàn tất.");
                OnUninstallComplete();
            }
        }

        // ── Hoàn tất ─────────────────────────────────────────────────────

        private void OnUninstallComplete()
        {
            StopTimers();

            var L = LangManager.L;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 100;
            lblstatus.Text = L.Get("form1.status.done");
            lblstatus.ForeColor = Color.Green;
            AppendLog("✔  Gỡ cài đặt hoàn tất.");

            SetStartupAfterReboot();
            AppendLog("[i] Công cụ dọn dẹp sẽ tự chạy sau khi reboot.");

            MessageBox.Show(L.Get("form1.dlg.complete"),
                L.Get("form1.dlg.complete.title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StopTimers()
        {
            _clickTimer?.Stop(); _clickTimer?.Dispose(); _clickTimer = null;
            _checkTimer?.Stop(); _checkTimer?.Dispose(); _checkTimer = null;
        }

        private void ResetUI()
        {
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 0;
            lblstatus.Text = LangManager.L.Get("form1.status.error");
            lblstatus.ForeColor = Color.Red;
            btnunistaller.Enabled = true;
        }

        // ── Tìm cửa sổ Huorong ───────────────────────────────────────────

        private IntPtr FindHuorongWindow(ref int pollsWithoutWindow)
        {
            foreach (string title in HuorongWindowTitles)
            {
                IntPtr hwnd = FindWindow(null, title);
                if (hwnd != IntPtr.Zero) { pollsWithoutWindow = 0; return hwnd; }
            }

            // Fallback scan mỗi 10 poll (~5s) để tránh GetProcesses() liên tục
            if (++pollsWithoutWindow % 10 == 0)
            {
                foreach (Process p in Process.GetProcesses())
                {
                    try
                    {
                        if (p.MainWindowHandle != IntPtr.Zero &&
                            p.MainWindowTitle.IndexOf("Huorong", StringComparison.OrdinalIgnoreCase) >= 0)
                        { pollsWithoutWindow = 0; return p.MainWindowHandle; }
                    }
                    catch { }
                }
            }
            return IntPtr.Zero;
        }

        private bool TryClickButton(IntPtr parent, string[] buttonTexts)
        {
            bool found = false;
            EnumChildWindows(parent, (hWnd, _) =>
            {
                if (!IsWindowVisible(hWnd) || !IsWindowEnabled(hWnd)) return true;
                var sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, 256);
                string text = sb.ToString().Trim();
                foreach (string target in buttonTexts)
                {
                    if (string.Equals(text, target, StringComparison.OrdinalIgnoreCase))
                    {
                        SendMessage(hWnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                        found = true;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
            return found;
        }

        // ── Tạo Scheduled Task + Fallback RunOnce ────────────────────────

        private void SetStartupAfterReboot()
        {
            string exePath = Application.ExecutablePath;
            string taskArgs =
                "/Create /F /RL HIGHEST /SC ONLOGON" +
                " /TN \"" + StartupValueName + "\"" +
                " /TR \"\\\"" + exePath + "\\\" /afterreboot\"";

            bool ok = RunSchtasks(taskArgs, out string err);
            AppendLog(ok
                ? "[*] Scheduled Task tạo thành công."
                : "[!] schtasks lỗi: " + err);

            if (!ok) FallbackRunOnce(exePath);
        }

        private static bool RunSchtasks(string arguments, out string errorOutput)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (Process p = Process.Start(psi))
            {
                p.StandardOutput.ReadToEnd();
                errorOutput = p.StandardError.ReadToEnd().Trim();
                p.WaitForExit();
                return p.ExitCode == 0;
            }
        }

        private void FallbackRunOnce(string exePath)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", writable: true))
                {
                    key?.SetValue(StartupValueName, "\"" + exePath + "\" /afterreboot");
                    AppendLog("[*] Fallback: HKLM RunOnce đã ghi.");
                }
            }
            catch (Exception ex) { AppendLog("[!] Fallback thất bại: " + ex.Message); }
        }

        // ── UI helpers ────────────────────────────────────────────────────

        private void AppendLog(string message)
        {
            string line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message + "\n";
            txtprocess.AppendText(line);
            txtprocess.ScrollToCaret();
        }

        private void SetStatus(string text, Color color)
        {
            lblstatus.Text = text;
            lblstatus.ForeColor = color;
        }
    }
}
