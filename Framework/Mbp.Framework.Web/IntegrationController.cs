using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mbp.Discovery;
using Microsoft.Extensions.Configuration;

namespace Mbp.Framework.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IMbpDiscovery _ngDiscovery;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRoot _configurationRoot;

        public IntegrationController(IMbpDiscovery ngDiscovery, IConfiguration configuration)
        {
            _ngDiscovery = ngDiscovery;
            _configuration = configuration;
            _configurationRoot = (IConfigurationRoot)configuration;
        }

        /// <summary>
        /// 测试服务发现，注册中心为系统平台
        /// </summary>
        /// <param name="serviceName">服务名，对应系统平台，应用编号，单实例</param>
        /// <returns></returns>
        [HttpGet("{serviceName}")]

        public virtual string TestDiscoveryByMgmt(string serviceName)
        {
            return _ngDiscovery.GetWebApiServiceUrl(serviceName);
        }


        [HttpGet]
        public virtual string ReadConig()
        {
            foreach (var item in _configurationRoot.AsEnumerable())
            {
                Console.WriteLine($"{item.Key}----------{item.Value}");
            }

            string str = "";
            foreach (var provider in _configurationRoot.Providers.ToList())
            {
                str += provider.ToString() + "\n";
            }

            return (str);
        }
    }
}
