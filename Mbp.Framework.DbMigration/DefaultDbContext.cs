using Mbp.Framework.DbMigration.Do;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Framework.DbMigration
{
    internal class DefaultDbContext : DbContext
    {
        public DbSet<OracleEntity> OracleEntities { get; set; }

        public DbSet<SampleEntity> SampleEntities { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("Server=localhost;Database=mbpcore;User=root;Password=uihimaging;IgnoreCommandTransaction=true;Min Pool Size=2;Max Pool Size=50;", new MySqlServerVersion(new Version("8.0.18")));
    }
}
