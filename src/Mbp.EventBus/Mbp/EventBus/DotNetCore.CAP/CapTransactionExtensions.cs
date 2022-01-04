using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Data;

namespace DotNetCore.CAP
{
    public static class CapTransactionExtensions
    {
        public static ICapTransaction Begin(this ICapTransaction transaction,
               IClientSessionHandle dbTransaction, bool autoCommit = false)
        {
            if (!dbTransaction.IsInTransaction) dbTransaction.StartTransaction();

            transaction.DbTransaction = dbTransaction;
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

        /// <summary>
        /// Start the CAP transaction
        /// </summary>
        /// <param name="client">The <see cref="IMongoClient" />.</param>
        /// <param name="publisher">The <see cref="ICapPublisher" />.</param>
        /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
        /// <returns>The <see cref="IClientSessionHandle" /> of MongoDB transaction session object.</returns>
        public static IClientSessionHandle StartTransaction(this IMongoClient client,
            ICapPublisher publisher, bool autoCommit = false)
        {
            var clientSessionHandle = client.StartSession();
            publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
            var capTrans = publisher.Transaction.Value.Begin(clientSessionHandle, autoCommit);
            return new CapMongoDbClientSessionHandle(capTrans);
        }

        public static ICapTransaction Begin(this ICapTransaction transaction,
            IDbContextTransaction dbTransaction, bool autoCommit = false)
        {
            transaction.DbTransaction = dbTransaction;
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

        public static ICapTransaction Begin(this ICapTransaction transaction,
            IDbTransaction dbTransaction, bool autoCommit = false)
        {
            transaction.DbTransaction = dbTransaction;
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

        /// <summary>
        /// Start the CAP transaction
        /// </summary>
        /// <param name="database">The <see cref="DatabaseFacade" />.</param>
        /// <param name="publisher">The <see cref="ICapPublisher" />.</param>
        /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
        /// <returns>The <see cref="IDbContextTransaction" /> of EF dbcontext transaction object.</returns>
        public static IDbContextTransaction BeginTransaction(this DatabaseFacade database,
            ICapPublisher publisher, bool autoCommit = false)
        {
            var trans = database.BeginTransaction();
            publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
            var capTrans = publisher.Transaction.Value.Begin(trans, autoCommit);
            return new CapEFDbTransaction(capTrans);
        }

        /// <summary>
        /// Start the CAP transaction
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection" />.</param>
        /// <param name="publisher">The <see cref="ICapPublisher" />.</param>
        /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
        /// <returns>The <see cref="ICapTransaction" /> object.</returns>
        public static ICapTransaction BeginTransaction(this IDbConnection dbConnection,
            ICapPublisher publisher, bool autoCommit = false)
        {
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }

            var dbTransaction = dbConnection.BeginTransaction();
            publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
            return publisher.Transaction.Value.Begin(dbTransaction, autoCommit);
        }
    }
}
