using Mbp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core
{
    /// <summary>
    /// 业务提示异常类
    /// </summary>
    public class PromptingException : MbpException
    {
        public PromptingException(string message) : base(message)
        {

        }

        public PromptingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
