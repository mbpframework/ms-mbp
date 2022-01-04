using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core
{
    public class ServiceProviderEngineContext
    {
        private static IServiceProviderEngine _engine;
        public static IServiceProviderEngine Init(IServiceProviderEngine engine)
        {
            if (_engine == null)
            {
                _engine = engine;
            }
            return _engine;
        }

        public static IServiceProviderEngine Current
        {
            get
            {
                return _engine;
            }
        }
    }
}
