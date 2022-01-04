using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Extensions
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object GetValueTypeDefaultValue(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
