using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mbp.DataAccess;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Mbp.Framework.DataAccess.ProviderStategy
{
    public class MysqlProvider : IDbProviderStategy
    {
        public DbContextOptionsBuilder UseMbpDb(DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig)
        {
            return dbContextOptionsBuilder.UseMySql(dbConfig.ConnectionString,
                mySqlOptions =>
                {
                    mySqlOptions
                       .ServerVersion(new ServerVersion(new Version(dbConfig.Version), ServerType.MySql));
                });
        }
    }
}
