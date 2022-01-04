using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core
{
    /// <summary>
    /// 指示数据库更新冲突异常
    /// </summary>
    public class ConcurrentException : DbUpdateConcurrencyException
    {

    }
}
