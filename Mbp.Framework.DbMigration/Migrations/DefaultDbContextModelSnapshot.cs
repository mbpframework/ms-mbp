﻿// <auto-generated />
using Mbp.Framework.DbMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mbp.Framework.DbMigration.Migrations
{
    [DbContext(typeof(DefaultDbContext))]
    partial class DefaultDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Mbp.Framework.DbMigration.Do.OracleEntity", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CODE")
                        .HasColumnType("longtext");

                    b.Property<string>("CUR_STATE")
                        .HasColumnType("longtext");

                    b.Property<int>("DELETED")
                        .HasColumnType("int");

                    b.Property<string>("NAME")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("sample_user_t");
                });

            modelBuilder.Entity("Mbp.Framework.DbMigration.Do.SampleEntity", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CODE")
                        .HasColumnType("longtext");

                    b.Property<string>("CUR_STATE")
                        .HasColumnType("longtext");

                    b.Property<int>("DELETED")
                        .HasColumnType("int");

                    b.Property<string>("NAME")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("SAMPLE_USER");
                });
#pragma warning restore 612, 618
        }
    }
}
