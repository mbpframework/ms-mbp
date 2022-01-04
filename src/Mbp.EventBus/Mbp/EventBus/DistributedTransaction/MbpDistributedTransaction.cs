using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Mbp.EventBus;

namespace Mbp.EventBus.DistributedTransaction
{
    /// <summary>
    /// Mbp分布式事务解决方案
    /// </summary>
    internal class MbpDistributedTransaction : IMbpDistributedTransaction
    {
        private readonly ICapPublisher _capBus;

        public MbpDistributedTransaction(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        public void ExecDistributedTransaction(DatabaseFacade database, Action action)
        {
            using (var trans = database.BeginTransaction(_capBus, autoCommit: false))
            {
                action();

                trans.Commit();
            }
        }
    }
}
