using Ins.Sample.DataAccess.Do;
using Microsoft.EntityFrameworkCore;
using WuhanIns.Nitrogen.Framework.DataAccess;

namespace Ins.Sample.DataAccess
{
    public class SecondDbContext : NgDbContext<SecondDbContext>
    {
        public SecondDbContext(DbContextOptions<SecondDbContext> options) : base(options)
        {
        }

        public DbSet<DemoEntity> Books { get; set; }
    }
}
