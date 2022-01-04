using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core.User.MultiTenant
{
    /// <summary>
    /// 当前租户信息
    /// </summary>
    public class CurrentTenant : ICurrentTenant
    {
        public bool IsAvailable => TenantId.HasValue;

        public Guid? TenantId { get; private set; }

        public string Name { get; private set; }

        public void Change(Guid? id, string name = null)
        {
            TenantId = id;
            Name = name;
        }
    }
}
