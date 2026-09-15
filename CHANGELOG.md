# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2026-09-15

### Added
- Initial release
- WPF UI launcher with process selection
- Registry spoofing module
- FileSystem spoofing module
- Process enumeration spoofing module
- WMI spoofing module
- Network adapter spoofing module (optional)
- Command-line injector (VMSpoofInjector)
- Automated setup scripts (batch, PowerShell, Bash)
- Configuration system with JSON profiles
- Comprehensive logging
- Pre-configured VM profiles (Generic, VMware, VirtualBox, Hyper-V)
- Full documentation

### Features
- Selective process-specific injection
- Individual module enable/disable controls
- Fast-path optimization for non-spoofed calls
- Zero system impact (isolated to target process)
- Auto-cleanup on process exit
- Per-module hook management
- Status monitoring and real-time feedback

### Known Limitations
- Only targets legacy malware detection methods
- Process-specific only (no system-wide spoofing)
- Cannot spoof kernel-mode CPUID instruction
- Cannot hide behavioral indicators
- Network module may interfere with connectivity

---

*VM-Spoofer v1.0.0*
*Windows 11 | .NET 6.0*
