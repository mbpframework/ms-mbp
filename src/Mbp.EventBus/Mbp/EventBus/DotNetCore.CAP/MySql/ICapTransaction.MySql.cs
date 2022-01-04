// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP.Transport;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    public class MySqlCapTransaction : CapTransactionBase
    {
        public MySqlCapTransaction(
            IDispatcher dispatcher) : base(dispatcher)
        {
        }

        public override void Commit()
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    dbContextTransaction.Commit();
                    break;
            }
            Flush();
        }

        public override async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    await dbContextTransaction.CommitAsync(cancellationToken);
                    break;
            }
            Flush();
        }

        public override void Rollback()
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    dbContextTransaction.Rollback();
                    break;
            }
        }

        public override async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    await dbContextTransaction.RollbackAsync(cancellationToken);
                    break;
            }
        }

        public override void Dispose()
        {
            (DbTransaction as IDbTransaction)?.Dispose();
            DbTransaction = null;
        }
    }
}