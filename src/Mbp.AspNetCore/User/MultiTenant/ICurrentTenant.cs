using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core.User.MultiTenant
{
    /// <summary>
    /// 当前租户信息
    /// </summary>
    public interface ICurrentTenant
    {
        public Guid? TenantId { get; }

        bool IsAvailable { get; }

        string Name { get; }

        void Change(Guid? id, string name = null);
    }
}
