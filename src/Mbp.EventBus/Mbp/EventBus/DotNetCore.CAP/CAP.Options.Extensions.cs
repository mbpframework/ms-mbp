// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using DotNetCore.CAP;
using DotNetCore.CAP.MongoDB;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UseMySql(this CapOptions options, string connectionString)
        {
            return options.UseMySql(opt => { opt.ConnectionString = connectionString; });
        }

        public static CapOptions UseMySql(this CapOptions options, Action<MySqlOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure += x => x.Version = options.Version;

            options.RegisterExtension(new MySqlCapOptionsExtension(configure));

            return options;
        }

        public static CapOptions UseEntityFramework<TContext>(this CapOptions options)
            where TContext : DbContext
        {
            return options.UseEntityFramework<TContext>(opt => { });
        }

        public static CapOptions UseEntityFramework<TContext>(this CapOptions options, Action<EFOptions> configure)
            where TContext : DbContext
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new MySqlCapOptionsExtension(x =>
            {
                configure(x);
                x.DbContextType = typeof(TContext);
                x.Version = options.Version;
            }));

            return options;
        }

        public static CapOptions UseMongoDB(this CapOptions options)
        {
            return options.UseMongoDB(x => { });
        }

        public static CapOptions UseMongoDB(this CapOptions options, string connectionString)
        {
            return options.UseMongoDB(x => { x.DatabaseConnection = connectionString; });
        }

        public static CapOptions UseMongoDB(this CapOptions options, Action<MongoDBOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            configure += x => x.Version = options.Version;

            options.RegisterExtension(new MongoDBCapOptionsExtension(configure));

            return options;
        }
    }
}