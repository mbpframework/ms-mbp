using Mbp.Dependency;
using System.Threading;
using System.Threading.Tasks;

namespace Mbp.EventBus
{
    /// <summary>
    /// 事件总线服务
    /// </summary>
    public interface IMbpEventBus : ISingletonDependency
    {
        /// <summary>
        /// 异步发布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">订阅主题</param>
        /// <param name="contentObj">消息主题</param>
        /// <param name="cancellationToken">取消等待执行</param>
        /// <returns></returns>
        Task PublishAsync<T>(string name, T contentObj, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">订阅主题</param>
        /// <param name="contentObj">消息主题</param>
        void Publish<T>(string name, T contentObj);
    }
}
