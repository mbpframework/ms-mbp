namespace Mbp.Logging
{
    /// <summary>
    /// 日志提供程序创建工厂
    /// </summary>
    public class LoggerProviderFactory
    {
        public IMbpLoggerProvider Create(string provider)
        {
            return provider switch
            {
                "Serilog" => new SerilogProvider(),
                _ => null,
            };
        }
    }
}
