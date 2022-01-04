using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mbp.Ddd.Domain
{
    /// <summary>
    /// 指示实体类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey ID { get; set; }
    }
}
