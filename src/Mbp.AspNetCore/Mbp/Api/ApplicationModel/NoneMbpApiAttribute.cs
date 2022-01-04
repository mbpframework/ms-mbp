using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.AspNetCore.Api
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class NoneMbpApiAttribute : Attribute
    {
    }
}
