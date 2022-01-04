using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Mbp.Logging
{
    /// <summary>
    /// Mbp日志记录器
    /// </summary>
    public class MbpLogger : ILogger
    {
        private readonly string _categoryName;

        public MbpLogger(string categoryName)
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
            // 对日志分类进行过滤，框架产生的日志才进行处理
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_categoryName.StartsWith("Mbp"))
            {
            }
        }
    }
}
