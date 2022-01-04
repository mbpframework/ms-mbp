using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Mbp.DataAccess;

namespace Mbp.Framework.DataAccess.ProviderStategy
{
    public class OracleProvider : IDbProviderStategy
    {
        public DbContextOptionsBuilder UseMbpDb(DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig)
        {
            return dbContextOptionsBuilder.UseOracle(dbConfig.ConnectionString, options =>
            {
                options.UseOracleSQLCompatibility(dbConfig.Version);
            });
        }
    }
}
