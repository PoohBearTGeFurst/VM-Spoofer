using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VMSpoofCore
{
    /// <summary>
    /// Configuration for VM spoofing.
    /// This class is immutable after deserialization for performance.
    /// </summary>
    public class SpoofConfiguration
    {
        [JsonPropertyName("spoofProfile")]
        public string SpoofProfile { get; set; } = "Generic";

        [JsonPropertyName("enabledModules")]
        public List<string> EnabledModules { get; set; } = new()
        {
            "Registry",
            "FileSystem",
            "Process",
            "WMI"
        };

        // Fake hardware information
        [JsonPropertyName("fakeManufacturer")]
        public string FakeManufacturer { get; set; } = "Dell Inc.";

        [JsonPropertyName("fakeModel")]
        public string FakeModel { get; set; } = "OptiPlex 7090";

        [JsonPropertyName("fakeSerialNumber")]
        public string FakeSerialNumber { get; set; } = "A1B2C3D4E5F6";

        [JsonPropertyName("fakeBiosManufacturer")]
        public string FakeBiosManufacturer { get; set; } = "American Megatrends Inc.";

        [JsonPropertyName("fakeBiosVersion")]
        public string FakeBiosVersion { get; set; } = "2.3.0";

        [JsonPropertyName("fakeComputerName")]
        public string FakeComputerName { get; set; } = "DESKTOP-USER";

        // System info
        [JsonPropertyName("fakeProcessorName")]
        public string FakeProcessorName { get; set; } = "Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz";

        [JsonPropertyName("fakeTotalMemoryMB")]
        public uint FakeTotalMemoryMB { get; set; } = 16384;

        // Network spoofing
        [JsonPropertyName("fakeMacAddress")]
        public string FakeMacAddress { get; set; } = "00:11:22:33:44:55";

        [JsonPropertyName("enableNetworkSpoof")]
        public bool EnableNetworkSpoof { get; set; } = false;

        /// <summary>Gets whether a specific module is enabled.</summary>
        public bool IsModuleEnabled(string moduleName)
        {
            return EnabledModules.Contains(moduleName);
        }
    }
}
