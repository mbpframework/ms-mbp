using Microsoft.EntityFrameworkCore;
using Nitrogen.Ddd.Domain;
using Nitrogen.Ddd.Nitrogen.Ddd.Domain.Extensions;
using Nitrogen.Orm.Dapper.Extensions;
using Nitrogen.Web.User;
using Nitrogen.Web.User.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions
{
    /// <summary>
    /// SQL转换类
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    public class SqlConvertCore<TPrimary> //: ISqlConvertCore<TPrimary>
        where TPrimary : class, IEntity
    {
        // 暂存SQL片段
        private SqlStorage _sqlStorage = new SqlStorage();

        /// <summary>
        /// SQL字符串
        /// </summary>
        public string SqlString { get { return this._sqlStorage.ToString(); } }

        public ICurrentTenant CurrentTenant { get; private set; }

        protected virtual Guid? CurrentTenantId => CurrentTenant?.TenantId;

        public ICurrentUser CurrentUser { get; private set; }

        /// <summary>
        /// 数据库参数集合
        /// </summary>
        public Dictionary<string, object> DbParams { get { return this._sqlStorage.DbParams; } }

        public void SetDatabaseType(DatabaseType databaseType)
        {
            // 初始化数据库类型
            this._sqlStorage.DatabaseType = databaseType;
        }

        public void SetCurrentTenant(ICurrentTenant currentTenant) => CurrentTenant = currentTenant;

        public void SetCurrentUser(ICurrentUser currentUser) => CurrentUser = currentUser;

        /// <summary>
        /// 清理SQL片段
        /// </summary>
        public void Clear()
        {
            this._sqlStorage.Clear();
        }

        // Select函数转换SQL片段
        private string SelectParser()
        {
            _sqlStorage.Clear();
            _sqlStorage.IsSingleTable = false;
            _sqlStorage.CurrentDQL = DQL.Select;

            return "SELECT {0}\nFROM " + typeof(TPrimary).GetTableName();
        }

        // Select函数转换SQL片段
        private string SelectParser(string tableName)
        {
            _sqlStorage.Clear();
            _sqlStorage.IsSingleTable = false;
            _sqlStorage.CurrentDQL = DQL.Select;

            return "SELECT {0}\nFROM " + tableName;
        }
        public SqlConvertCore<TPrimary> Select(Expression<Func<TPrimary, object>> expression = null, string tableName = null)
        {
            string sql = string.IsNullOrEmpty(tableName) ? SelectParser() : SelectParser(tableName);

            if (expression == null)
            {
                _sqlStorage.Sql.AppendFormat(sql, (typeof(TPrimary)).GetTableColumns());
            }
            else
            {
                ExpressionToSqlProvider.Select(expression.Body, this._sqlStorage);
                _sqlStorage.Sql.AppendFormat(sql, this._sqlStorage.SelectFieldsStr);
            }

            return this;
        }

        /// <summary>
        /// Where函数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlConvertCore<TPrimary> Where(Expression<Func<TPrimary, bool>> expression, string tableName = null)
        {
            _sqlStorage += "\nWHERE";

            if (ShouldFilterEntity<TPrimary>())
            {
                var filterExpression = CreateFilterExpression<TPrimary>();
                if (filterExpression != null)
                {
                    ExpressionToSqlProvider.Where(filterExpression.Body, _sqlStorage);
                }
            }

            _sqlStorage += " AND";

            ExpressionToSqlProvider.Where(expression.Body, _sqlStorage);
            _sqlStorage.CurrentDQL = DQL.Where;
            return this;
        }

        public SqlConvertCore<TPrimary> Delete(string tableName = null)
        {
            this._sqlStorage.Clear();
            this._sqlStorage.IsSingleTable = true;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(TPrimary).GetTableName();
            }
            this._sqlStorage += "DELETE FROM" + tableName;
            return this;
        }

        public SqlConvertCore<TPrimary> Update(Expression<Func<object>> expression = null, string tableName = null)
        {
            this._sqlStorage.Clear();
            this._sqlStorage.IsSingleTable = true;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(TPrimary).GetTableName();
            }
            this._sqlStorage += "UPDATE " + tableName + " SET ";
            ExpressionToSqlProvider.Update(expression.Body, this._sqlStorage);
            return this;
        }

        protected virtual bool ShouldFilterEntity<TEntity>() where TEntity : class
        {
            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 升序排序
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlConvertCore<TPrimary> OrderBy(Expression<Func<TPrimary, object>> expression, string tableName = null)
        {
            if (_sqlStorage.CurrentDQL != DQL.OrderBy)
            {
                _sqlStorage += "\nORDER BY ";
                ExpressionToSqlProvider.OrderBy(expression.Body, _sqlStorage);
                _sqlStorage.CurrentDQL = DQL.OrderBy;
            }
            else
            {
                ExpressionToSqlProvider.OrderBy(expression.Body, _sqlStorage);
            }
            return this;
        }

        /// <summary>
        /// 降序排序
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlConvertCore<TPrimary> OrderByDescending(Expression<Func<TPrimary, object>> expression, string tableName = null)
        {
            if (_sqlStorage.CurrentDQL != DQL.OrderBy)
            {
                _sqlStorage += "\nORDER BY ";
                ExpressionToSqlProvider.OrderByDescending(expression.Body, _sqlStorage);
                _sqlStorage.CurrentDQL = DQL.OrderBy;
            }
            else
            {
                ExpressionToSqlProvider.OrderByDescending(expression.Body, _sqlStorage);
            }
            return this;
        }

        // 创建筛选器表达式
        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
           where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                expression = e => Dp.Property<bool>(e, "IS_DELETED") != true;
            }

            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> multiTenantFilter = e => Dp.Property<Guid>(e, "TENANT_ID") == CurrentTenantId;
                expression = expression == null ? multiTenantFilter : CombineExpressions(expression, multiTenantFilter);
            }

            return expression;
        }

        // 连接2个表达式
        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
