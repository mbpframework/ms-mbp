using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;

namespace Mbp
{
    /// <summary>
    /// 基于ASPNETCORE环境的抽象模块
    /// </summary>
    public abstract class MbpAspNetModule : MbpModule
    {
        /// <summary>
        /// 应用AspNetCore的服务业务
        /// </summary>
        /// <param name="app">AspNetCore应用程序构建器</param>
        public virtual void OnModuleInitialization(IApplicationBuilder app)
        {
        }
    }
}
