using DotNetCore.CAP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mbp.EventBus;

namespace Mbp.EventBus
{
    public class MbpEventBus : IMbpEventBus
    {
        private readonly ICapPublisher _capBus;

        public MbpEventBus(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        public void Publish<T>(string name, T contentObj)
        {
            _capBus.Publish(name, contentObj);
        }

        public Task PublishAsync<T>(string name, T contentObj, CancellationToken cancellationToken = default)
        {
            return _capBus.PublishAsync(name, contentObj, new Dictionary<string, string>(), cancellationToken);
        }
    }
}
