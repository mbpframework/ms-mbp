using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Mbp.Core
{
    public class ServiceProviderEngine : IServiceProviderEngine
    {
        private IServiceProvider _provider;
        public ServiceProviderEngine(IServiceProvider provider)
        {
            this._provider = provider;
        }

        public T Resolve<T>()
        {
            if (this._provider != null)
            {
                return this._provider.GetService<T>();
            }
            return default(T);
        }
    }
}
