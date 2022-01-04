using Microsoft.EntityFrameworkCore;
using Mbp.DataAccess;
using System;

namespace Mbp.DataAccess.DbProvider
{
    public class ProviderRoute
    {
        private IDbProviderStategy _dbProviderStategy = null;

        public ProviderRoute(string DbType)
        {
            switch (DbType)
            {
                case "SQL Server":
                    _dbProviderStategy = new SqlServerProvider();
                    break;
                case "MySql":
                    _dbProviderStategy = new MysqlProvider();
                    break;
                case "Oracle":
                    _dbProviderStategy = new OracleProvider();
                    break;
                default:
                    _dbProviderStategy = new SqlServerProvider();
                    break;
            }
        }

        public DbContextOptionsBuilder UseMbpDb(DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig, IServiceProvider serviceProvider)
        {
            return _dbProviderStategy.UseMbpDb(dbContextOptionsBuilder, dbConfig);
        }
    }
}
