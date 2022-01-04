namespace Microsoft.Extensions.Logging
{
    public static class ILoggingBuilderExtensions
    {
        /// <summary>
        /// 添加Mbp日志提供程序
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddMbpLogProvider(this ILoggingBuilder builder)
        {          
            return builder;
        }
    }
}
