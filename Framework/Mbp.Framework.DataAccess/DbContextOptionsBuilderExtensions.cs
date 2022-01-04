using Microsoft.EntityFrameworkCore;
using Mbp.Framework.DataAccess.ProviderStategy;
using Mbp.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Mbp.Framework.DataAccess
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMbpDb(this DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig, IServiceProvider serviceProvider)
        {
            return new ProviderRoute(dbConfig.DbType).UseMbpDb(dbContextOptionsBuilder, dbConfig, serviceProvider);
        }
    }
}
