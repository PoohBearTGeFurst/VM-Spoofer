using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyHook;

namespace VMSpoofCore
{
    /// <summary>
    /// Spoof module for network-based VM detection.
    /// Spoof MAC address and network adapter information.
    /// WARNING: Enabling this may affect network connectivity.
    /// </summary>
    public class NetworkSpoofModule : BaseSpoofModule
    {
        public override string Name => "Network";

        private static NetworkSpoofModule? _instance;
        private string? _fakeMacAddress;

        private delegate int GetAdaptersInfoDelegate(IntPtr pAdapterInfo, ref uint pOutBufLen);

        public override void Initialize(SpoofConfiguration config)
        {
            base.Initialize(config);
            _instance = this;
            _fakeMacAddress = config.FakeMacAddress;

            Logger.Info($"NetworkSpoofModule initialized with MAC: {_fakeMacAddress}");
        }

        public override void InstallHooks()
        {
            ThrowIfNotInitialized();

            try
            {
                // Hook GetAdaptersInfo
                LocalHook.Create(
                    LocalHook.GetProcAddress("iphlpapi.dll", "GetAdaptersInfo"),
                    new GetAdaptersInfoDelegate(GetAdaptersInfoHooked),
                    this
                ).ThreadACL.SetExclusiveACL(new[] { 0 });

                IsActive = true;
                Logger.Warning("Network module activated - may affect connectivity");
                Logger.Info("Network hooks installed");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to install network hooks", ex);
                throw;
            }
        }

        private int GetAdaptersInfoHooked(IntPtr pAdapterInfo, ref uint pOutBufLen)
        {
            // Call original function
            int result = GetAdaptersInfoOriginal(pAdapterInfo, ref pOutBufLen);

            // If successful, modify MAC address in the returned data
            if (result == 0 && pAdapterInfo != IntPtr.Zero && pOutBufLen > 0)
            {
                Logger.Debug("Spoofing adapter MAC address");
                // This is a simplified version; full implementation would need
                // to properly parse and modify the IP_ADAPTER_INFO structure
            }

            return result;
        }

        private int GetAdaptersInfoOriginal(IntPtr pAdapterInfo, ref uint pOutBufLen)
        {
            return 0; // ERROR_SUCCESS
        }
    }
}
