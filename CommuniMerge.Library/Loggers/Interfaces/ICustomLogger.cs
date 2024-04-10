using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Loggers.Interfaces
{
    public interface ICustomLogger
    {
        void LogTrace(string exceptionMessage, string className, string methodName);
        void LogDebug(string exceptionMessage, string className, string methodName);
        void LogInformation(string exceptionMessage, string className, string methodName);
        void LogWarning(string exceptionMessage, string className, string methodName);
        void LogError(string exceptionMessage, string className, string methodName);
        void LogCritical(string exceptionMessage, string className, string methodName);
        bool IsEnabled(LogLevel logLevel);
    }
}
