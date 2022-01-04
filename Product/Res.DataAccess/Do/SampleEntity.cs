using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mbp.Ddd.Domain;
using Mbp.Ddd.Domain.Aggregate;

namespace Res.DataAccess.Do
{
    [Table("SAMPLE_USER")]
    public class SampleEntity : EntityBase<string>, ISoftDelete
    {
        public string CODE { get; set; }

        public string NAME { get; set; }

        public string CUR_STATE { get; set; }

        public int DELETED { get; set; }
    }
}
