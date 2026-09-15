using System;
using System.Collections.Generic;
using System.Management;

namespace VMSpoofCore
{
    /// <summary>
    /// Spoof module for WMI-based VM detection.
    /// Intercepts WMI queries for system hardware and BIOS information.
    /// </summary>
    public class WmiSpoofModule : BaseSpoofModule
    {
        public override string Name => "WMI";

        private Dictionary<string, Dictionary<string, object>>? _spoofedWmiData;

        public override void Initialize(SpoofConfiguration config)
        {
            base.Initialize(config);

            // Pre-compute spoofed WMI data
            _spoofedWmiData = new Dictionary<string, Dictionary<string, object>>
            {
                ["Win32_ComputerSystemProduct"] = new Dictionary<string, object>
                {
                    ["Vendor"] = config.FakeManufacturer,
                    ["Name"] = config.FakeModel,
                    ["Version"] = "1.0",
                    ["IdentifyingNumber"] = config.FakeSerialNumber
                },
                ["Win32_BaseBoard"] = new Dictionary<string, object>
                {
                    ["Manufacturer"] = config.FakeManufacturer,
                    ["Product"] = config.FakeModel,
                    ["Version"] = "1.0",
                    ["SerialNumber"] = config.FakeSerialNumber
                },
                ["Win32_BIOS"] = new Dictionary<string, object>
                {
                    ["Manufacturer"] = config.FakeBiosManufacturer,
                    ["Name"] = config.FakeBiosVersion,
                    ["Version"] = config.FakeBiosVersion,
                    ["SMBIOSBIOSVersion"] = config.FakeBiosVersion
                },
                ["Win32_Processor"] = new Dictionary<string, object>
                {
                    ["Name"] = config.FakeProcessorName,
                    ["Manufacturer"] = "GenuineIntel",
                    ["Family"] = 6,
                    ["Model"] = 165
                },
                ["Win32_ComputerSystem"] = new Dictionary<string, object>
                {
                    ["Manufacturer"] = config.FakeManufacturer,
                    ["Model"] = config.FakeModel,
                    ["Name"] = config.FakeComputerName,
                    ["TotalPhysicalMemory"] = (long)config.FakeTotalMemoryMB * 1024 * 1024
                }
            };

            Logger.Info($"WmiSpoofModule initialized with {_spoofedWmiData.Count} spoofed WMI classes");
        }

        public override void InstallHooks()
        {
            ThrowIfNotInitialized();

            try
            {
                // WMI interception is complex and requires hooking at the ManagementObject level
                // This would typically be done through:
                // 1. Hooking COM interfaces for WMI
                // 2. Intercepting ManagementObject.Get() calls
                // 3. Returning spoofed data from _spoofedWmiData

                // For this implementation, we'll implement a passive WMI spoofer
                // that gets activated by the injector

                IsActive = true;
                Logger.Info("WMI module activated (passive mode)");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to install WMI hooks", ex);
                throw;
            }
        }

        public Dictionary<string, object>? GetSpoofedData(string wmiClass)
        {
            if (_spoofedWmiData?.TryGetValue(wmiClass, out var data) == true)
            {
                return new Dictionary<string, object>(data);
            }

            return null;
        }
    }
}
