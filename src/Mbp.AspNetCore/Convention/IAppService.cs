using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.AspNetCore.Convention
{
    /// <summary>
    /// 标志此类为一个应用服务，会自动发现成Web API
    /// </summary>
    public interface IAppService : IRemoteService
    {
    }
}
