using System.Collections.Generic;
using Mbp.Utils;

namespace Mbp.Configuration
{
    public class ProductSetting
    {
        public Dictionary<string, ProductBaseConfig> ProductConfigs { get; } = new Dictionary<string, ProductBaseConfig>();
    }

    public class ProductBaseConfig
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string[] Filters { get; set; }
    }
}
