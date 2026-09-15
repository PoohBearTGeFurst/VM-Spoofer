@echo off
REM VM-Spoofer Setup Script
REM This script sets up the development environment and builds the project

echo.
echo =====================================
echo VM-Spoofer Setup Script
echo =====================================
echo.

REM Check if running as administrator
powershell -Command "if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) { exit 1 }"

if errorlevel 1 (
    echo ERROR: This script must be run as Administrator!
    echo Please right-click on this file and select "Run as administrator"
    pause
    exit /b 1
)

echo [*] Running as Administrator...
echo.

REM Check if .NET 6 SDK is installed
echo [*] Checking for .NET 6 SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERROR: .NET 6 SDK not found!
    echo.
    echo Please install .NET 6 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [+] .NET SDK version: %DOTNET_VERSION%
echo.

REM Check if Visual Studio Build Tools are available (optional but recommended)
echo [*] Checking for Visual Studio Build Tools...
where msbuild >nul 2>&1
if errorlevel 1 (
    echo [!] Visual Studio Build Tools not found (optional)
    echo     Install from: https://visualstudio.microsoft.com/downloads/
    echo     Select "Desktop development with C#"
) else (
    for /f "tokens=*" %%i in ('msbuild -version ^| findstr /R "^[0-9]"') do set MSBUILD_VERSION=%%i
    echo [+] MSBuild found: %MSBUILD_VERSION%
)
echo.

REM Restore NuGet packages
echo [*] Restoring NuGet packages...
cd /d "%~dp0"
dotnet restore "VM-Spoofer.sln"
if errorlevel 1 (
    echo ERROR: Failed to restore NuGet packages!
    pause
    exit /b 1
)
echo [+] NuGet packages restored successfully
echo.

REM Build the solution
echo [*] Building VM-Spoofer solution...
echo    This may take a few minutes...
echo.
dotnet build "VM-Spoofer.sln" -c Release
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo [+] Build completed successfully
echo.

REM Create output directory
if not exist "Output" mkdir Output
echo [+] Output directory created
echo.

REM Copy executables to Output folder
echo [*] Copying binaries to Output folder...
if exist "VMSpoofLauncher\bin\Release\net6.0-windows" (
    xcopy "VMSpoofLauncher\bin\Release\net6.0-windows\*" "Output\" /E /Y /Q
    echo [+] Launcher binaries copied
)

if exist "VMSpoofCore\bin\Release\net6.0-windows" (
    xcopy "VMSpoofCore\bin\Release\net6.0-windows\*" "Output\" /E /Y /Q
    echo [+] Core binaries copied
)
echo.

echo.
echo =====================================
echo Setup Complete!
echo =====================================
echo.
echo Next steps:
echo 1. Run the launcher: VMSpoofLauncher.exe
echo 2. Select a target process from the list
echo 3. Enable desired spoof modules
echo 4. Click START to begin spoofing
echo.
echo For more information, see README.md
echo.
pause
