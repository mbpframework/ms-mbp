using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Dto
{
    /// <summary>
    /// 指示Dto中是否包含附件信息
    /// </summary>
    public interface IHasAttachmentDto
    {
        Guid AttachmentRelative { get; set; }

        string AttachmentName { get; set; }

        string AttachementUrl { get; set; }
    }
}
