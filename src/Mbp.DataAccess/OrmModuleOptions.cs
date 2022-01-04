using Mbp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.DataAccess
{
    /// <summary>
    /// Orm模块选项
    /// </summary>
    public class OrmModuleOptions
    {
        public Dictionary<string, DbConfig> DbConnections { get; } = new Dictionary<string, DbConfig>();

        public string DesDecryptKey { get; set; }
    }

    public class DbConfig
    {
        public string DbType { get; set; }

        public string Version { get; set; }

        public string ConnectionString { get; set; }

        public string TnsAdmin { get; set; }

        public string DbName { get; set; }
    }
}
