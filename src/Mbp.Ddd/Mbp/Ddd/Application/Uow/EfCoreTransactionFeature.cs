using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mbp.Ddd.Application.Uow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mbp.Ddd.Application.Uow
{
    public class EfCoreTransactionFeature : ITransactionFeature
    {
        public bool IsCommit { get; private set; }
        public bool IsRollback { get; private set; }
        public bool IsOpenTransaction => _isOpenTransaction;

        private bool _isOpenTransaction;
        private bool _isDispose;
        private IDbContextTransaction _dbContextTransaction;
        private DbContext _dbContext;

        public EfCoreTransactionFeature(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SetTransaction(IDbContextTransaction dbContextTransaction)
        {
            if (_isOpenTransaction)
                throw new Exception("this transaction feature is already set transaction!");

            _isOpenTransaction = true;
            _dbContextTransaction = dbContextTransaction;
        }

        public void Commit()
        {
            if (IsCommit)
                return;

            IsCommit = true;

            _dbContext.SaveChanges();
            _dbContextTransaction?.Commit();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (IsCommit)
                return;

            IsCommit = true;

            await _dbContext.SaveChangesAsync();
            await _dbContextTransaction?.CommitAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (_isDispose)
                return;

            _isDispose = true;

            _dbContextTransaction?.Dispose();
            _dbContext?.Dispose();
        }

        public void Rollback()
        {
            if (IsRollback)
                return;

            IsRollback = true;

            _dbContextTransaction?.Rollback();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (IsRollback)
                return;

            IsRollback = true;

            await _dbContextTransaction?.RollbackAsync(cancellationToken);
        }
    }
}
