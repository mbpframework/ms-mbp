using Mbp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Net
{
    internal class InnerServiceErrorException : MbpException
    {
        public InnerServiceErrorException(string message) : base(message)
        {
        }
    }
}
