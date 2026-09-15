using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VMSpoofCore;

namespace VMSpoofInjector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("VM-Spoofer Command Line Injector");
            Console.WriteLine("=====================================");
            Console.WriteLine();

            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            try
            {
                // Parse command line arguments
                int? pid = null;
                string? configFile = null;

                for (int i = 0; i < args.Length; i++)
                {
                    if ((args[i] == "--pid" || args[i] == "-p") && i + 1 < args.Length)
                    {
                        if (int.TryParse(args[i + 1], out int parsedPid))
                        {
                            pid = parsedPid;
                            i++;
                        }
                    }
                    else if ((args[i] == "--config" || args[i] == "-c") && i + 1 < args.Length)
                    {
                        configFile = args[i + 1];
                        i++;
                    }
                }

                if (pid == null)
                {
                    Console.WriteLine("ERROR: PID not specified");
                    PrintUsage();
                    return;
                }

                // Load configuration
                SpoofConfiguration config = new();

                if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
                {
                    try
                    {
                        string json = File.ReadAllText(configFile);
                        var loadedConfig = JsonSerializer.Deserialize<SpoofConfiguration>(json);
                        if (loadedConfig != null)
                        {
                            config = loadedConfig;
                            Console.WriteLine($"[+] Configuration loaded from {configFile}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] Failed to load config file: {ex.Message}");
                        Console.WriteLine("[*] Using default configuration");
                    }
                }
                else if (!string.IsNullOrEmpty(configFile))
                {
                    Console.WriteLine($"[!] Config file not found: {configFile}");
                    Console.WriteLine("[*] Using default configuration");
                }
                else
                {
                    Console.WriteLine("[*] Using default configuration");
                }

                Console.WriteLine();

                // Verify process exists
                Process? targetProcess = null;
                try
                {
                    targetProcess = Process.GetProcessById(pid.Value);
                    Console.WriteLine($"[+] Target process found: {targetProcess.ProcessName} (PID: {pid})");
                }
                catch
                {
                    Console.WriteLine($"ERROR: Process with PID {pid} not found");
                    return;
                }

                Console.WriteLine();

                // Initialize spoof engine
                Console.WriteLine("[*] Initializing spoof engine...");
                using (var engine = new SpoofEngine())
                {
                    // Register modules
                    engine.RegisterModule(new RegistrySpoofModule());
                    engine.RegisterModule(new FileSystemSpoofModule());
                    engine.RegisterModule(new ProcessSpoofModule());
                    engine.RegisterModule(new WmiSpoofModule());
                    engine.RegisterModule(new NetworkSpoofModule());

                    Console.WriteLine($"[+] {engine.ModuleCount} modules registered");
                    Console.WriteLine();

                    // Initialize with configuration
                    Console.WriteLine("[*] Initializing modules...");
                    engine.Initialize(config);
                    Console.WriteLine("[+] Modules initialized");
                    Console.WriteLine();

                    // Install hooks
                    Console.WriteLine("[*] Installing hooks...");
                    engine.InstallHooks();
                    Console.WriteLine("[+] Hooks installed successfully");
                    Console.WriteLine();

                    // Display installed hooks
                    var hooks = engine.GetInstalledHooks();
                    Console.WriteLine($"[+] Total hooks installed: {hooks.Count}");
                    foreach (var hook in hooks)
                    {
                        Console.WriteLine($"    - {hook}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("[*] Spoofing active. Press Enter to stop...");
                    Console.ReadLine();

                    Console.WriteLine("[*] Stopping spoofing...");
                }

                Console.WriteLine("[+] All hooks removed. Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: VMSpoofInjector.exe --pid <PID> [--config <CONFIG_FILE>]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --pid, -p <PID>           Target process ID");
            Console.WriteLine("  --config, -c <FILE>       Configuration JSON file (optional)");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  VMSpoofInjector.exe --pid 1234");
            Console.WriteLine("  VMSpoofInjector.exe --pid 1234 --config spoof.json");
            Console.WriteLine();
        }
    }
}
