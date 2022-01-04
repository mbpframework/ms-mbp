using Mbp.DataAccess;
using Microsoft.EntityFrameworkCore;
using Res.DataAccess.Do;

namespace Res.DataAccess
{
    public class SecondDbContext : MbpDbContext<SecondDbContext>
    {
        public SecondDbContext(DbContextOptions<SecondDbContext> options) : base(options)
        {
        }

        public DbSet<SampleEntity> Books { get; set; }
    }
}
