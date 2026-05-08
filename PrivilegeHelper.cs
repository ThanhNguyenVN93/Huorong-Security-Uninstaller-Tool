using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace frm_HIS_uninstaller_tool
{
    internal static class PrivilegeHelper
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(
            IntPtr tokenHandle, bool disableAll,
            ref TOKEN_PRIVILEGES newState, uint len,
            IntPtr prevState, IntPtr retLen);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr h);

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID { public uint LowPart; public int HighPart; }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID_AND_ATTRIBUTES { public LUID Luid; public uint Attributes; }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privilege;
        }

        private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint TOKEN_QUERY             = 0x0008;
        private const uint SE_PRIVILEGE_ENABLED    = 0x0002;

        // Bật cả hai đặc quyền cần thiết để chiếm quyền sở hữu
        public static bool EnableTakeOwnership()
            => EnablePrivilege("SeTakeOwnershipPrivilege")
            && EnablePrivilege("SeRestorePrivilege");

        private static bool EnablePrivilege(string name)
        {
            if (!OpenProcessToken(GetCurrentProcess(),
                TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr token))
                return false;
            try
            {
                if (!LookupPrivilegeValue(null, name, out LUID luid)) return false;
                var tp = new TOKEN_PRIVILEGES
                {
                    PrivilegeCount = 1,
                    Privilege = new LUID_AND_ATTRIBUTES
                    {
                        Luid       = luid,
                        Attributes = SE_PRIVILEGE_ENABLED
                    }
                };
                AdjustTokenPrivileges(token, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                return Marshal.GetLastWin32Error() == 0;
            }
            finally { CloseHandle(token); }
        }

        /// <summary>Chiếm ownership + cấp FullControl cho Administrators trên file.</summary>
        public static bool TakeOwnershipOfFile(string path)
        {
            try
            {
                EnableTakeOwnership();
                var adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                // Bước 1: đặt owner = Administrators
                var ownerSec = new FileSecurity(path, AccessControlSections.Owner);
                ownerSec.SetOwner(adminSid);
                File.SetAccessControl(path, ownerSec);

                // Bước 2: cấp quyền FullControl
                var aceSec = new FileSecurity(path, AccessControlSections.Access);
                aceSec.AddAccessRule(new FileSystemAccessRule(
                    adminSid,
                    FileSystemRights.FullControl,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));
                File.SetAccessControl(path, aceSec);
                return true;
            }
            catch { return false; }
        }

        /// <summary>Chiếm ownership + cấp FullControl cho Administrators trên registry key trong HKLM.</summary>
        public static bool TakeOwnershipOfRegistryKey(string hklmSubPath)
        {
            try
            {
                EnableTakeOwnership();
                var adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                // Bước 1: mở với quyền TakeOwnership rồi đặt owner
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    hklmSubPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.TakeOwnership))
                {
                    if (key == null) return false;
                    var sec = key.GetAccessControl(AccessControlSections.Owner);
                    sec.SetOwner(adminSid);
                    key.SetAccessControl(sec);
                }

                // Bước 2: mở lại với ChangePermissions rồi cấp FullControl
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    hklmSubPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.ChangePermissions))
                {
                    if (key == null) return false;
                    var sec = key.GetAccessControl();
                    sec.AddAccessRule(new RegistryAccessRule(
                        adminSid,
                        RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow));
                    key.SetAccessControl(sec);
                }
                return true;
            }
            catch { return false; }
        }
    }
}
