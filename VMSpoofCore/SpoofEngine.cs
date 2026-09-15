using System;
using System.Collections.Generic;
using System.Linq;

namespace VMSpoofCore
{
    /// <summary>
    /// Main orchestrator for all spoofing modules.
    /// Manages module lifecycle and coordinates hook installation.
    /// </summary>
    public class SpoofEngine : IDisposable
    {
        private readonly Dictionary<string, ISpoofModule> _modules = new();
        private SpoofConfiguration? _configuration;
        private bool _disposed = false;
        private bool _hooksInstalled = false;

        /// <summary>Gets whether hooks are currently installed.</summary>
        public bool HooksInstalled => _hooksInstalled;

        /// <summary>Gets the number of registered modules.</summary>
        public int ModuleCount => _modules.Count;

        /// <summary>Registers a spoofing module.</summary>
        public void RegisterModule(ISpoofModule module)
        {
            ThrowIfDisposed();
            
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (_modules.ContainsKey(module.Name))
                throw new InvalidOperationException($"Module '{module.Name}' already registered");

            _modules[module.Name] = module;
            Logger.Info($"Module registered: {module.Name}");
        }

        /// <summary>Initializes all registered modules with the given configuration.</summary>
        public void Initialize(SpoofConfiguration config)
        {
            ThrowIfDisposed();

            if (_configuration != null)
                throw new InvalidOperationException("SpoofEngine already initialized");

            _configuration = config ?? throw new ArgumentNullException(nameof(config));

            // Initialize only enabled modules
            foreach (var moduleName in config.EnabledModules)
            {
                if (_modules.TryGetValue(moduleName, out var module))
                {
                    try
                    {
                        module.Initialize(config);
                        Logger.Info($"Module initialized: {moduleName}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to initialize module {moduleName}", ex);
                        throw;
                    }
                }
                else
                {
                    Logger.Warning($"Enabled module not found: {moduleName}");
                }
            }
        }

        /// <summary>Installs all hooks for enabled modules.</summary>
        public void InstallHooks()
        {
            ThrowIfDisposed();

            if (_configuration == null)
                throw new InvalidOperationException("SpoofEngine not initialized. Call Initialize() first.");

            if (_hooksInstalled)
                throw new InvalidOperationException("Hooks already installed");

            try
            {
                foreach (var moduleName in _configuration.EnabledModules)
                {
                    if (_modules.TryGetValue(moduleName, out var module))
                    {
                        try
                        {
                            module.InstallHooks();
                            Logger.Info($"Hooks installed for module: {moduleName}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Failed to install hooks for module {moduleName}", ex);
                            throw;
                        }
                    }
                }

                _hooksInstalled = true;
                Logger.Info("All hooks installed successfully");
            }
            catch
            {
                // Rollback: uninstall any hooks that were installed
                UninstallHooks();
                throw;
            }
        }

        /// <summary>Uninstalls all hooks.</summary>
        public void UninstallHooks()
        {
            ThrowIfDisposed();

            if (!_hooksInstalled)
                return;

            foreach (var module in _modules.Values)
            {
                try
                {
                    module.UninstallHooks();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to uninstall hooks for module {module.Name}", ex);
                }
            }

            _hooksInstalled = false;
            Logger.Info("All hooks uninstalled");
        }

        /// <summary>Gets a list of all installed hooks across all modules.</summary>
        public IReadOnlyList<string> GetInstalledHooks()
        {
            var hooks = new List<string>();
            foreach (var module in _modules.Values)
            {
                hooks.AddRange(module.GetInstalledHooks());
            }
            return hooks.AsReadOnly();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            UninstallHooks();

            foreach (var module in _modules.Values)
            {
                try
                {
                    module?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error disposing module {module.Name}", ex);
                }
            }

            _modules.Clear();
            _disposed = true;
            Logger.Info("SpoofEngine disposed");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SpoofEngine));
        }
    }
}
