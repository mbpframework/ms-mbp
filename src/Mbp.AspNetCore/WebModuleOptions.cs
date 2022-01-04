using Mbp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.AspNetCore
{
    /// <summary>
    /// Web模块选项
    /// </summary>
    public class WebModuleOptions
    {
        /// <summary>
        /// 允许访问第三方的域名
        /// </summary>
        public string AllowedHosts { get; set; }

        /// <summary>
        /// Web Api是否启用Restful风格
        /// </summary>
        public bool IsRestful { get; set; }

        /// <summary>
        /// 业务模块路径，提供给业务模块开发调试的时候方便找路径
        /// </summary>
        public Dictionary<string, string> ApplicationModulePath { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 认证服务地址
        /// </summary>
        public string IdentityServer { get; set; }

        /// <summary>
        /// 是否强烈要求HTTPS协议
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string RouteBasePath { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string RoutePrefix { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Title { get; set; } = "Mbp WebAPI";

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "v1";

        /// <summary>
        /// api 元数据信息
        /// </summary>
        public string Url { get; set; } = "/swagger/v1/swagger.json";

        /// <summary>
        /// Mime
        /// </summary>
        public Dictionary<string, string> Mimes { get; set; }
    }
}
