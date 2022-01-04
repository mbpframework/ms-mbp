using Mbp.DataAccess;
using Mbp.DataAccess.DbProvider;
using Microsoft.EntityFrameworkCore;
using System;

namespace Mbp.DataAccess
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMbpDb(this DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig, IServiceProvider serviceProvider)
        {
            return new ProviderRoute(dbConfig.DbType).UseMbpDb(dbContextOptionsBuilder, dbConfig, serviceProvider);
        }
    }
}
