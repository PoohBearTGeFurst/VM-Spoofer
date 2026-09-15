# Quick Start Guide - VM-Spoofer

## 5-Minute Setup

### Step 1: Install Prerequisites

**Option A: Windows (Automatic)**
```batch
setup.bat
```

**Option B: PowerShell (Automatic)**
```powershell
powershell -ExecutionPolicy Bypass -File setup.ps1
```

This will:
- ✅ Check for .NET 6 SDK
- ✅ Restore NuGet packages
- ✅ Build the project
- ✅ Create Output folder with binaries

### Step 2: Run the Launcher

```
Output\VMSpoofLauncher.exe
```

⚠️ **Must run as Administrator!**
- Right-click → Run as administrator
- Or: `runas /user:Administrator Output\VMSpoofLauncher.exe`

### Step 3: Select Target Process

1. Click the dropdown and select a process
2. Click **Refresh** if you don't see your target
3. Common targets:
   - `notepad.exe` (safe test)
   - `explorer.exe` (file manager)
   - Malware binary (for testing)

### Step 4: Configure Modules

Enable/disable spoof modules:
- ✅ Registry Spoofing (default ON)
- ✅ File System Spoofing (default ON)  
- ✅ Process Spoofing (default ON)
- ✅ WMI Spoofing (default ON)
- ⚠️ Network Spoofing (default OFF - may break network)

### Step 5: Start Spoofing

1. Click **START** button
2. Watch status bar turn green
3. Spoofing is now active for that process
4. Run malware analysis
5. Click **STOP** to remove all hooks

---

## Usage Examples

### Example 1: Analyzing Legacy Ransomware

```
1. Launch notepad.exe
2. Open VMSpoofLauncher.exe as Admin
3. Select notepad.exe from list
4. Keep all modules enabled (default)
5. Click START
6. Replace notepad.exe with malware binary
7. Run malware - it sees fake bare metal
8. Click STOP when done analyzing
```

### Example 2: Testing Malware in Isolated VM

```
1. Start malware in VM
2. Open VMSpoofLauncher.exe as Admin
3. Find malware process in list
4. Disable Network Spoofing (keep others on)
5. Click START
6. Malware thinks it's on real machine
7. Monitor behavior while spoofing active
```

### Example 3: Command-Line Testing

```powershell
# Find target process ID
Get-Process notepad | Select-Object ProcessName, Id

# Inject with default config
.\Output\VMSpoofInjector.exe --pid 5678

# Inject with custom config
.\Output\VMSpoofInjector.exe --pid 5678 --config config\vmware.json

# Inject with shorthand
.\Output\VMSpoofInjector.exe -p 5678 -c config\vmware.json
```

---

## Configuration Profiles

### Using Pre-Made Profiles

```bash
# Generic (bare metal)
VMSpoofInjector.exe --pid 1234 --config config/generic.json

# Spoof as VMware
VMSpoofInjector.exe --pid 1234 --config config/vmware.json

# Spoof as VirtualBox
VMSpoofInjector.exe --pid 1234 --config config/virtualbox.json

# Spoof as Hyper-V
VMSpoofInjector.exe --pid 1234 --config config/hyperv.json
```

### Creating Custom Profile

1. Copy `config/generic.json`
2. Modify values:
   ```json
   {
     "fakeManufacturer": "Your Brand",
     "fakeModel": "Your Model",
     "fakeProcessorName": "Your CPU",
     "fakeTotalMemoryMB": 32768
   }
   ```
3. Save as `config/myprofile.json`
4. Use:
   ```bash
   VMSpoofInjector.exe --pid 1234 --config config/myprofile.json
   ```

---

## Troubleshooting

### Problem: "Run as administrator" error
```
❌ ERROR: This script must be run as Administrator!
```
**Solution**: Right-click file → "Run as administrator"

### Problem: ".NET 6 SDK not found"
```
❌ ERROR: .NET 6 SDK not found!
```
**Solution**: 
1. Download .NET 6 from https://dotnet.microsoft.com/download/dotnet/6.0
2. Install SDK (not runtime)
3. Verify: `dotnet --version` in CMD

### Problem: Can't find target process
```
❌ Process not showing in dropdown
```
**Solution**:
1. Click **Refresh** button
2. Try running target as admin
3. Try a different test process (notepad.exe)

### Problem: Hooks fail to install
```
❌ Error starting spoofing: Hook installation failed
```
**Solution**:
1. Ensure process is 64-bit (x64)
2. Ensure you're running as admin
3. Check %APPDATA%\VMSpoofer\spoofer.log
4. Try different target process

### Problem: Network stopped working
```
❌ No internet after running Network module
```
**Solution**: 
1. Restart networking: `ipconfig /release` then `ipconfig /renew`
2. Or: Restart computer
3. Don't enable Network Spoofing unless needed

### Problem: Malware still detects VM
```
❌ Malware still knows it's a VM
```
**Solutions**:
- Malware may use different detection method
- Enable ALL modules (all checkboxes checked)
- Try different spoof profile
- Modern malware may be too sophisticated
- Check logs: %APPDATA%\VMSpoofer\spoofer.log

---

## Next Steps

### Learning More
- Read [README.md](README.md) for full documentation
- Check [CONTRIBUTING.md](CONTRIBUTING.md) for development
- Review [CHANGELOG.md](CHANGELOG.md) for version history

### Common Tasks

**View logs while running:**
```powershell
Get-Content $env:APPDATA\VMSpoofer\spoofer.log -Wait -Tail 10
```

**Find process ID:**
```powershell
Get-Process | Where {$_.ProcessName -like "*malware*"} | Select Name, Id
```

**Kill process:**
```powershell
Stop-Process -Id 1234
# Or: taskkill /PID 1234 /F
```

**List all processes:**
```powershell
Get-Process | Sort ProcessName
```

---

## Security Reminder

⚠️ **IMPORTANT**

- Only use on systems YOU OWN
- Only test malware you have permission to analyze
- Keep backups of important data
- Test on non-critical systems first
- This tool modifies process behavior - use carefully
- Always click STOP when done to remove hooks

---

## Support

For issues:
1. Check troubleshooting section above
2. Review logs: `%APPDATA%\VMSpoofer\spoofer.log`
3. Try a different target process
4. Open GitHub issue with:
   - Windows version
   - .NET version (`dotnet --version`)
   - Target process name
   - Exact error message
   - Steps to reproduce

---

**You're ready to go! 🚀**

Start with notepad.exe as a safe test, then move to malware analysis.
