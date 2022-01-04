using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mbp.Ddd.Domain.Aggregate
{
    /// <summary>
    /// 指示聚合根类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class AggregateBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey ID { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public DateTime CONCURRENCYSTAMP { get; set; }
    }
}
