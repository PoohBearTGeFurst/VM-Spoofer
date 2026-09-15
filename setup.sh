#!/bin/bash

# VM-Spoofer Setup Script for Linux/WSL
# This script sets up the development environment and builds the project

set -e

echo ""
echo "====================================="
echo "VM-Spoofer Setup Script"
echo "====================================="
echo ""

# Check if running on Windows/WSL
if grep -qi "microsoft" /proc/version; then
    echo "[*] Running on WSL (Windows Subsystem for Linux)"
else
    echo "[!] This project is designed for Windows."
    echo "    Use WSL or a Windows machine to run this."
fi

echo ""

# Check if .NET SDK is installed
echo "[*] Checking for .NET 6 SDK..."
if ! command -v dotnet &> /dev/null; then
    echo ""
    echo "ERROR: .NET 6 SDK not found!"
    echo ""
    echo "Please install .NET 6 SDK from:"
    echo "https://dotnet.microsoft.com/download/dotnet/6.0"
    echo ""
    echo "On Ubuntu/Debian:"
    echo "  wget https://dot.net/v1/dotnet-install.sh"
    echo "  chmod +x dotnet-install.sh"
    echo "  ./dotnet-install.sh --channel 6.0"
    echo ""
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "[+] .NET SDK version: $DOTNET_VERSION"
echo ""

# Restore NuGet packages
echo "[*] Restoring NuGet packages..."
cd "$(dirname "$0")"
dotnet restore "VM-Spoofer.sln"
echo "[+] NuGet packages restored successfully"
echo ""

# Build the solution
echo "[*] Building VM-Spoofer solution..."
echo "    This may take a few minutes..."
echo ""
dotnet build "VM-Spoofer.sln" -c Release
echo "[+] Build completed successfully"
echo ""

# Create output directory
mkdir -p Output
echo "[+] Output directory created"
echo ""

# Copy executables to Output folder
echo "[*] Copying binaries to Output folder..."
if [ -d "VMSpoofLauncher/bin/Release/net6.0-windows" ]; then
    cp -r "VMSpoofLauncher/bin/Release/net6.0-windows"/* Output/ 2>/dev/null || true
    echo "[+] Launcher binaries copied"
fi

if [ -d "VMSpoofCore/bin/Release/net6.0-windows" ]; then
    cp -r "VMSpoofCore/bin/Release/net6.0-windows"/* Output/ 2>/dev/null || true
    echo "[+] Core binaries copied"
fi
echo ""

echo ""
echo "====================================="
echo "Setup Complete!"
echo "====================================="
echo ""
echo "Next steps:"
echo "1. On Windows, run the launcher: VMSpoofLauncher.exe"
echo "2. Select a target process from the list"
echo "3. Enable desired spoof modules"
echo "4. Click START to begin spoofing"
echo ""
echo "For more information, see README.md"
echo ""
