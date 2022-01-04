using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mbp.Internal.Extensions;

namespace Mbp.Modular
{
    /// <summary>
    /// 模块抽象基类
    /// </summary>
    public abstract class MbpModule : IMbpModule
    {
        /// <summary>
        /// 模块级别,为了安全,其他模块不定义Core
        /// </summary>
        public virtual EnumModuleLevel Level => EnumModuleLevel.Application;

        /// <summary>
        /// 模块名称
        /// </summary>
        public virtual string ModuleName => this.GetType().Name;

        public List<MbpModule> Dependencies { get; set; } = new List<MbpModule>();

        public virtual IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// 支持.net core自带的依赖注入方式
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// 使用模块,主要应用于AspNetCore环境的
        /// </summary>
        /// <param name="provider"></param>
        public virtual void OnModuleInitialization(IServiceProvider provider)
        {

        }

        /// <summary>
        /// 模块初始化前
        /// </summary>
        /// <param name="provider"></param>
        public virtual void OnPreModuleInitialization(IServiceProvider provider)
        {
        }

        /// <summary>
        /// 获取当前模块的依赖模块类型
        /// </summary>
        /// <returns></returns>
        public Type[] FindDependedModuleTypes(Type moduleType = null)
        {
            // 使用调用者的类型
            if (moduleType == null)
            {
                moduleType = GetType();
            }

            List<Type> dependTypes = new List<Type>();

            // 查找依赖特性
            var attributes = moduleType.GetCustomAttributes().OfType<IDependedTypesProvider>();

            // 遍历依赖特性找到依赖模块类型
            foreach (var attribute in attributes)
            {
                foreach (var type in attribute.GetDependedTypes())
                {
                    if (type.IsAbstract) continue;
                    dependTypes.AddIfNotExist(type);
                }
            }

            // 去重后返回
            return dependTypes.Distinct().ToArray();
        }
    }
}
