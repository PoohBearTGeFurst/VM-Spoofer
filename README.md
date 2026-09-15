# VM-Spoofer

A Windows 11 application that tricks legacy malware into thinking it's running on bare metal by spoofing VM detection vectors.

## Overview

VM-Spoofer intercepts and spoofs the detection methods used by legacy malware and viruses to identify virtual machines. By making the environment appear as a normal desktop computer, you can safely analyze and test malware in a controlled VM environment without triggering anti-analysis behaviors.

### Key Features

✅ **Easy-to-Use WPF UI** - Simple graphical interface for process selection and module control
✅ **Selective Injection** - Only hooks target process, doesn't affect system-wide behavior
✅ **Multiple Detection Vectors** - Spoof Registry, FileSystem, Processes, WMI, and Network adapters
✅ **Individual Module Control** - Enable/disable specific spoofing modules per session
✅ **Zero System Impact** - Hooks auto-remove when process exits
✅ **Performance Optimized** - Minimal overhead with fast-path passthrough for non-spoofed calls
✅ **Configuration Profiles** - Support for pre-configured spoof profiles
✅ **Command-Line Interface** - CLI injector for automated testing

## System Requirements

- **OS**: Windows 11 (64-bit)
- **.NET**: .NET 6.0 SDK or Runtime
- **Architecture**: x64
- **Privileges**: Administrator rights required

## Installation

### Option 1: Automated Setup (Recommended)

**For Windows (CMD):**
```bash
cd VM-Spoofer
setup.bat
```

**For PowerShell:**
```powershell
cd VM-Spoofer
powershell -ExecutionPolicy Bypass -File setup.ps1
```

**For WSL/Linux:**
```bash
cd VM-Spoofer
bash setup.sh
```

### Option 2: Manual Build

