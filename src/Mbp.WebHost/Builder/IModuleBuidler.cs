using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbp.WebHost.Builder
{
    public interface IModuleBuidler
    {
        /// <summary>
        /// 构建业务开发脚手架
        /// </summary>
        /// <returns></returns>
        IModuleBuidler BuildApplicationFalsework();

        /// <summary>
        /// 构建Mbp脚手架
        /// </summary>
        /// <returns></returns>
        IModuleBuidler BuildMbpFalsework();
    }
}
