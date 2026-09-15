# VM-Spoofer Security Policy

## Reporting Security Vulnerabilities

If you discover a security vulnerability in VM-Spoofer, please report it
responsibly by emailing the maintainers directly rather than using public
issue tracking.

### What to Include

1. Description of the vulnerability
2. Steps to reproduce (if applicable)
3. Potential impact
4. Suggested fix (if you have one)
5. Your contact information

### What NOT to do

- Do NOT post exploit code publicly
- Do NOT test against systems you don't own
- Do NOT disclose the vulnerability before a fix is released
- Do NOT use this tool for illegal activities

## Security Best Practices

When using VM-Spoofer:

1. **Always use administrator accounts sparingly** - Only run as admin when
   absolutely necessary

2. **Test in isolated environments** - Use VMs, air-gapped networks, or
   sandboxes

3. **Disable after use** - Always click STOP to remove hooks immediately
   after analysis

4. **Monitor system behavior** - Watch for unexpected changes or crashes

5. **Keep backups** - Maintain current backups before using process injection
   tools

6. **Verify target process** - Double-check the target process ID before
   starting spoofing

7. **Review logs** - Check spoofer.log for errors or unexpected behavior

8. **Test safely** - Start with harmless processes (notepad.exe) before
   malware

## Known Limitations

VM-Spoofer has these security limitations:

1. **Process-specific only** - Cannot provide system-wide spoofing
2. **Legacy malware only** - Modern malware uses additional detection vectors
3. **User-mode only** - Cannot spoof kernel-mode checks (CPUID, etc.)
4. **No behavior hiding** - Malware may still exhibit suspicious behavior
5. **Requires admin** - Necessary for process injection

## Supported & Tested Platforms

- Windows 11 (21H2+)
- .NET 6.0 Runtime/SDK
- x64 architecture only

## Unsupported Platforms

VM-Spoofer is NOT supported on:
- Windows 10 or earlier
- x86 (32-bit) systems
- Other operating systems

## Version Support

Only the latest version receives security updates.

## Responsible Disclosure Timeline

We will attempt to:
1. Acknowledge vulnerability report within 48 hours
2. Release patch within 14 days of report
3. Provide credit to reporter (if desired)

## Security Incidents

If you discover that VM-Spoofer is being misused for illegal purposes,
please report it to the appropriate authorities.

---

*Last Updated: 2026-09-15*
