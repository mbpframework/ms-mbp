using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Domain
{
    /// <summary>
    /// 指示该实体支持审计
    /// </summary>
    public interface IAudited
    {
        
        Guid? LASTMODIFIER_ID { get; set; }

        DateTime? LASTMODIFICATION_TIME { get; set; }

        DateTime CREATION_TIME { get; set; }

        Guid? CREATOR_ID { get; set; }

        Guid? DELETER_ID { get; set; }

        DateTime? DELETION_TIME { get; set; }
    }
}
