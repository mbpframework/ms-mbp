using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mbp.Configuration
{
    /// <summary>
    /// 动态加载配置组件
    /// </summary>
    public class ConfigProviderFactory
    {
        public IConfigProvider Create(string provider)
        {
            (string assembly, string type) = ParseProvider(provider);

            return Activator.CreateInstance(assembly, type).Unwrap() as IConfigProvider;
        }

        private (string, string) ParseProvider(string provider)
        {
            if (string.IsNullOrEmpty(provider))
                throw new ArgumentNullException(nameof(provider));

            var arr = provider.Split(":");
            if (arr.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(provider));

            return (arr[0], arr[1]);
        }
    }
}
