using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyHook;

namespace VMSpoofCore
{
    /// <summary>
    /// Spoof module for file system-based VM detection.
    /// Hides VM installation directories and files.
    /// </summary>
    public class FileSystemSpoofModule : BaseSpoofModule
    {
        public override string Name => "FileSystem";

        private static FileSystemSpoofModule? _instance;
        private HashSet<string>? _vmPaths;

        private delegate bool FindFirstFileWDelegate(string lpFileName, out IntPtr lpFindFileData);
        private delegate bool GetFileAttributesWDelegate(string lpFileName);

        public override void Initialize(SpoofConfiguration config)
        {
            base.Initialize(config);
            _instance = this;

            // Pre-compute list of VM-related paths to hide
            _vmPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                @"C:\Program Files\VMware",
                @"C:\Program Files (x86)\VMware",
                @"C:\Program Files\Oracle\VirtualBox",
                @"C:\Program Files (x86)\Oracle\VirtualBox",
                @"C:\Program Files\Parallels",
                @"C:\Program Files (x86)\Parallels",
                @"C:\Program Files\Xen",
                @"C:\Program Files (x86)\Xen",
                @"C:\Program Files\KVM",
                @"C:\Program Files (x86)\KVM",
                @"C:\Program Files\Citrix",
                @"C:\Program Files (x86)\Citrix",
                @"C:\Windows\System32\drivers\vmmouse.sys",
                @"C:\Windows\System32\drivers\vmx_fb.sys",
                @"C:\Windows\System32\Drivers\vmrawdsk.sys",
                @"C:\Windows\System32\Drivers\vfsfilter.sys"
            };

            Logger.Info($"FileSystemSpoofModule initialized with {_vmPaths.Count} hidden paths");
        }

        public override void InstallHooks()
        {
            ThrowIfNotInitialized();

            try
            {
                // Hook FindFirstFileW
                LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "FindFirstFileW"),
                    new FindFirstFileWDelegate(FindFirstFileWHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                // Hook GetFileAttributesW
                LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "GetFileAttributesW"),
                    new GetFileAttributesWDelegate(GetFileAttributesWHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                IsActive = true;
                Logger.Info("FileSystem hooks installed");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to install filesystem hooks", ex);
                throw;
            }
        }

        private bool FindFirstFileWHooked(string lpFileName, out IntPtr lpFindFileData)
        {
            lpFindFileData = IntPtr.Zero;

            // Fast-path: check if path is VM-related
            if (!IsVmPath(lpFileName))
            {
                return FindFirstFileWOriginal(lpFileName, out lpFindFileData);
            }

            Logger.Debug($"Blocked file access: {lpFileName}");
            return false; // File not found
        }

        private bool GetFileAttributesWHooked(string lpFileName)
        {
            // Fast-path: check if path is VM-related
            if (!IsVmPath(lpFileName))
            {
                return GetFileAttributesWOriginal(lpFileName);
            }

            Logger.Debug($"Blocked attribute query: {lpFileName}");
            return false; // File not found (0xFFFFFFFF)
        }

        private bool IsVmPath(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            foreach (var vmPath in _vmPaths!)
            {
                if (filePath.StartsWith(vmPath, StringComparison.OrdinalIgnoreCase) ||
                    filePath.Contains(vmPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool FindFirstFileWOriginal(string lpFileName, out IntPtr lpFindFileData)
        {
            lpFindFileData = IntPtr.Zero;
            return false;
        }

        private bool GetFileAttributesWOriginal(string lpFileName)
        {
            return false;
        }
    }
}
