using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mbp.DataAccess.EfCore.Interceptors
{
    /// <summary>
    /// 连接拦截
    /// </summary>
    public class MbpQueryConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public MbpQueryConnectionInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            base.ConnectionOpened(connection, eventData);
        }

        public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }

        public override void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
        {
            base.ConnectionClosed(connection, eventData);
        }

        public override Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
        {
            return base.ConnectionClosedAsync(connection, eventData);
        }
    }
}
