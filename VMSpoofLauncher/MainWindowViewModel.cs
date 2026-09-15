using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using VMSpoofCore;

namespace VMSpoofLauncher
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private SpoofEngine? _spoofEngine;
        private Process? _targetProcess;
        private bool _isRunning;
        private string _statusMessage = "Ready";
        private int _selectedProcessIndex = -1;
        private ObservableCollection<ProcessInfo> _processList;
        private bool _registryModuleEnabled = true;
        private bool _fileSystemModuleEnabled = true;
        private bool _processModuleEnabled = true;
        private bool _wmiModuleEnabled = true;
        private bool _networkModuleEnabled = false;

        public ObservableCollection<ProcessInfo> ProcessList
        {
            get => _processList;
            set { SetProperty(ref _processList, value); }
        }

        public int SelectedProcessIndex
        {
            get => _selectedProcessIndex;
            set { SetProperty(ref _selectedProcessIndex, value); }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set { SetProperty(ref _isRunning, value); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { SetProperty(ref _statusMessage, value); }
        }

        public bool RegistryModuleEnabled
        {
            get => _registryModuleEnabled;
            set { SetProperty(ref _registryModuleEnabled, value); }
        }

        public bool FileSystemModuleEnabled
        {
            get => _fileSystemModuleEnabled;
            set { SetProperty(ref _fileSystemModuleEnabled, value); }
        }

        public bool ProcessModuleEnabled
        {
            get => _processModuleEnabled;
            set { SetProperty(ref _processModuleEnabled, value); }
        }

        public bool WmiModuleEnabled
        {
            get => _wmiModuleEnabled;
            set { SetProperty(ref _wmiModuleEnabled, value); }
        }

        public bool NetworkModuleEnabled
        {
            get => _networkModuleEnabled;
            set { SetProperty(ref _networkModuleEnabled, value); }
        }

        public MainWindowViewModel()
        {
            _processList = new ObservableCollection<ProcessInfo>();
            RefreshProcessList();
        }

        public void RefreshProcessList()
        {
            try
            {
                ProcessList.Clear();
                var processes = Process.GetProcesses();
                
                foreach (var proc in processes)
                {
                    try
                    {
                        var name = proc.ProcessName;
                        var id = proc.Id;
                        ProcessList.Add(new ProcessInfo { Name = name, Pid = id, DisplayName = $"{name} (PID: {id})" });
                    }
                    catch
                    {
                        // Skip processes we can't access
                    }
                }

                StatusMessage = $"Loaded {ProcessList.Count} processes";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading processes: {ex.Message}";
            }
        }

        public void StartSpoofing()
        {
            if (SelectedProcessIndex < 0)
            {
                StatusMessage = "Please select a process first";
                return;
            }

            if (IsRunning)
            {
                StatusMessage = "Spoofing already running";
                return;
            }

            try
            {
                var selectedProcess = ProcessList[SelectedProcessIndex];
                _targetProcess = Process.GetProcessById(selectedProcess.Pid);

                // Build configuration based on enabled modules
                var config = new SpoofConfiguration
                {
                    EnabledModules = new()
                {
                        (RegistryModuleEnabled ? "Registry" : null),
                        (FileSystemModuleEnabled ? "FileSystem" : null),
                        (ProcessModuleEnabled ? "Process" : null),
                        (WmiModuleEnabled ? "WMI" : null),
                        (NetworkModuleEnabled ? "Network" : null)
                    }.FindAll(x => x != null)!
                };

                _spoofEngine = new SpoofEngine();

                // Register all available modules
                _spoofEngine.RegisterModule(new RegistrySpoofModule());
                _spoofEngine.RegisterModule(new FileSystemSpoofModule());
                _spoofEngine.RegisterModule(new ProcessSpoofModule());
                _spoofEngine.RegisterModule(new WmiSpoofModule());
                _spoofEngine.RegisterModule(new NetworkSpoofModule());

                // Initialize and install hooks
                _spoofEngine.Initialize(config);
                _spoofEngine.InstallHooks();

                IsRunning = true;
                StatusMessage = $"Spoofing active on {selectedProcess.DisplayName}";
                Logger.Info($"Spoofing started for PID {selectedProcess.Pid}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error starting spoofing: {ex.Message}";
                Logger.Error($"Failed to start spoofing", ex);
                IsRunning = false;
            }
        }

        public void StopSpoofing()
        {
            if (!IsRunning)
                return;

            try
            {
                _spoofEngine?.UninstallHooks();
                _spoofEngine?.Dispose();
                _spoofEngine = null;
                _targetProcess = null;

                IsRunning = false;
                StatusMessage = "Spoofing stopped";
                Logger.Info("Spoofing stopped");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error stopping spoofing: {ex.Message}";
                Logger.Error("Failed to stop spoofing", ex);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class ProcessInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Pid { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
