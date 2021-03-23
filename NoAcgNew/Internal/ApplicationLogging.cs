using Microsoft.Extensions.Logging;

namespace NoAcgNew.Internal
{
    /// <summary>
    /// Shared logger
    /// </summary>
    internal static class ApplicationLogging
    {
        internal static ILoggerFactory LoggerFactory { get; set; }// = new LoggerFactory();
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}