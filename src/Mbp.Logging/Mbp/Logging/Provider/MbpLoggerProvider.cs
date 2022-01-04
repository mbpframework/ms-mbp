using Microsoft.Extensions.Logging;

namespace Mbp.Logging
{
    /// <summary>
    /// Mbp日志提供程序，框架中的所有写日志请求都将通过这个提供程序
    /// </summary>
    [ProviderAlias("NgLog")]
    public class MbpLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MbpLogger(categoryName);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
