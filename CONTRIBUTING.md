# VM-Spoofer Development Guidelines

## Code Style

- **C# version**: Latest (C# 11)
- **Naming**: PascalCase for public, camelCase for private
- **Nullability**: Always enable nullable reference types
- **Comments**: XML documentation for public APIs

### Example

```csharp
/// <summary>
/// Initializes the spoofing module.
/// </summary>
/// <param name="config">Configuration for this module</param>
public void Initialize(SpoofConfiguration config)
{
    ThrowIfDisposed();
    Configuration = config ?? throw new ArgumentNullException(nameof(config));
}
```

## Architecture Principles

### 1. Modularity
Each spoof module should be completely independent:
- Implement ISpoofModule interface
- No dependencies between modules
- Can be enabled/disabled individually

### 2. Performance First
All spoofing should be optimized for speed:
- Fast-path for non-spoofed calls
- Pre-computed lookup caches
- Minimal allocations
- Zero-copy where possible

### 3. Safety
Ensure hooks are safe and reversible:
- All hooks must be uninstallable
- No persistent state modifications
- Automatic cleanup on failure
- Graceful error handling

### 4. Isolation
Keep changes isolated to target process:
- Process-specific hooks only
- No system-wide modifications
- No registry/file system changes
- No background services

## Adding a New Module

1. Create new class inheriting from `BaseSpoofModule`
2. Implement abstract methods:
   ```csharp
   public override string Name => "ModuleName";
   public override void InstallHooks() { ... }
   ```
3. Register in `MainWindowViewModel.cs` and `VMSpoofInjector/Program.cs`
4. Add configuration options to `SpoofConfiguration.cs`
5. Update UI checkboxes in `MainWindow.xaml`
6. Document in README.md

## Testing

### Manual Testing

1. Build project in Release mode
2. Run as Administrator
3. Select test process
4. Enable modules
5. Click START
6. Run detection tool against target process
7. Verify detections are spoofed

### Test Cases

- [ ] Single module disabled
- [ ] All modules enabled
- [ ] Process termination cleans up hooks
- [ ] No impact on other processes
- [ ] Configuration loading works
- [ ] Invalid configuration handled gracefully

## Common Issues & Solutions

### Issue: Hook installation fails
**Solution**: 
- Ensure target process is 64-bit (x64)
- Check that EasyHook supports the API being hooked
- Verify process has sufficient permissions

### Issue: Spoofed values not returned
**Solution**:
- Check that module is in EnabledModules list
- Verify hook was installed (check logs)
- Ensure malware is calling the expected API

### Issue: System-wide performance degradation
**Solution**:
- Module likely has a memory leak or infinite loop
- Check that hooks are properly cleaned up
- Use Task Manager to monitor memory usage
- Review recent module changes

## Performance Benchmarks

Target metrics:
- Hook install: <100ms
- Per-call overhead: <0.1ms
- Memory footprint: <10MB
- Process startup: No delay

Benchmark with:
```csharp
var sw = Stopwatch.StartNew();
engine.InstallHooks();
sw.Stop();
Console.WriteLine($"Install time: {sw.ElapsedMilliseconds}ms");
```

## Debugging

### Enable Debug Logging

1. Build in Debug mode
2. Logs automatically written to: `%APPDATA%\VMSpoofer\spoofer.log`
3. Tail logs while running:
   ```powershell
   Get-Content $env:APPDATA\VMSpoofer\spoofer.log -Wait
   ```

### Attach Debugger

1. Visual Studio → Debug → Attach to Process
2. Filter for target process
3. Set breakpoints in module code
4. Step through interception

## Documentation

- Keep README.md up-to-date
- Add XML comments to public APIs
- Document module-specific configuration
- Include usage examples

## Security Considerations

⚠️ **IMPORTANT**

- Never modify system files
- Never persist registry changes
- Never create scheduled tasks
- Never install services
- Always clean up on unload
- Never escalate privileges beyond what's needed
- Validate all input (filenames, registry paths, etc.)
- Use safe string comparisons

## Future Enhancements

Potential areas for expansion:

- [ ] Kernel-mode driver for CPUID spoofing
- [ ] USB device spoofing
- [ ] Display adapter spoofing
- [ ] Disk geometry spoofing
- [ ] Memory layout randomization detection
- [ ] Behavioral analysis evasion
- [ ] Malware pattern database
- [ ] Automated testing framework
- [ ] DLL injection detection evasion
- [ ] Anti-debugging circumvention

## References

- [EasyHook Documentation](https://www.easyhook.net/)
- [Windows API Documentation](https://docs.microsoft.com/en-us/windows/)
- [Common VM Detection Techniques](https://evasions.checkpoint.com/)
- [Malware Analysis OSINT](https://www.exploit-db.com/)

---

*Last Updated: 2026-09-15*
