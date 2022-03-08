using System;
using System.ComponentModel.DataAnnotations.Schema;
using WuhanIns.Nitrogen.Ddd.Domain;
using WuhanIns.Nitrogen.Ddd.Domain.Aggregate;

namespace Ins.Sample.DataAccess.Do
{
    [Table("SYS_USER")]
    public class DemoEntity : AggregateBase<Guid>, ISoftDelete
    {
        public string CODE { get; set; }

        public string NAME { get; set; }

        public string CUR_STATE { get; set; }

        public bool IS_DELETED { get; set; }
    }
}
