using System;
using System.Diagnostics;
using System.IO;

namespace VMSpoofCore
{
    /// <summary>
    /// Simple logging utility for debugging and monitoring.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VMSpoofer",
            "spoofer.log"
        );

        private static readonly object LockObj = new();
        private static bool _initialized = false;

        static Logger()
        {
            Initialize();
        }

        private static void Initialize()
        {
            lock (LockObj)
            {
                if (_initialized) return;

                try
                {
                    var logDir = Path.GetDirectoryName(LogFile);
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir!);
                    }
                    _initialized = true;
                }
                catch
                {
                    // Silently fail if logging setup fails
                }
            }
        }

        public static void Info(string message)
        {
            Log("[INFO]", message);
        }

        public static void Warning(string message)
        {
            Log("[WARN]", message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            var msg = ex != null ? $"{message} - {ex}" : message;
            Log("[ERROR]", msg);
        }

        public static void Debug(string message)
        {
#if DEBUG
            Log("[DEBUG]", message);
#endif
        }

        private static void Log(string level, string message)
        {
            if (!_initialized) return;

            try
            {
                lock (LockObj)
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logMessage = $"{timestamp} {level} {message}";
                    File.AppendAllText(LogFile, logMessage + Environment.NewLine);
                }
            }
            catch
            {
                // Silently fail
            }
        }
    }
}
