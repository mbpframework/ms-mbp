using Mbp.Core.User.MultiTenant;
using Mbp.Ddd.Domain;
using Mbp.Ddd.Domain.Aggregate;
using Microsoft.EntityFrameworkCore;
using Res.DataAccess.Do;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ins.Demo.DbMigration
{
    class Class1 : DbContext
    {
        public Class1()
        {

        }

        public DbSet<SampleEntity> Users { get; set; }

        public DbSet<UserAggregate_Test>  UserAggregates { get; set; }

        public DbSet<BookEntity_Test>  BookEntities { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseMySql("Server=172.18.35.34;Database=Mbp;User=instest;Password=instest;IgnoreCommandTransaction=true;Min Pool Size=2;Max Pool Size=50;", o =>
        //    {
        //        o.EnableRetryOnFailure();
        //    });
    }

    [Table("User")]
    public class UserAggregate_Test : AggregateBase<Guid>, ISoftDelete, IMultiTenant
    {
        public string USER_NAME { get; set; }

        public string USER_CODE { get; set; }

        public int AGE { get; set; }

        public string USER_POSITION { get; set; }

        public List<BookEntity_Test> BOOKS { get; set; }

        public Guid? TENANT_ID { get; set; }

        public int DELETED { get; set; }
    }

    /// <summary>
    /// 示例程序 实体
    /// </summary>
    [Table("Book")]
    public class BookEntity_Test : EntityBase<Guid>, ISoftDelete, IMultiTenant
    {
        public string BOOK_NAME { get; set; }

        public string BOOK_CODE { get; set; }

        public decimal BOOK_PRICE { get; set; }

        public Guid? TENANT_ID { get; set; }

        public int DELETED { get; set; }
    }
}
