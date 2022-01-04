using Mbp.DataAccess;
using Mbp.Framework.Domain.Do;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mbp.Framework.Domain
{
    public class DefaultDbContext : MbpDbContext<DefaultDbContext>
    {
        private IOptions<OrmModuleOptions> _ormOptions;

        public DefaultDbContext(DbContextOptions<DefaultDbContext> options, IOptions<OrmModuleOptions> ormOptions) : base(options)
        {
            _ormOptions = ormOptions;
        }

        public DbSet<SampleEntity> SampleEntities { get; set; }

        public DbSet<OracleEntity> OracleEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
