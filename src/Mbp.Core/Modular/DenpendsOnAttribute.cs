using System;

namespace Mbp.Modular
{
    /// <summary>
    /// 模块依赖关系注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute, IDependedTypesProvider
    {
        public DependsOnAttribute(params Type[] denpendsType)
        {
            DependedTypes = denpendsType ?? new Type[0];
        }

        /// <summary>
        /// 依赖类型
        /// </summary>
        public virtual Type[] DependedTypes { get; }

        public virtual Type[] GetDependedTypes()
        {
            return DependedTypes;
        }
    }
}
