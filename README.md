# Huorong Security Uninstaller Tool

Công cụ gỡ cài đặt và dọn dẹp hoàn toàn **Huorong Internet Security (火绒互联网安全软件)** khỏi hệ thống Windows — bao gồm file, thư mục, registry key, driver, DriverStore và mọi dấu vết còn sót lại.

> Ngôn ngữ giao diện: **Tiếng Anh** (mặc định) hoặc **Tiếng Việt** — chuyển đổi ngay trên Form1.

---

## Tính năng

### Form 1 — Gỡ cài đặt tự động

- Tự động phát hiện Huorong qua registry (`HKLM\...\Uninstall\HuorongSysdiag`), hỗ trợ cả 32-bit và 64-bit
- Khởi chạy trình gỡ cài đặt gốc và **tự động click các nút xác nhận** (EN + ZH):
  - `Uninstall / 卸载` → `OK / 确定` → `Reboot later / 稍后重启`
- Theo dõi quá trình qua 2 timer: click dialog (500 ms) + kiểm tra registry (30 s)
- Sau khi gỡ xong, tự lên lịch chạy dọn dẹp sau reboot qua **Task Scheduler** (dự phòng: `HKLM\RunOnce`)
- Ghi log thời gian thực

**Điều khiển bổ sung:**

| Thành phần | Chức năng |
|---|---|
| `btnLang` | Toggle EN ↔ VI — thay đổi ngay cả 4 form |
| `pbDonate` | Mở Form4 (Donate) |

---

### Form 2 — Dọn dẹp file & registry

Tự động quét sau khi reboot, cho phép người dùng chọn và xóa từng mục.

**File & thư mục:**

| Đường dẫn | Ghi chú |
|---|---|
| `%ProgramFiles%\Huorong` | Thư mục cài đặt chính |
| `%ProgramFiles(x86)%\Huorong` | Bản 32-bit |
| `%ProgramData%\Huorong` / `HuorongSysdiag` | Dữ liệu chương trình |
| `%LocalAppData%\Huorong` / `%AppData%\Huorong` | Dữ liệu người dùng |
| `%WINDIR%\System32\drivers\sysdiag.sys` | Driver kernel |
| `%WINDIR%\SysWOW64\drivers\sysdiag.sys` | Driver 32-bit |
| `DriverStore\FileRepository\*` | Quét tên thư mục + nội dung `.inf` |

**Registry:**

- `HKLM\SOFTWARE\Huorong` (và WOW6432Node)
- Uninstall entries, Services: `sysdiag`, `HipsDaemon`, `HipsMain`, `HipsSvc`, `HipsFilter`, `HuorongAV`, `HuorongAgent`
- Run / RunOnce values chứa từ khóa Huorong
- Legacy driver entries: `LEGACY_SYSDIAG`, `LEGACY_HRWFPDRV`, ...
- Data value scan: `BagMRU`, `TypedPaths`, `CrawlScopeManager`, `Session Manager`

**An toàn:**

- **Dừng service/driver** (`sc stop` + `sc delete`) trước khi xóa file
- **Backup registry tự động** → `Desktop\HIS_RegBackup_YYYYMMDD_HHmmss\`
- **TakeOwnership fallback** (`SeTakeOwnershipPrivilege` + `SeRestorePrivilege`) khi gặp file/key bị khóa
- Tự động khởi động lại **Windows Search (WSearch)** nếu đã xóa `CrawlScopeManager`
- Xuất báo cáo `.txt` tự động ra Desktop sau mỗi lần xóa

---

### Form 3 — Deep Scan (Quét registry sâu)

Tìm kiếm toàn diện toàn bộ registry theo từ khóa và data value.

- Quét song song (`Parallel.ForEach`) trên: `HKLM\SOFTWARE`, `HKLM\SYSTEM`, `HKCU\SOFTWARE`
- Phát hiện: string, expand string, multi-string, **REG_BINARY** (ASCII + UTF-16LE)
- Phân loại 3 mức độ rủi ro:

| Mức | Màu | Ví dụ |
|---|---|---|
| Safe (An toàn) | Xanh lá | Key Huorong bình thường |
| Warning (Cảnh báo) | Cam | Services, Run entries |
| Dangerous (Nguy hiểm) | Đỏ | `SYSTEM\CurrentControlSet\Services` |

- **Nút Stop** dừng scan ngay lập tức (cancel `reg.exe` bằng `CancellationToken`)
- **Registry Integrity Check** sau mỗi lần xóa — kiểm tra 6 key hệ thống quan trọng:
  - `HKLM\SYSTEM\CurrentControlSet\Control`
  - `HKLM\SYSTEM\CurrentControlSet\Services`
  - `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion`
  - `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion`
  - `HKLM\SOFTWARE\Classes\.exe`
  - `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer`
- Backup registry + xuất báo cáo `.txt` trước khi xóa

---

### Form 4 — Donate

Form nhỏ hiển thị 2 mã QR:

- **Techcombank (TCB)**
- **MoMo**

Truy cập từ icon `♥` ở góc phải tất cả các form (Form1 / Form2 / Form3).

---

## Đa ngôn ngữ

- Giao diện hỗ trợ **English** và **Tiếng Việt**
- Chuyển đổi bằng nút `EN | VI` trên Form1 — áp dụng ngay lập tức cho cả 4 form
- File ngôn ngữ: `lang_en.xml` và `lang_vi.xml` — **embedded trong exe**, không cần file ngoài
- Có thể override bằng file `lang_xx.xml` đặt cạnh exe (ưu tiên cao hơn)

---

## Tự chứa (Self-contained)

Toàn bộ tài nguyên được **embedded trực tiếp vào exe**:

| Resource | Loại |
|---|---|
| `donate.jpg` | Ảnh icon donate |
| `tcb.jpg` | QR Techcombank |
| `momo.jpg` | QR MoMo |
| `cleanup_config.xml` | Cấu hình danh sách quét |
| `lang_en.xml` | Chuỗi tiếng Anh |
| `lang_vi.xml` | Chuỗi tiếng Việt |

Chỉ cần **1 file `.exe`** duy nhất — chạy được trên mọi máy không cần cài thêm.  
File cấu hình/ngôn ngữ bên ngoài (nếu có) sẽ được ưu tiên (cho phép tùy chỉnh).

---

## Cấu hình ngoài (tùy chọn)

File `cleanup_config.xml` có thể đặt cạnh exe để override cài đặt:

```xml
<CleanupConfig>
  <Language>en</Language>          <!-- "en" hoặc "vi" -->
  <FileScanRoots>
    <Path>%PROGRAMFILES%\Huorong</Path>
    ...
  </FileScanRoots>
  <RegistryKeywords>
    <Keyword>Huorong</Keyword>
    <Keyword>火绒</Keyword>
    ...
  </RegistryKeywords>
  ...
