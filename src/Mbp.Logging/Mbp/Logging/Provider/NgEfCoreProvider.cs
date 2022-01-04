using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mbp.Logging
{
    /// <summary>
    /// EFCORE日志提供程序
    /// </summary>
    public class NgEfCoreProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new NgEfCoreLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// EFCORE日志记录器
    /// </summary>
    public class NgEfCoreLogger : ILogger
    {
        private static readonly string _sqlPattern = "Executed DbCommand ((.*?)) [Parameters=[(.*?)], CommandType='(.*?)', CommandTimeout='(.*?)']";
        private readonly string _categoryName;

        public NgEfCoreLogger(string categoryName)
        {
            _categoryName = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_categoryName == "Microsoft.EntityFrameworkCore.Database.Command" && logLevel == LogLevel.Information)
            {
                // 读取EF CORE的Command日志记录
                var logContent = formatter(state, exception);
                var match = Regex.Match(logContent, _sqlPattern);
                if (match != null)
                {
                    string duration = match.Groups[0]?.Value;
                    string parameters = match.Groups[1]?.Value;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(logContent);
                Console.ResetColor();
            }
        }
    }
}
