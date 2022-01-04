using Mbp.DataAccess;
using Mbp.Framework.Domain.Do;
using Microsoft.EntityFrameworkCore;

namespace Mbp.Framework.Domain
{
    public class SecondDbContext : MbpDbContext<SecondDbContext>
    {
        public SecondDbContext(DbContextOptions<SecondDbContext> options) : base(options)
        {
        }

        public DbSet<SampleEntity> Books { get; set; }
    }
}
