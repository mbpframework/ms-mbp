using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Uow
{
    public interface IUowDbContextFactory<out TDbCotnext>
        where TDbCotnext : DbContext
    {
        TDbCotnext GetDbContext();
    }
}
