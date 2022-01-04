using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 因没规划单独的多租户模块，因为多租户作为系统的一种运营上的特性，将它独立会比较合理。
/// 现在没有规划独立分开他，所以将多租户视为一种用户信息的管理，划分在Web下面User下面。
/// </summary>
namespace Mbp.Core.User.MultiTenant
{
    /// <summary>
    /// 指示继承该接口的实体支持多租户存储
    /// </summary>
    public interface IMultiTenant
    {
        Guid? TENANT_ID { get; }
    }
}