1. Install [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Clone the repository
3. Open `VM-Spoofer.sln` in Visual Studio or run:
   ```bash
   dotnet build "VM-Spoofer.sln" -c Release
   ```
4. Executables will be in `Output/` folder

## Usage

### WPF Launcher (GUI)

1. Run **VMSpoofLauncher.exe** as Administrator
2. Select target process from the dropdown list
3. Enable/disable spoof modules as needed:
   - **Registry Spoofing** - Spoof hardware info in registry (default: ON)
   - **File System Spoofing** - Hide VM tool directories (default: ON)
   - **Process Spoofing** - Hide VM service processes (default: ON)
   - **WMI Spoofing** - Spoof WMI queries (default: ON)
   - **Network Spoofing** - Spoof MAC address (default: OFF - may affect connectivity)
4. Click **START** to begin spoofing
5. Click **STOP** to stop and remove all hooks

![UI Screenshot](docs/ui-preview.md)

### Command-Line Injector

```bash
# Basic usage
VMSpoofInjector.exe --pid 1234

# With custom configuration
VMSpoofInjector.exe --pid 1234 --config vmware.json

# Shorthand
VMSpoofInjector.exe -p 1234 -c vmware.json
```

## Configuration

Create a JSON configuration file to customize spoofing behavior:

```json
{
  "spoofProfile": "VMware",
  "enabledModules": ["Registry", "FileSystem", "Process", "WMI"],
  "fakeManufacturer": "Dell Inc.",
  "fakeModel": "OptiPlex 7090",
  "fakeSerialNumber": "ABC123XYZ",
  "fakeBiosManufacturer": "American Megatrends Inc.",
  "fakeBiosVersion": "2.3.0",
  "fakeComputerName": "DESKTOP-USER",
  "fakeProcessorName": "Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz",
  "fakeTotalMemoryMB": 16384,
  "enableNetworkSpoof": false
}
```

### Configuration Profiles

#### Generic Bare Metal
```json
{
  "spoofProfile": "Generic",
  "enabledModules": ["Registry", "FileSystem", "Process"]
}
```

#### VMware Spoof
```json
{
  "spoofProfile": "VMware",
  "fakeManufacturer": "Dell Inc.",
  "fakeModel": "OptiPlex 7090"
}
```

#### VirtualBox Spoof
```json
{
  "spoofProfile": "VirtualBox",
  "fakeManufacturer": "Lenovo",
  "fakeModel": "ThinkCentre M90"
}
```

## Architecture

### Projects

- **VMSpoofCore** - Core spoofing engine and modules
  - ISpoofModule interface
  - SpoofEngine orchestrator
  - Individual spoof modules
  - Logger and utilities

- **VMSpoofLauncher** - WPF graphical interface
  - Process selection and monitoring
  - Module enable/disable controls
  - Real-time status display

- **VMSpoofInjector** - Command-line injector
  - Process targeting
  - Configuration loading
  - Hook installation and management

### Spoof Modules

#### Registry Module
- **Detection Vector**: Registry queries for VM indicators
- **Spoofs**: Processor name, BIOS version, system manufacturer
- **Target Keys**:
  - HKLM\\HARDWARE\\DESCRIPTION\\System
  - HKLM\\SYSTEM\\CurrentControlSet\\Control
  - System manufacturer/model information

#### FileSystem Module
- **Detection Vector**: File existence checks
- **Spoofs**: VM installation directories and tool executables
- **Hidden Paths**:
  - C:\\Program Files\\VMware
  - C:\\Program Files\\Oracle\\VirtualBox
  - C:\\Program Files\\Parallels
  - C:\\Program Files\\Xen
  - VM driver files (vmmouse.sys, vmx_fb.sys, etc.)

#### Process Module
- **Detection Vector**: Process enumeration
- **Spoofs**: VM service processes from listing
- **Hidden Processes**:
  - vmtoolsd.exe, vmware-tray.exe
  - VBoxService.exe, VBoxTray.exe
  - prl_cc.exe, prl_tools.exe
  - qemu-ga.exe, xensvc.exe

#### WMI Module
- **Detection Vector**: WMI class queries
- **Spoofs**: Hardware information via WMI
- **Spoofed Classes**:
  - Win32_ComputerSystemProduct
  - Win32_BaseBoard
  - Win32_BIOS
  - Win32_Processor
  - Win32_ComputerSystem

#### Network Module (Optional)
- **Detection Vector**: Network adapter enumeration
- **Spoofs**: MAC address and adapter info
- **WARNING**: May affect network connectivity if enabled

## How It Works

1. **Injection**: When you select a target process and click START, VMSpoofLauncher uses EasyHook to inject hooks into the target process only
2. **Hook Installation**: Individual spoof modules install API hooks for their respective detection vectors
3. **Interception**: When malware queries VM indicators, hooks intercept these calls
4. **Spoofing**: Spoofed values are returned from pre-computed cache (zero-allocation)
5. **Fast-path**: Non-VM-related calls pass through immediately with minimal overhead
6. **Cleanup**: When target process exits or you click STOP, all hooks are automatically uninstalled

## Performance Impact

- **Hook Install Time**: <100ms
- **Per-Call Overhead**: <0.1ms for spoofed calls
- **Memory Footprint**: ~5-10MB for injected DLL
- **System Impact**: Zero (isolated to target process)
- **Browser/Gaming**: Unaffected (separate processes)

## Troubleshooting

### "Administrator required" error
- Run as Administrator (right-click → Run as administrator)

### ".NET 6 SDK not found"
- Install from https://dotnet.microsoft.com/download/dotnet/6.0
- Verify installation: `dotnet --version`

### Hooks not installing
- Ensure target process is running when you click START
- Check that process has write access to its memory
- Try with a different target process

### Malware still detecting VM
- Not all malware uses these detection vectors
- Newer malware may use additional behavioral or hardware checks
- Enable all modules (Registry, FileSystem, Process, WMI)
- Consider what VM environment you're spoofing as

### Network connectivity issues
- If Network Spoofing is enabled, disable it
- Network module may interfere with adapter enumeration

## Logs

Logs are written to:
```
%APPDATA%\VMSpoofer\spoofer.log
```

Check this file for detailed debugging information.

## Limitations

- **Only targets legacy malware** - Modern malware uses more sophisticated detection
- **Process-specific only** - Cannot spoof system-wide behavior
- **Hardware-level detection** - Cannot spoof CPUID or kernel-mode checks
- **Behavioral detection** - Cannot hide suspicious process behavior
- **Network module optional** - May conflict with actual network adapter info

## Safety & Disclaimer

⚠️ **WARNING**: This tool injects hooks into processes. While carefully designed to be safe:

- Only use on systems YOU OWN
- Only target processes you intend to analyze
- Test on non-critical systems first
- Keep backups of important data
- Monitor system behavior after use

The authors are not responsible for any system damage or data loss.

## Development

### Project Structure
```
VM-Spoofer/
├── VMSpoofCore/              # Core library
│   ├── ISpoofModule.cs
│   ├── SpoofEngine.cs
│   ├── BaseSpoofModule.cs
│   ├── SpoofConfiguration.cs
│   ├── Logger.cs
│   └── Modules/
│       ├── RegistrySpoofModule.cs
│       ├── FileSystemSpoofModule.cs
│       ├── ProcessSpoofModule.cs
│       ├── WmiSpoofModule.cs
│       └── NetworkSpoofModule.cs
├── VMSpoofLauncher/          # WPF UI
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── MainWindowViewModel.cs
│   └── App.xaml
├── VMSpoofInjector/          # CLI Injector
│   └── Program.cs
├── setup.bat
├── setup.ps1
├── setup.sh
└── README.md
```

### Building

```bash
# Full build
dotnet build "VM-Spoofer.sln" -c Release

# Build specific project
dotnet build "VMSpoofCore\VMSpoofCore.csproj" -c Release

# Clean
dotnet clean "VM-Spoofer.sln"
```

## Contributing

Contributions welcome! Areas for improvement:

- Additional spoof modules (Disk, USB, Display)
- Enhanced WMI hooking
- Performance optimizations
- Better malware detection pattern database
- Unit tests

## License

MIT License - See LICENSE file

## Disclaimer

This tool is provided for educational and authorized security research purposes only. Unauthorized use of this tool against systems you don't own is illegal. Always ensure you have explicit permission before testing malware or using intrusive analysis tools.

## Support

For issues, questions, or feature requests:

1. Check existing issues on GitHub
2. Review the Troubleshooting section above
3. Check logs in `%APPDATA%\VMSpoofer\spoofer.log`
4. Open a new GitHub issue with:
   - Windows version
   - .NET version
   - Exact steps to reproduce
   - Target process and malware (if applicable)
   - Log file contents

---

**Happy malware analysis! 🛡️**