</CleanupConfig>
```

---

## Yêu cầu hệ thống

| Thành phần | Yêu cầu |
|---|---|
| Hệ điều hành | Windows 7 trở lên (khuyến nghị Windows 10/11) |
| .NET Framework | 4.8 |
| Quyền hạn | **Administrator** (bắt buộc — khai báo trong `app.manifest`) |
| Công cụ hệ thống | `schtasks.exe`, `reg.exe`, `sc.exe` (có sẵn trên Windows) |

---

## Luồng hoạt động

```
Khởi động exe
    │
    ├── Tham số /afterreboot? ──→ YES ──→ Form2 (dọn dẹp), thoát
    │
    └── NO
         ▼
    Form1: Phát hiện Huorong trong registry
         │
         ▼
    Khởi chạy uninstaller + tự click dialog xác nhận
         │
         ▼
    Đăng ký Task Scheduler / RunOnce để chạy sau reboot
         │
         ▼
    [Người dùng reboot máy]
         │
         ▼
    Form2: Dừng service → Quét file & registry → Xóa → Báo cáo
         │
         └── (tùy chọn) Form3: Deep Scan toàn bộ registry
                              └── (tùy chọn) Form4: Donate ♥
```

---

## Cấu trúc project

```
frm_HIS_uninstaller_tool/
├── Form1.cs / .Designer.cs     Gỡ cài đặt + toggle ngôn ngữ + donate icon
├── Form2.cs / .Designer.cs     Dọn dẹp file & registry sau reboot
├── Form3.cs / .Designer.cs     Deep Scan registry
├── Form4.cs / .Designer.cs     Donate (QR TCB + MoMo)
├── CleanupConfig.cs            Singleton config — đọc XML hoặc embedded resource
├── CleanupLogger.cs            Ghi log + xuất báo cáo .txt ra Desktop
├── LangManager.cs              Singleton ngôn ngữ + toggle runtime EN/VI
├── PrivilegeHelper.cs          SeTakeOwnershipPrivilege / SeRestorePrivilege
├── ResourceHelper.cs           Load ảnh & XML từ embedded assembly resources
├── Program.cs                  Entry point
├── cleanup_config.xml          Cấu hình danh sách quét (embedded + override)
├── lang_en.xml                 Chuỗi tiếng Anh (embedded + override)
├── lang_vi.xml                 Chuỗi tiếng Việt (embedded + override)
├── donate.jpg / tcb.jpg / momo.jpg   Ảnh donate (embedded)
├── images.ico                  Icon ứng dụng
├── app.manifest                UAC requireAdministrator
└── Properties/
    └── AssemblyInfo.cs
```

---

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# / .NET Framework 4.8 |
| UI | Windows Forms |
| Tự động click | `FindWindow`, `EnumChildWindows`, `SendMessage` (P/Invoke) |
| Chiếm quyền | `AdjustTokenPrivileges`, `SeTakeOwnershipPrivilege` (P/Invoke) |
| Registry | `Microsoft.Win32.RegistryKey`, `FileSecurity`, `RegistryAccessRule` |
| WMI | `System.Management` (thông tin OS) |
| Đa luồng | `Task.Run()`, `Parallel.ForEach`, `CancellationToken` |
| Embedded resources | `Assembly.GetManifestResourceStream()` |
| Ngôn ngữ | `XmlDocument` + singleton `LangManager` |

---

## Build

```
Visual Studio 2019 trở lên:
  Ctrl+Shift+B  →  bin\Release\Huorong Security Uninstaller Tool.exe

MSBuild:
  MSBuild frm_HIS_uninstaller_tool.csproj /p:Configuration=Release
```

---

> **Cảnh báo:** Công cụ này xóa registry và file hệ thống. Backup tự động được tích hợp trong Form2 và Form3, nhưng hãy đảm bảo bạn hiểu những gì đang được xóa trước khi xác nhận.

## License

MIT License — Xem file [LICENSE](LICENSE) để biết thêm chi tiết.
