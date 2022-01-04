using Mbp.Modular;
using Mbp.Ddd;
using Mbp.Framework.Domain.Share;
using System;

namespace Mbp.Framework.Contracts
{
    [DependsOn(typeof(MbpFrameworkDomainShareModule), typeof(MbpDddModule))]
    public class MbpFrameworkApplicationContractsModule : MbpModule
    {
    }
}
