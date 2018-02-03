using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Common;

namespace NuGetDepends
{
    public class NuGetSerilogLogger : ILogger
    {
        public Serilog.ILogger BaseLogger { get; }

        public NuGetSerilogLogger(Serilog.ILogger baseLogger)
        {
            BaseLogger = baseLogger ?? throw new ArgumentNullException(nameof(baseLogger));
        }

        /// <inheritdoc />
        public void LogDebug(string data)
        {
            BaseLogger.Debug(data);
        }

        /// <inheritdoc />
        public void LogVerbose(string data)
        {
            BaseLogger.Verbose(data);
        }

        /// <inheritdoc />
        public void LogInformation(string data)
        {
            BaseLogger.Information(data);
        }

        /// <inheritdoc />
        public void LogMinimal(string data)
        {
            BaseLogger.Information(data);
        }

        /// <inheritdoc />
        public void LogWarning(string data)
        {
            BaseLogger.Warning(data);
        }

        /// <inheritdoc />
        public void LogError(string data)
        {
            BaseLogger.Error(data);
        }

        /// <inheritdoc />
        public void LogInformationSummary(string data)
        {
            BaseLogger.Information(data);
        }

        /// <inheritdoc />
        public void LogErrorSummary(string data)
        {
            BaseLogger.Error(data);
        }
    }
}
