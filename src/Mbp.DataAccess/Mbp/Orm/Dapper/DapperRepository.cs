using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WuhanIns.Nitrogen.Ddd.Application.UI;
using WuhanIns.Nitrogen.Ddd.Application.Uow;
using WuhanIns.Nitrogen.Ddd.Domain;
using WuhanIns.Nitrogen.Ddd.Domain.Repository;
using WuhanIns.Nitrogen.Orm.Dapper.Extensions;
using WuhanIns.Nitrogen.Orm.Dapper.Extensions.Expression2Sql;
using WuhanIns.Nitrogen.Web.User;
using WuhanIns.Nitrogen.Web.User.MultiTenant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WuhanIns.Nitrogen.Tracing;

namespace WuhanIns.Nitrogen.Orm.Dapper
{
    /// <summary>
    /// Dapper仓储特定实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    internal class DapperRepository<TEntity, TDbContext, TKey> : IDapperRepository
        <TEntity, TDbContext, TKey>
          where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
    {
        public IDbConnection DbConnection => _dbContextFactory.GetDbContext().Database.GetDbConnection();

        private readonly IUnitOfWorkManager _uowManager;
        private readonly IUowDbContextFactory<TDbContext> _dbContextFactory;
        private readonly Expression2SqlCore<TEntity> _sqlConvert = null;
        private readonly IOptions<OrmModuleOptions> _options;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;
        private readonly ITraceService _traceService;

        public DapperRepository(IUnitOfWorkManager uowManager, IOptions<OrmModuleOptions> options
            , ICurrentTenant currentTenant, ICurrentUser currentUser, ITraceService traceService)
        {
            _uowManager = uowManager;
            _options = options;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _traceService = traceService;

            _dbContextFactory = new UowDbContextFactory<TDbContext>(_uowManager);

            _sqlConvert = new Expression2SqlCore<TEntity>();
            _sqlConvert.SetCurrentTenant(_currentTenant);
            _sqlConvert.SetCurrentUser(_currentUser);

            var dbType = _options.Value.DbConnections[typeof(TDbContext).Name]?.DbType;
            if (dbType == "MySql")
                _sqlConvert.SetDatabaseType(DatabaseType.MySQL);
            else if (dbType == "SQL Server")
                _sqlConvert.SetDatabaseType(DatabaseType.SQLServer);
            else
                _sqlConvert.SetDatabaseType(DatabaseType.Oracle);
        }

        public TEntity Get(TKey Id, string tableName = null)
        {
            SetShardingName(tableName);

            return Get(Id);
        }

        private TEntity Get(TKey Id)
        {
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(o => o.ID.Equals(Id));

                commandId = BeginTrace(convert);
                var entity = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams).FirstOrDefault();
                FinishTrace(commandId, convert);

                return entity;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> func = null, string tableName = null)
        {
            SetShardingName(tableName);

            return Get(func);
        }

        private TEntity Get(Expression<Func<TEntity, bool>> func = null)
        {
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(func);

                commandId = BeginTrace(convert);
                var entity = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams).FirstOrDefault();
                FinishTrace(commandId, convert);

                return entity;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public IEnumerable<TEntity> GetAll(string tableName = null)
        {
            SetShardingName(tableName);
            return GetAll();
        }

