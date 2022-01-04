using Mbp.Modular;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Internal.Extensions
{
    /// <summary>
    /// 重写对象比较器
    /// </summary>
    public class ModuleEqualityComparer : EqualityComparer<MbpModule>
    {
        public override bool Equals(MbpModule x, MbpModule y)
        {
            return (x == null && y == null) || (x != null && y != null && x.ModuleName == y.ModuleName);
        }

        public override int GetHashCode(MbpModule obj)
        {
            return obj == null ? 0 : obj.ModuleName.GetHashCode();
        }
    }
}
