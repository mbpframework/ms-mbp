using Ins.Sample.DataAccess.Do;
using Microsoft.EntityFrameworkCore;

namespace Ins.Demo.DbMigration
{
    class Class1 : DbContext
    {
        public Class1()
        {

        }

        public DbSet<DemoEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("Server=localhost;Database=Nitrogen;User=root;Password=123456;", o =>
            {
                o.EnableRetryOnFailure();
            });
    }
}
