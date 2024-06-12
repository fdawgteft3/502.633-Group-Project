using System;
using System.IO;

namespace WeatherApplication
{
    public sealed class ErrorLogger
    {
        // Singleton Design Pattern using Lazy<T> for thread-safe lazy initialization
        private static readonly Lazy<ErrorLogger> lazyInstance = new Lazy<ErrorLogger>(() => new ErrorLogger());

        // Log file path
        private readonly string logFilePath;

        // Public property to access the Singleton instance
        public static ErrorLogger Instance => lazyInstance.Value;

        // Private constructor to prevent instantiation from outside
        private ErrorLogger()
        {
            // Get the log file path from environment variables, or use a default value if not set
            logFilePath = Environment.GetEnvironmentVariable("ERROR_LOG_FILE_PATH") ?? "error.log";
        }

        // Method to log error messages
        public void LogError(string errorMessage)
        {
            try
            {
                // Append error message with date and time to the log file
                using (var writer = new StreamWriter(logFilePath, true))
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