# Huorong Security Uninstaller Tool

Công cụ gỡ cài đặt và dọn dẹp hoàn toàn phần mềm **Huorong Internet Security (火绒互联网安全软件)** khỏi hệ thống Windows, bao gồm toàn bộ file, thư mục, registry key và driver còn sót lại sau khi gỡ cài đặt.

## Tính năng

### Form 1 – Gỡ cài đặt tự động
- Tự động phát hiện Huorong qua registry (`HKLM\SOFTWARE\...\Uninstall\HuorongSysdiag`)
- Hỗ trợ cả registry 32-bit và 64-bit
- Khởi chạy trình gỡ cài đặt gốc của Huorong và **tự động click các nút xác nhận** (hỗ trợ cả tiếng Anh và tiếng Trung)
- Trình tự click: `Uninstall / 卸载` → `OK / 确定` → `Reboot later / 稍后重启`
- Sau khi gỡ xong, tự động lên lịch chạy dọn dẹp sau khi khởi động lại bằng **Task Scheduler** (dự phòng: `HKLM\RunOnce`)
- Ghi log thời gian thực vào giao diện

### Form 2 – Dọn dẹp file và registry
Tự động quét và xóa các thành phần còn sót lại sau khi gỡ cài đặt.

**File & thư mục được quét:**
- `C:\Program Files\Huorong`
- `C:\Program Files (x86)\Huorong`
- `C:\ProgramData\Huorong` / `HuorongSysdiag`
- `%AppData%\Local\Huorong` / `%AppData%\Roaming\Huorong`
- `C:\Windows\System32\drivers\sysdiag.sys`
- `C:\Windows\SysWOW64\drivers\sysdiag.sys`

**Registry keys được xóa:**
- `HKLM\SOFTWARE\Huorong` và `WOW6432Node`
- Uninstall entries
- Services: `sysdiag`, `HipsDaemon`, `HipsMain`, `HipsSvc`, `HipsFilter`, `HuorongAV`, ...
- Run/RunOnce values chứa từ khóa Huorong
- Legacy driver entries: `LEGACY_SYSDIAG`, `LEGACY_HRWFPDRV`, ...

**Tính năng an toàn:**
- **Backup registry tự động** trước khi xóa → lưu tại `Desktop\HIS_RegBackup_YYYYMMDD_HHmmss\`
- Bỏ chọn mặc định các mục nguy hiểm (REG_BINARY, AppCompatCache, LEGACY drivers)
- Màu sắc cảnh báo: cam = được bảo vệ bởi kernel, xanh = thư mục
- Hỗ trợ chọn từng mục trước khi xóa

### Form 3 – Quét registry sâu
- Dùng `reg.exe` để tìm kiếm toàn bộ registry theo từ khóa: `Huorong`, `火绒`
- Quét song song (`Parallel.ForEach`) trên: `HKLM\SOFTWARE`, `HKLM\SYSTEM`, `HKCU\SOFTWARE`
- Phát hiện cả binary data chứa chuỗi Huorong (ASCII và UTF-16LE)
- Tự động bỏ chọn các mục không an toàn để xóa

## Yêu cầu hệ thống

| Thành phần | Yêu cầu |
|---|---|
| Hệ điều hành | Windows 7 trở lên (khuyến nghị Windows 10/11) |
| .NET Framework | 4.8 |
| Quyền hạn | **Administrator** (bắt buộc) |
| Công cụ hệ thống | `schtasks.exe`, `reg.exe` (có sẵn trên Windows) |

## Cài đặt & sử dụng

### Chạy từ bản build
1. Tải file `Huorong Security Uninstaller Tool.exe` từ thư mục `bin\Release\`
2. Chuột phải → **Run as administrator**
3. Làm theo hướng dẫn trên giao diện

### Build từ source
```
1. Mở file frm_HIS_uninstaller_tool.slnx bằng Visual Studio 2019+
2. Chọn Build → Build Solution (hoặc Ctrl+Shift+B)
3. File exe xuất ra tại: bin\Release\Huorong Security Uninstaller Tool.exe
```

### Luồng hoạt động
```
Khởi động
    │
    ├── Tham số /afterreboot? ──→ YES ──→ Hiện Form2 (dọn dẹp), thoát
    │
    └── NO
         │
         ▼
    Form1: Phát hiện Huorong → Gỡ cài đặt tự động
         │
         ▼
    Lên lịch chạy lại sau reboot (Task Scheduler / RunOnce)
         │
         ▼
    [Khởi động lại máy]
         │
         ▼
    Form2: Quét file & registry → Người dùng xác nhận → Xóa
         │
         ▼ (tùy chọn)
    Form3: Quét sâu registry bằng reg.exe
```

## Cấu trúc project

```
frm_HIS_uninstaller_tool/
├── Form1.cs / Form1.Designer.cs   # Gỡ cài đặt tự động & lên lịch dọn dẹp
├── Form2.cs / Form2.Designer.cs   # Dọn dẹp file & registry sau reboot
├── Form3.cs / Form3.Designer.cs   # Quét registry sâu qua reg.exe
├── Program.cs                     # Entry point
├── Properties/
│   └── AssemblyInfo.cs            # Metadata, version 1.0.0.0
├── App.config                     # Cấu hình .NET runtime
├── app.manifest                   # UAC elevation (requireAdministrator)
├── images.ico                     # Icon ứng dụng
└── frm_HIS_uninstaller_tool.slnx  # Solution file
```

## Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Framework:** .NET Framework 4.8
- **UI:** Windows Forms
- **Win32 APIs:** `FindWindow`, `EnumChildWindows`, `SendMessage`, `GetWindowText`
- **Registry:** `Microsoft.Win32.Registry`, `RegistryKey`
- **WMI:** `System.Management` (truy vấn thông tin OS)
- **Đa luồng:** `Task.Run()`, `Parallel.ForEach`, `Invoke()` pattern

## Lưu ý

> **Cảnh báo:** Công cụ này thực hiện xóa registry và file hệ thống. Luôn backup registry trước khi dùng. Chức năng backup tự động được tích hợp sẵn trong Form2 và Form3.

> Công cụ chỉ nhắm đến các thành phần của Huorong Security. Không ảnh hưởng đến phần mềm khác.

## License

MIT License — Xem file [LICENSE](LICENSE) để biết thêm chi tiết.
