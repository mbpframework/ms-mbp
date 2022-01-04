using System;
using System.Collections.Generic;
using System.Text;

namespace Nitrogen.Orm.Dapper.Extensions
{
    /// <summary>
    /// Dp拦截方法
    /// </summary>
    public static class Dp
    {
        public static TProperty Property<TProperty>(object entity, string propertyName)
        => throw new InvalidOperationException("PropertyMethodInvoked");
    }
}
