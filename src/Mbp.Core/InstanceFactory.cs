using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core
{
    public sealed class InstanceFactory
    {
        public T Create<T>(string provider) where T : class
        {
            (string assembly, string type) = ParseProvider(provider);

            return (T)Activator.CreateInstance(assembly, type).Unwrap();
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
