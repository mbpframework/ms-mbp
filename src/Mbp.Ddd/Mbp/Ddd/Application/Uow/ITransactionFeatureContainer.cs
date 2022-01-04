using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Uow
{
    public interface ITransactionFeatureContainer
    {
        ITransactionFeature GetOrAddTransactionFeature(string key, ITransactionFeature TransactionFeature);

        ITransactionFeature GetTransactionFeature(string key);

        void RemoveTransaction(string key);
    }
}
