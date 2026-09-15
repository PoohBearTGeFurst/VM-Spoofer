using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyHook;

namespace VMSpoofCore
{
    /// <summary>
    /// Spoof module for registry-based VM detection.
    /// Hooks registry API calls to hide VM indicators.
    /// </summary>
    public class RegistrySpoofModule : BaseSpoofModule
    {
        public override string Name => "Registry";

        private static RegistrySpoofModule? _instance;
        private Dictionary<string, object>? _spoofedValues;
        private delegate uint RegOpenKeyExDelegate(IntPtr hKey, string lpSubKey, uint ulOptions, uint samDesired, out IntPtr phkResult);
        private delegate uint RegQueryValueExDelegate(IntPtr hKey, string lpValueName, IntPtr lpReserved, out uint lpType, IntPtr lpData, ref uint lpcbData);

        public override void Initialize(SpoofConfiguration config)
        {
            base.Initialize(config);
            _instance = this;

            // Pre-compute spoofed registry values
            _spoofedValues = new Dictionary<string, object>
            {
                ["HKLM\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0\\ProcessorNameString"] = Configuration!.FakeProcessorName,
                ["HKLM\\HARDWARE\\DESCRIPTION\\System\\SystemBiosVersion"] = Configuration.FakeBiosVersion,
                ["HKLM\\HARDWARE\\DESCRIPTION\\System\\SystemManufacturer"] = Configuration.FakeManufacturer,
                ["HKLM\\SYSTEM\\CurrentControlSet\\Control\\SystemInformation\\ComputerName"] = Configuration.FakeComputerName,
            };

            Logger.Info($"RegistrySpoofModule initialized with {_spoofedValues.Count} spoofed values");
        }

        public override void InstallHooks()
        {
            ThrowIfNotInitialized();

            try
            {
                // Hook RegOpenKeyExW
                LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "RegOpenKeyExW"),
                    new RegOpenKeyExDelegate(RegOpenKeyExHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                // Hook RegQueryValueExW
                LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "RegQueryValueExW"),
                    new RegQueryValueExDelegate(RegQueryValueExHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                IsActive = true;
                Logger.Info("Registry hooks installed");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to install registry hooks", ex);
                throw;
            }
        }

        private uint RegOpenKeyExHooked(IntPtr hKey, string lpSubKey, uint ulOptions, uint samDesired, out IntPtr phkResult)
        {
            // Fast-path: pass through non-VM registry keys
            if (lpSubKey == null || !IsVmRelatedKey(lpSubKey))
            {
                return RegOpenKeyExOriginal(hKey, lpSubKey, ulOptions, samDesired, out phkResult);
            }

            Logger.Debug($"Intercepted registry key: {lpSubKey}");
            return RegOpenKeyExOriginal(hKey, lpSubKey, ulOptions, samDesired, out phkResult);
        }

        private uint RegQueryValueExHooked(IntPtr hKey, string lpValueName, IntPtr lpReserved, out uint lpType, IntPtr lpData, ref uint lpcbData)
        {
            // This would contain the actual spoofing logic
            // For now, pass through to original
            return RegQueryValueExOriginal(hKey, lpValueName, lpReserved, out lpType, lpData, ref lpcbData);
        }

        private bool IsVmRelatedKey(string keyPath)
        {
            string[] vmIndicators = new[]
            {
                "VMware", "VirtualBox", "Hyper-V", "Xen", "KVM",
                "QEMU", "Citrix", "Parallels", "VPC",
                "ProcessorNameString", "CentralProcessor",
                "SystemBiosVersion", "SystemManufacturer"
            };

            foreach (var indicator in vmIndicators)
            {
                if (keyPath.Contains(indicator, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private uint RegOpenKeyExOriginal(IntPtr hKey, string lpSubKey, uint ulOptions, uint samDesired, out IntPtr phkResult)
        {
            phkResult = IntPtr.Zero;
            return 0; // ERROR_SUCCESS
        }

        private uint RegQueryValueExOriginal(IntPtr hKey, string lpValueName, IntPtr lpReserved, out uint lpType, IntPtr lpData, ref uint lpcbData)
        {
            lpType = 0;
            return 0; // ERROR_SUCCESS
        }
    }
}
