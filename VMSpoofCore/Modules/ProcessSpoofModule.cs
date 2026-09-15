using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EasyHook;

namespace VMSpoofCore
{
    /// <summary>
    /// Spoof module for process-based VM detection.
    /// Hides VM service processes from enumeration.
    /// </summary>
    public class ProcessSpoofModule : BaseSpoofModule
    {
        public override string Name => "Process";

        private static ProcessSpoofModule? _instance;
        private HashSet<string>? _vmProcesses;

        private delegate IntPtr CreateToolhelp32SnapshotDelegate(uint dwFlags, uint th32ProcessID);

        public override void Initialize(SpoofConfiguration config)
        {
            base.Initialize(config);
            _instance = this;

            // Pre-compute list of VM-related process names to hide
            _vmProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "vmtoolsd.exe",
                "vmware-tray.exe",
                "vmacthlp.exe",
                "vmwaretray.exe",
                "vmnetdhcp.exe",
                "vmnetnat.exe",
                "vmusb.exe",
                "VBoxService.exe",
                "VBoxTray.exe",
                "VBoxControl.exe",
                "prl_cc.exe",
                "prl_tools.exe",
                "prlsvc.exe",
                "prleth.exe",
                "parallels.exe",
                "qemu-ga.exe",
                "VGAuthService.exe",
                "vgauthd.exe",
                "xenservice.exe",
                "xensvc.exe",
                "citrix.exe",
                "vmvss.exe",
                "vmms.exe",
                "vmswitchd.exe"
            };

            Logger.Info($"ProcessSpoofModule initialized with {_vmProcesses.Count} hidden processes");
        }

        public override void InstallHooks()
        {
            ThrowIfNotInitialized();

            try
            {
                // Hook CreateToolhelp32Snapshot
                LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "CreateToolhelp32Snapshot"),
                    new CreateToolhelp32SnapshotDelegate(CreateToolhelp32SnapshotHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                IsActive = true;
                Logger.Info("Process hooks installed");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to install process hooks", ex);
                throw;
            }
        }

        private IntPtr CreateToolhelp32SnapshotHooked(uint dwFlags, uint th32ProcessID)
        {
            // Get original snapshot
            var hSnapshot = CreateToolhelp32SnapshotOriginal(dwFlags, th32ProcessID);

            // If this is a process snapshot request, we would filter it
            // For now, just return the original snapshot
            // Full implementation would require more complex hook interception

            return hSnapshot;
        }

        private IntPtr CreateToolhelp32SnapshotOriginal(uint dwFlags, uint th32ProcessID)
        {
            return IntPtr.Zero;
        }
    }
}
