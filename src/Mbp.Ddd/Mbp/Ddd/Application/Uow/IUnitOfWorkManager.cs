using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Uow
{
    public interface IUnitOfWorkManager : IUnitOfWokrProvider, IDisposable
    {
        /// <summary>
        /// Create a <see cref="IUnitOfWork"/> with a default options
        /// </summary>
        IUnitOfWork Create<TDbContext>(bool requiresNew = true) where TDbContext : DbContext;

        /// <summary>
        ///  Create a <see cref="IUnitOfWork"/> with a custom options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IUnitOfWork Create<TDbContext>(UnitOfWorkOptions options) where TDbContext : DbContext;
    }
}
