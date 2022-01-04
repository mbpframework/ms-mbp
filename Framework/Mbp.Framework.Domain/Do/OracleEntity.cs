using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Mbp.Ddd.Domain;

namespace Mbp.Framework.Domain.Do
{
    [Table("sample_user_t")]
    
    public class OracleEntity : EntityBase<string>, ISoftDelete
    {
        public string CODE { get; set; }

        public string NAME { get; set; }

        public string CUR_STATE { get; set; }

        public int DELETED { get; set; }
    }
}
