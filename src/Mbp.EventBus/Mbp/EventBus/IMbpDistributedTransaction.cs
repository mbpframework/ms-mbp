using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Mbp.EventBus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMbpDistributedTransaction
    {
        void ExecDistributedTransaction(DatabaseFacade database, Action action);
    }
}
