using System;
using System.IO;

namespace WeatherApplication
{
    public sealed class ErrorLogger
    {
        private static readonly Lazy<ErrorLogger> lazyInstance = new Lazy<ErrorLogger>(() => new ErrorLogger());
        private readonly string logFilePath;

        public static ErrorLogger Instance => lazyInstance.Value;

        private ErrorLogger()
        {
            // Get the log file path from environment variables, or use a default value if not set
            logFilePath = Environment.GetEnvironmentVariable("ERROR_LOG_FILE_PATH") ?? "error.log";
        }

        public void LogError(string errorMessage)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Log to console if writing to file fails
                Console.WriteLine($"Failed to log error: {ex.Message}");
            }
        }
    }
}

