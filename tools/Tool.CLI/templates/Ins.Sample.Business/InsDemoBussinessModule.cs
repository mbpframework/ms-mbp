using Ins.Sample.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using WuhanIns.Nitrogen.Framework.Application;
using WuhanIns.Nitrogen.Modular;

namespace Ins.Sample.Business
{
    [DependsOn(typeof(InsDemoDataAccessModule))]
    public class InsDemoBussinessModule : NgFrameworkApplicationModule
    {
    }
}
