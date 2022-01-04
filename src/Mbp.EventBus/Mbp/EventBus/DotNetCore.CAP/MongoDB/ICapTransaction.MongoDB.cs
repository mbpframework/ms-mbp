// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    public class MongoDBCapTransaction : CapTransactionBase
    {
        public MongoDBCapTransaction(IDispatcher dispatcher)
            : base(dispatcher)
        {
        }

        public override void Commit()
        {
            Debug.Assert(DbTransaction != null);

            if (DbTransaction is IClientSessionHandle session) session.CommitTransaction();

            Flush();
        }

        public override async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            if (DbTransaction is IClientSessionHandle session) await session.CommitTransactionAsync(cancellationToken);

            Flush();
        }

        public override void Rollback()
        {
            Debug.Assert(DbTransaction != null);

            if (DbTransaction is IClientSessionHandle session) session.AbortTransaction();
        }

        public override async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            if (DbTransaction is IClientSessionHandle session) await session.AbortTransactionAsync(cancellationToken);
        }

        public override void Dispose()
        {
            (DbTransaction as IClientSessionHandle)?.Dispose();
            DbTransaction = null;
        }
    }
}