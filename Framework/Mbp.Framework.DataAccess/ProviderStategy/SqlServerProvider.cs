using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mbp.DataAccess;

namespace Mbp.Framework.DataAccess.ProviderStategy
{
    public class SqlServerProvider : IDbProviderStategy
    {
        public DbContextOptionsBuilder UseMbpDb(DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig)
        {
            return dbContextOptionsBuilder.UseSqlServer(dbConfig.ConnectionString,
                        o =>
                        {
                            if (int.Parse(dbConfig.Version) <= 2008)
                            {
                                o.UseRowNumberForPaging();
                            }
                        });
        }
    }
}
