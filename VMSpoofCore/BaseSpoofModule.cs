using System;
using System.Collections.Generic;
using EasyHook;

namespace VMSpoofCore
{
    /// <summary>
    /// Base class for all spoofing modules.
    /// Provides common functionality and hook management.
    /// </summary>
    public abstract class BaseSpoofModule : ISpoofModule
    {
        protected readonly List<LocalHook> InstalledHooks = new();
        protected SpoofConfiguration? Configuration;
        private bool _disposed = false;

        public abstract string Name { get; }

        public virtual bool IsActive { get; protected set; }

        public virtual void Initialize(SpoofConfiguration config)
        {
            ThrowIfDisposed();
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
        }

        public abstract void InstallHooks();

        public virtual void UninstallHooks()
        {
            ThrowIfDisposed();

            foreach (var hook in InstalledHooks)
            {
                try
                {
                    hook.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error disposing hook in {Name}", ex);
                }
            }

            InstalledHooks.Clear();
            IsActive = false;
        }

        public IReadOnlyList<string> GetInstalledHooks()
        {
            return InstalledHooks.ConvertAll(h => h.MethodName).AsReadOnly();
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            UninstallHooks();
            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(Name);
        }

        protected void ThrowIfNotInitialized()
        {
            if (Configuration == null)
                throw new InvalidOperationException($"{Name} module not initialized");
        }
    }
}
