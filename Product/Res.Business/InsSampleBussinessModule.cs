using Res.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Mbp.Framework.Application;
using Mbp.Modular;

namespace Res.Business
{
    [DependsOn(typeof(InsSampleDataAccessModule))]
    public class InsSampleBussinessModule : NgFrameworkApplicationModule
    {
    }
}
