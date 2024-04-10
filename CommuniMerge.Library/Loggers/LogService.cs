using CommuniMerge.Library.Data;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Loggers
{
    public class LogService : ICustomLogger
    {
        private readonly LogContext logContext;
        private readonly LogLevel logLevel;
        public LogService(LogContext logContext, IConfiguration configuration)
        {
            this.logContext = logContext;
            this.logLevel = configuration.GetValue<LogLevel>("Logging:LogLevel:Default");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.logLevel <= logLevel;
        }

        private void Log(string exceptionMessage, string className, string methodName, LogLevel logLevel)
        {
            var logEntry = new LogEntry
            {
                TimeStamp = DateTime.UtcNow,
                LogLevel = logLevel,
                ClassName = className,
                MethodName = methodName,
                Message = exceptionMessage,
            };
            logContext.Add(logEntry);
            logContext.SaveChanges();
        }

        public void LogCritical(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Critical);
        }

        public void LogDebug(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Debug);
        }

        public void LogError(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Error);
        }

        public void LogInformation(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Information);
        }

        public void LogTrace(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Trace);
        }

        public void LogWarning(string exceptionMessage, string className, string methodName)
        {
            Log(exceptionMessage, className, methodName, LogLevel.Warning);
        }
    }
}
