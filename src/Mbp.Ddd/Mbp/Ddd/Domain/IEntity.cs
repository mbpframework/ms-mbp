using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Domain
{
    /// <summary>
    /// 指示一个类型是否是实体
    /// </summary>
    public interface IEntity
    {

    }

    /// <summary>
    /// 实体类抽象接口
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<out TKey> : IEntity
    {
        TKey ID { get; }
    }
}
