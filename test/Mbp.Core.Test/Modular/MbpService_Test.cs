using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core.Test.Modular
{
    public class MbpService_Test : IMbpService_Test
    {
        public string GetName()
        {
            return "Mbp Service Inject Test";
        }
    }
}
