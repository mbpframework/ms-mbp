using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core
{
    public interface IServiceProviderEngine
    {
        T Resolve<T>();
    }
}
