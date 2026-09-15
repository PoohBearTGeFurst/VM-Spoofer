# VM-Spoofer Setup Script for PowerShell
# This script sets up the development environment and builds the project
# Run as Administrator: powershell -ExecutionPolicy Bypass -File setup.ps1

param(
    [switch]$SkipAdmin = $false
)

# Check if running as administrator
$isAdmin = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin -and -not $SkipAdmin) {
    Write-Host "`n" -ForegroundColor Red
    Write-Host "====================================="
    Write-Host "VM-Spoofer Setup Script" -ForegroundColor Cyan
    Write-Host "====================================="
    Write-Host "`nERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Please open PowerShell as Administrator and run:" -ForegroundColor Yellow
    Write-Host "  powershell -ExecutionPolicy Bypass -File setup.ps1" -ForegroundColor Green
    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "`n" -ForegroundColor Cyan
Write-Host "====================================="
Write-Host "VM-Spoofer Setup Script" -ForegroundColor Cyan
Write-Host "====================================="
Write-Host "`n" -ForegroundColor Cyan

if ($isAdmin) {
    Write-Host "[*] Running as Administrator..." -ForegroundColor Green
} else {
    Write-Host "[!] Not running as Administrator (some features may be unavailable)" -ForegroundColor Yellow
}

Write-Host "`n" -ForegroundColor Cyan

# Check if .NET 6 SDK is installed
Write-Host "[*] Checking for .NET 6 SDK..." -ForegroundColor Cyan
$dotnetVersion = dotnet --version 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nERROR: .NET 6 SDK not found!" -ForegroundColor Red
    Write-Host "`nPlease install .NET 6 SDK from:" -ForegroundColor Yellow
    Write-Host "https://dotnet.microsoft.com/download/dotnet/6.0" -ForegroundColor Green
    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "[+] .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host "`n" -ForegroundColor Cyan

# Check if MSBuild is available (optional)
Write-Host "[*] Checking for Visual Studio Build Tools..." -ForegroundColor Cyan
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue

if ($null -eq $msbuild) {
    Write-Host "[!] Visual Studio Build Tools not found (optional)" -ForegroundColor Yellow
    Write-Host "    Install from: https://visualstudio.microsoft.com/downloads/" -ForegroundColor Yellow
    Write-Host "    Select 'Desktop development with C#'" -ForegroundColor Yellow
} else {
    Write-Host "[+] MSBuild found" -ForegroundColor Green
}

Write-Host "`n" -ForegroundColor Cyan

# Restore NuGet packages
Write-Host "[*] Restoring NuGet packages..." -ForegroundColor Cyan
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

dotnet restore "VM-Spoofer.sln"

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore NuGet packages!" -ForegroundColor Red
    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "[+] NuGet packages restored successfully" -ForegroundColor Green
Write-Host "`n" -ForegroundColor Cyan

# Build the solution
Write-Host "[*] Building VM-Spoofer solution..." -ForegroundColor Cyan
Write-Host "    This may take a few minutes..." -ForegroundColor Yellow
Write-Host "`n" -ForegroundColor Cyan

dotnet build "VM-Spoofer.sln" -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "[+] Build completed successfully" -ForegroundColor Green
Write-Host "`n" -ForegroundColor Cyan

# Create output directory
if (-not (Test-Path "Output")) {
    New-Item -ItemType Directory -Path "Output" -Force | Out-Null
}
Write-Host "[+] Output directory created" -ForegroundColor Green
Write-Host "`n" -ForegroundColor Cyan

# Copy executables to Output folder
Write-Host "[*] Copying binaries to Output folder..." -ForegroundColor Cyan

if (Test-Path "VMSpoofLauncher\bin\Release\net6.0-windows") {
    Copy-Item -Path "VMSpoofLauncher\bin\Release\net6.0-windows\*" -Destination "Output\" -Recurse -Force
    Write-Host "[+] Launcher binaries copied" -ForegroundColor Green
}

if (Test-Path "VMSpoofCore\bin\Release\net6.0-windows") {
    Copy-Item -Path "VMSpoofCore\bin\Release\net6.0-windows\*" -Destination "Output\" -Recurse -Force
    Write-Host "[+] Core binaries copied" -ForegroundColor Green
}

Write-Host "`n" -ForegroundColor Cyan
Write-Host "`n" -ForegroundColor Cyan
Write-Host "====================================="
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "====================================="
Write-Host "`n" -ForegroundColor Cyan

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run the launcher: VMSpoofLauncher.exe" -ForegroundColor Green
Write-Host "2. Select a target process from the list" -ForegroundColor Green
Write-Host "3. Enable desired spoof modules" -ForegroundColor Green
Write-Host "4. Click START to begin spoofing" -ForegroundColor Green
Write-Host "`n" -ForegroundColor Cyan
Write-Host "For more information, see README.md" -ForegroundColor Yellow
Write-Host "`n" -ForegroundColor Cyan

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
