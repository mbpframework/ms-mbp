using Ins.Sample.DataAccess.Do;
using Microsoft.EntityFrameworkCore;
using WuhanIns.Nitrogen.Framework.DataAccess;

namespace Ins.Sample.DataAccess
{
    public class DefaultDbContext : NgDbContext<DefaultDbContext>
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }

        public DbSet<DemoEntity>   DemoEntities { get; set; }
    }
}
