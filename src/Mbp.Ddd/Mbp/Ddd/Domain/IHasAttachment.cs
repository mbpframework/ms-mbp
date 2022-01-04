using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Domain
{
    public interface IHasAttachment
    {
        Guid ATTACHMENT_RELATIVE { get; set; }
    }
}
