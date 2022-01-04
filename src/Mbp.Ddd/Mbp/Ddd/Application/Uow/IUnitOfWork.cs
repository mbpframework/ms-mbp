using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mbp.Ddd.Application.Uow
{
    public interface IUnitOfWork : ITransactionFeatureContainer, IDisposable
    {
        Guid ID { get; }

        bool IsDisposed { get; }

        UnitOfWorkOptions UnitOfWorkOptions { get; }

        public IServiceProvider ServiceProvider { get; }

        void SetOptions(UnitOfWorkOptions options);

        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken = default);

        event EventHandler<IUnitOfWork> DisposeHandler;

        void OnSaveChanged(Action action);

        void OnRollBacked(Action action);
    }
}
