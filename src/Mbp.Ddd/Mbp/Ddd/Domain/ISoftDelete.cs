using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Domain
{
    public interface ISoftDelete
    {
        int DELETED { get; set; }
    }
}
