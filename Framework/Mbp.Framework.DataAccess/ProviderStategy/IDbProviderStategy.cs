using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mbp.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Framework.DataAccess.ProviderStategy
{
    /// <summary>
    /// 切换不同数据库类型的策略
    /// </summary>
    public interface IDbProviderStategy
    {
        // 数据库类型和版本
        DbContextOptionsBuilder UseMbpDb(DbContextOptionsBuilder dbContextOptionsBuilder, DbConfig dbConfig);

    }
}
