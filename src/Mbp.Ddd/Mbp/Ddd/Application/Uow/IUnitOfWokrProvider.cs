using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Uow
{
    public interface IUnitOfWokrProvider
    {
        IUnitOfWork GetCurrentUnitOfWork();
    }
}
