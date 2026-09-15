using System;
using System.Collections.Generic;

namespace VMSpoofCore
{
    /// <summary>
    /// Interface for all VM spoofing modules.
    /// Each module is responsible for spoofing a specific detection vector.
    /// </summary>
    public interface ISpoofModule : IDisposable
    {
        /// <summary>Gets the name of this spoof module.</summary>
        string Name { get; }

        /// <summary>Gets whether this module is currently active.</summary>
        bool IsActive { get; }

        /// <summary>
        /// Initializes the module with the given configuration.
        /// This is called once before any hooks are installed.
        /// </summary>
        void Initialize(SpoofConfiguration config);

        /// <summary>
        /// Installs all hooks for this module.
        /// Should only be called after Initialize().
        /// </summary>
        void InstallHooks();

        /// <summary>
        /// Uninstalls all hooks for this module.
        /// Should be idempotent and safe to call multiple times.
        /// </summary>
        void UninstallHooks();

        /// <summary>
        /// Gets a list of all currently installed hooks for this module.
        /// </summary>
        IReadOnlyList<string> GetInstalledHooks();
    }
}
