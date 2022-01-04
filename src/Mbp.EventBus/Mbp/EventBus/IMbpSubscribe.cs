using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.EventBus
{
    /// <summary>
    /// 指示实现类有Mbp EventBus订阅方法
    /// </summary>
    public interface IMbpSubscribe : ICapSubscribe
    {
    }
}