        private IEnumerable<TEntity> GetAll()
        {
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(null);

                commandId = BeginTrace(convert);
                var list = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams);
                FinishTrace(commandId, convert);
                return list;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where = null, string tableName = null)
        {
            SetShardingName(tableName);
            return GetList(where);
        }

        private IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where = null)
        {
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(where);

                commandId = BeginTrace(convert);
                var list = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams);
                FinishTrace(commandId, convert);

                return list;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }

        }


        public IEnumerable<TEntity> GetListOrderBy(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null)
        {
            SetShardingName(tableName);
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(where);
                if (order != null)
                {
                    convert.OrderBy(order);
                }

                commandId = BeginTrace(convert);
                var list = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams);
                FinishTrace(commandId, convert);

                return list;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public IEnumerable<TEntity> GetListOrderByDescending(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null)
        {
            SetShardingName(tableName);
            Guid commandId = Guid.NewGuid();
            try
            {
                var convert = _sqlConvert.Select().Where(where);
                if (order != null)
                {
                    convert.OrderByDescending(order);
                }

                commandId = BeginTrace(convert);
                var list = DbConnection.Query<TEntity>(convert.SqlStr, convert.DbParams);
                FinishTrace(commandId, convert);

                return list;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public int Update(TEntity entity, string tableName = null)
        {
            Guid commandId = Guid.NewGuid();
            SetShardingName(tableName);
            try
            {
                var convert = _sqlConvert.Update(() => entity);

                commandId = BeginTrace(convert);
                var rows = DbConnection.Execute(_sqlConvert.SqlStr, _sqlConvert.DbParams);
                FinishTrace(commandId, convert);

                return rows;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public int Update(Expression<Func<TEntity>> expression, Expression<Func<TEntity, bool>> where, string tableName = null)
        {
            Guid commandId = Guid.NewGuid();
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            SetShardingName(tableName);
            try
            {
                var convert = _sqlConvert.Update(expression).Where(where);
                commandId = BeginTrace(convert);
                var rows = DbConnection.Execute(_sqlConvert.SqlStr, _sqlConvert.DbParams);
                FinishTrace(commandId, convert);

                return rows;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public int Insert(TEntity entity, string tableName = null)
        {

            Guid commandId = Guid.NewGuid();
            SetShardingName(tableName);
            try
            {
                var convert = _sqlConvert.Insert(() => entity);

                commandId = BeginTrace(convert);
                var rows = DbConnection.Execute(_sqlConvert.SqlStr, _sqlConvert.DbParams);
                FinishTrace(commandId, convert);

                return rows;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public int Delete(TEntity entity, string tableName = null)
        {
            Guid commandId = Guid.NewGuid();
            SetShardingName(tableName);
            try
            {
                var convert = _sqlConvert.Delete(entity);

                commandId = BeginTrace(convert);
                var rows = DbConnection.Execute(_sqlConvert.SqlStr, _sqlConvert.DbParams);
                FinishTrace(commandId, convert);

                return rows;
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        public (IEnumerable<TEntity>, int, int) PagingQueryTradition(Expression<Func<TEntity, object>> selectColumns, Expression<Func<TEntity, bool>> whereSql, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, string tableName = null)
        {
            Guid commandId = Guid.NewGuid();
            SetShardingName(tableName);
            try
            {
                var sqlMain = _sqlConvert.PagingQuery(selectColumns, whereSql, pageIndex, pageSize, orderBy).SqlStr;

                commandId = BeginTrace(_sqlConvert);
                // 查询数据
                var query = DbConnection.QueryMultiple(sqlMain, _sqlConvert.DbParams);
                var data = query.Read<TEntity>();
                // 总条数
                int pagecount = query.Read<Page>().First().PageCount;

                // 总页数
                int total = pagecount / pageSize;

                if (pagecount % pageSize > 0)
                    total++;

                FinishTrace(commandId, sqlMain, _sqlConvert.DbParams);

                return (data, pagecount, total);
            }
            catch (Exception ex)
            {
                ExcuteFail(commandId, ex);
                throw;
            }
        }

        private void SetShardingName(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
                _sqlConvert.SetShardingName(tableName);
        }

        private Guid BeginTrace(Expression2SqlCore<TEntity> convert)
        {
            if (_traceService.IsOpen && (_traceService.TraceMode & TraceMode.Sql) > 0)
            {
                var commandId = Guid.NewGuid();
                var sqlCommand = _traceService.CreateSqlTransaction(commandId);
                sqlCommand.EndTime = DateTime.Now;
                sqlCommand.SQL = convert.SqlStr;
                sqlCommand.IsSuccess = true;
                sqlCommand.OrmFramework = "Dapper";

                return commandId;
            }
            // 不跟踪的时候将跟踪命令ID置空 
            return Guid.Empty;
        }

        private void FinishTrace(Guid commandId, Expression2SqlCore<TEntity> convert)
        {
            // 跟踪命令ID为空的时候不跟踪
            if (commandId == Guid.Empty) return;

            var sqlCommand = _traceService.GetSqlCommand(commandId);

            sqlCommand.EndTime = DateTime.Now;
            if (_traceService.IsOpen && (_traceService.TraceMode & TraceMode.Duration) > 0)
            {
                sqlCommand.Duration = (sqlCommand.EndTime - sqlCommand.BeginTime).TotalMilliseconds;
            }
            sqlCommand.Parameters = convert.DbParams;

            _traceService.CommitTransaction(commandId);
        }

        private void FinishTrace(Guid commandId, string sql, Dictionary<string, object> parameters)
        {
            // 跟踪命令ID为空的时候不跟踪
            if (commandId == Guid.Empty) return;

            var sqlCommand = _traceService.GetSqlCommand(commandId);

            sqlCommand.EndTime = DateTime.Now;
            if (_traceService.IsOpen && (_traceService.TraceMode & TraceMode.Duration) > 0)
            {
                sqlCommand.Duration = (sqlCommand.EndTime - sqlCommand.BeginTime).TotalMilliseconds;
            }
            sqlCommand.Parameters = parameters;

            _traceService.CommitTransaction(commandId);
        }

        private void ExcuteFail(Guid commandId, Exception ex)
        {
            var sqlCommand = _traceService.GetSqlCommand(commandId);
            sqlCommand.Error = ex.Message;
            sqlCommand.IsSuccess = false;
            _traceService.CommitTransaction(commandId);
        }
    }
}
