using Nitrogen.Ddd.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nitrogen.Ddd.Domain.Repository
{
    /// <summary>
    /// 排序结果接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ISeqencingRepository<TEntity, TKey> : INgRepository
        where TEntity : class, IEntity<TKey>
    {
        
    }
}
