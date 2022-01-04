using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mbp.Core;

namespace Mbp.DataAccess.EfCore.Interceptors
{
    /// <summary>
    /// 命令拦截
    /// </summary>
    public class MbpQueryCommandInterceptor : DbCommandInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMbpContextAccessor _MbpContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MbpQueryCommandInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _MbpContextAccessor = _serviceProvider.GetService<IMbpContextAccessor>();
            _httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();


            if (_MbpContextAccessor == null ||  _httpContextAccessor == null)
            {
                throw new MbpException($"程序运行异常：EF CORE生命周期内如何解析到应有的注册实例{nameof(_MbpContextAccessor)},{nameof(_httpContextAccessor)}");
            }

            // 代办：数据库拦截换日志拦截
            if (_MbpContextAccessor.MbpContext == null || _httpContextAccessor.HttpContext == null)
            {
                LoggerFactory.Create(c => { c.AddConsole(); }).CreateLogger<MbpQueryCommandInterceptor>().LogWarning("无法在当前上下文中，解析到httpcontext和Mbpcontext,拦截工作无法进行！");
            }
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            return result;
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(result);
        }

        // 命令创建开始
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            return base.CommandCreated(eventData, result);
        }

        // 执行结束，表示执行成功
        public override InterceptionResult DataReaderDisposing(DbCommand command, DataReaderDisposingEventData eventData, InterceptionResult result)
        {
            FinishSqlTransaction(eventData.CommandId, command.CommandText, command.Parameters, eventData.Duration, true);

            return base.DataReaderDisposing(command, eventData, result);
        }

        // 执行失败
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            FinishSqlTransaction(eventData.CommandId, command.CommandText, command.Parameters, eventData.Duration, false, eventData.Exception);

            base.CommandFailed(command, eventData);
        }

        // 执行失败
        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            FinishSqlTransaction(eventData.CommandId, command.CommandText, command.Parameters, eventData.Duration, false, eventData.Exception);

            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        private void FinishSqlTransaction(Guid commandId, string commandText, DbParameterCollection parmameters, TimeSpan duration, bool isSuccess = false, Exception exception = null)
        {
        }
    }
}
