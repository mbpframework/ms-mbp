#region License
/**
 * Copyright (c) 2015, 何志祥 (strangecity@qq.com).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * without warranties or conditions of any kind, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using WuhanIns.Nitrogen.Ddd.Domain;
using WuhanIns.Nitrogen.Ddd.Nitrogen.Ddd.Domain.Extensions;
using WuhanIns.Nitrogen.Web.User;
using WuhanIns.Nitrogen.Web.User.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Expression2Sql
{
    public class Expression2SqlCore<T>
        where T : class, IEntity
    {
        private SqlPack _sqlPack = new SqlPack();

        public ICurrentTenant CurrentTenant { get; private set; }

        protected virtual Guid? CurrentTenantId => CurrentTenant?.TenantId;

        public ICurrentUser CurrentUser { get; private set; }

        public string ShardingName { get; private set; }

        public string SqlStr { get { return _sqlPack.ToString(); } }
        public Dictionary<string, object> DbParams { get { return _sqlPack.DbParams; } }

        public Expression2SqlCore()
        {
            PropertyInfoCache.InitCacheInfo<T>();
        }

        public void SetDatabaseType(DatabaseType databaseType)
        {
            // 初始化数据库类型
            _sqlPack.DatabaseType = databaseType;
        }

        public void SetCurrentTenant(ICurrentTenant currentTenant) => CurrentTenant = currentTenant;

        public void SetCurrentUser(ICurrentUser currentUser) => CurrentUser = currentUser;

        public void SetShardingName(string shardingName) => ShardingName = shardingName;

        public void Clear()
        {
            _sqlPack.Clear();
        }

        private string SelectParser(params Type[] ary)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = false;

            foreach (var item in ary)
            {
                string tableName = typeof(T).GetTableName();
                _sqlPack.SetTableAlias(tableName);
            }

            // shardingName只针对当前类型
            if (!string.IsNullOrEmpty(ShardingName))
                return "select {0}\nfrom " + ShardingName + " " + _sqlPack.GetTableAlias(typeof(T).GetTableName());

            return "select {0}\nfrom " + typeof(T).GetTableName() + " " + _sqlPack.GetTableAlias(typeof(T).GetTableName());
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression2SqlCore<T> Select(Expression<Func<T, object>> expression = null)
        {
            string sql = SelectParser(typeof(T));

            if (expression == null)
            {
                var fields = typeof(T).GetTableColumns(_sqlPack.GetTableAlias(typeof(T).GetTableName()));
                _sqlPack.Sql.AppendFormat(sql, fields);
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2>(Expression<Func<T, T2, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            string sql = SelectParser(typeof(T), typeof(T2));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3>(Expression<Func<T, T2, T3, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();
            PropertyInfoCache.InitCacheInfo<T6>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();
            PropertyInfoCache.InitCacheInfo<T6>();
            PropertyInfoCache.InitCacheInfo<T7>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();
            PropertyInfoCache.InitCacheInfo<T6>();
            PropertyInfoCache.InitCacheInfo<T7>();
            PropertyInfoCache.InitCacheInfo<T8>();
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();
            PropertyInfoCache.InitCacheInfo<T6>();
            PropertyInfoCache.InitCacheInfo<T7>();
            PropertyInfoCache.InitCacheInfo<T8>();
            PropertyInfoCache.InitCacheInfo<T9>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public Expression2SqlCore<T> Select<T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression = null)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            PropertyInfoCache.InitCacheInfo<T4>();
            PropertyInfoCache.InitCacheInfo<T5>();
            PropertyInfoCache.InitCacheInfo<T6>();
            PropertyInfoCache.InitCacheInfo<T7>();
            PropertyInfoCache.InitCacheInfo<T8>();
            PropertyInfoCache.InitCacheInfo<T9>();
            PropertyInfoCache.InitCacheInfo<T10>();

            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));

            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                Expression2SqlProvider.Select(expression.Body, _sqlPack);
                _sqlPack.Sql.AppendFormat(sql, _sqlPack.SelectFieldsStr);
            }

            return this;
        }

        public Expression2SqlCore<T> PagingQuery(Expression<Func<T, object>> selectColumns, Expression<Func<T, bool>> whereSql, int pageIndex, int pageSize, Expression<Func<T, object>> orderBy)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = false;

            string tableName = !string.IsNullOrEmpty(ShardingName) ? ShardingName : typeof(T).GetTableName();


            _sqlPack.SetTableAlias(typeof(T).GetTableName());
            string tableAlias = _sqlPack.GetTableAlias(typeof(T).GetTableName());

            foreach (Expression item in (selectColumns.Body as NewExpression).Arguments)
            {
                if (item is MemberExpression)
                {
                    _sqlPack.SelectFields.Add(tableAlias + "." + (item as MemberExpression).Member.Name);
                }
            }

            var sqlMain = "SELECT {0}\nFROM " + tableName + " " + tableAlias + " INNER JOIN";
            sqlMain += " (SELECT " + tableAlias + ".ID FROM " + tableName + " " + tableAlias;

            var selectColumnsStr = _sqlPack.SelectFieldsStr;

            if (whereSql != null)
            {
                sqlMain += " WHERE {1}";
            }
            if (orderBy != null)
            {
                sqlMain += " ORDER BY {2} ASC";
            }
            sqlMain += $" LIMIT {(pageIndex - 1) * 5}, {pageSize}) main using(ID);";

            var whereStr = string.Empty;
            SqlPack sqlPack = new SqlPack();
            if (whereSql != null)
            {
                sqlPack.SetTableAlias(typeof(T).GetTableName());
                sqlPack.IsSingleTable = false;
                Expression2SqlProvider.Where(whereSql.Body, sqlPack);
                whereStr = sqlPack.Sql.ToString();
            }

            var orderbyStr = string.Empty;
            if (orderBy != null)
            {
                orderbyStr += tableAlias + "." + (orderBy.Body as MemberExpression).Member.Name;
            }

            var parameters = sqlPack.DbParams;
            foreach (var parameter in parameters)
            {
                _sqlPack.DbParams.Add(parameter.Key, parameter.Value);
            }
            _sqlPack.Sql.Clear();

            _sqlPack.Sql.AppendFormat(sqlMain, selectColumnsStr, whereStr, orderbyStr);
            _sqlPack += "SELECT count(*) PageCount FROM " + tableName + " " + tableAlias;

            return this;
        }

        private Expression2SqlCore<T> JoinParser<T2>(Expression<Func<T, T2, bool>> expression, string leftOrRightJoin = "")
        {
            PropertyInfoCache.InitCacheInfo<T2>();

            string joinTableName = PropertyInfoCache.GetTableName(typeof(T2).FullName);// typeof(T2).Name;
            _sqlPack.SetTableAlias(joinTableName);
            _sqlPack.Sql.AppendFormat("\n{0}join {1} on", leftOrRightJoin, joinTableName + " " + _sqlPack.GetTableAlias(joinTableName));
            Expression2SqlProvider.Join(expression.Body, _sqlPack);
            return this;
        }
        private Expression2SqlCore<T> JoinParser2<T2, T3>(Expression<Func<T2, T3, bool>> expression, string leftOrRightJoin = "")
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            string joinTableName = PropertyInfoCache.GetTableName(typeof(T3).FullName);// typeof(T3).Name;
            _sqlPack.SetTableAlias(joinTableName);
            _sqlPack.Sql.AppendFormat("\n{0}join {1} on", leftOrRightJoin, joinTableName + " " + _sqlPack.GetTableAlias(joinTableName));
            Expression2SqlProvider.Join(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            return JoinParser(expression);
        }
        public Expression2SqlCore<T> Join<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            return JoinParser2(expression);
        }

        public Expression2SqlCore<T> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            return JoinParser(expression, "inner ");
        }
        public Expression2SqlCore<T> InnerJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            return JoinParser2(expression, "inner ");
        }

        public Expression2SqlCore<T> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            return JoinParser(expression, "left ");
        }
        public Expression2SqlCore<T> LeftJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            return JoinParser2(expression, "left ");
        }

        public Expression2SqlCore<T> RightJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            return JoinParser(expression, "right ");
        }
        public Expression2SqlCore<T> RightJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            return JoinParser2(expression, "right ");
        }

        public Expression2SqlCore<T> FullJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            return JoinParser(expression, "full ");
        }
        public Expression2SqlCore<T> FullJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            PropertyInfoCache.InitCacheInfo<T2>();
            PropertyInfoCache.InitCacheInfo<T3>();
            return JoinParser2(expression, "full ");
        }

        public Expression2SqlCore<T> Where(Expression<Func<T, bool>> expression)
        {
            _sqlPack += "\nWHERE";

            if (ShouldFilterEntity<T>())
            {
                var filterExpression = CreateFilterExpression<T>();
                if (filterExpression != null)
                {
                    Expression2SqlProvider.Where(filterExpression.Body, _sqlPack);
                }
            }

            if (expression != null)
            {
                _sqlPack += " AND";
                Expression2SqlProvider.Where(expression.Body, _sqlPack);
            }

            return this;
        }

        public Expression2SqlCore<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _sqlPack += "\ngroup by ";
            Expression2SqlProvider.GroupBy(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> OrderBy(Expression<Func<T, object>> expression)
        {
            _sqlPack += "\norder by ";
            Expression2SqlProvider.OrderBy(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            _sqlPack += "\norder by ";
            Expression2SqlProvider.OrderByDescending(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> Max(Expression<Func<T, object>> expression)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            Expression2SqlProvider.Max(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> Min(Expression<Func<T, object>> expression)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            Expression2SqlProvider.Min(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> Avg(Expression<Func<T, object>> expression)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            Expression2SqlProvider.Avg(expression.Body, _sqlPack);
            return this;
        }

        public Expression2SqlCore<T> Count(Expression<Func<T, object>> expression = null)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            if (expression == null)
            {
                _sqlPack.Sql.AppendFormat("select count(*) from {0}", ModelCache<T>._TableName);
            }
            else
            {
                Expression2SqlProvider.Count(expression.Body, _sqlPack);
            }

            return this;
        }

        public Expression2SqlCore<T> Sum(Expression<Func<T, object>> expression)
        {
            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            Expression2SqlProvider.Sum(expression.Body, _sqlPack);
            return this;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public Expression2SqlCore<T> Delete(T entity)
        {
            string tableName;
            if (!string.IsNullOrEmpty(ShardingName))
                tableName = ShardingName;
            else
                tableName = ModelCache<T>._TableName;

            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            _sqlPack.SetTableAlias(tableName);

            if (ShouldFilterEntity<T>())
            {
                // 软删除
                _sqlPack += "update " + tableName + " set IS_DELETED=1 ";
            }
            else
            {
                // 真删除
                _sqlPack += "delete from " + tableName;
            }
            _sqlPack += $"where ID='{entity.GetType().GetColumnValue("ID", entity)}'";
            return this;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression2SqlCore<T> Insert(Expression<Func<T>> expression = null)
        {
            string tableName;
            if (!string.IsNullOrEmpty(ShardingName))
                tableName = ShardingName;
            else
                tableName = ModelCache<T>._TableName;

            T entity = expression.Compile().Invoke();


            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            _sqlPack += $"insert into {tableName}";
            _sqlPack += $"({GetInsertColumns(typeof(T))}) values ";
            _sqlPack += "(";
            GetInsertValues(entity);
            _sqlPack += ")";
            return this;
        }

        private string GetInsertColumns(Type type)
        {
            StringBuilder columnsBuilder = new StringBuilder();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                columnsBuilder.Append(p.Name);
                columnsBuilder.Append(",");
            }

            columnsBuilder.Remove(columnsBuilder.Length - 1, 1);

            return columnsBuilder.ToString();
        }

        private void GetInsertValues(T entity)
        {
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                if (p.Name == "ID")
                {
                    _sqlPack.AddDbParameter(Guid.NewGuid());
                }
                else
                {
                    _sqlPack.AddDbParameter(p.GetValue(entity));
                }
                _sqlPack += ",";
            }

            if (_sqlPack[_sqlPack.Length - 1] == ',')
            {
                _sqlPack.Sql.Remove(_sqlPack.Length - 1, 1);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression2SqlCore<T> Update(Expression<Func<T>> expression = null)
        {
            string tableName;
            if (!string.IsNullOrEmpty(ShardingName))
                tableName = ShardingName;
            else
                tableName = ModelCache<T>._TableName;

            T entity = expression.Compile().Invoke();

            _sqlPack.Clear();
            _sqlPack.IsSingleTable = true;
            _sqlPack += "update " + tableName + " set ";

            foreach (var property in entity.GetType().GetProperties())
            {
                // 过滤ID
                if (property.Name == "ID") continue;

                _sqlPack += property.Name + " =";
                _sqlPack.AddDbParameter(property.GetValue(entity));
                _sqlPack += ",";
            }

            if (_sqlPack[_sqlPack.Length - 1] == ',')
            {
                _sqlPack.Sql.Remove(_sqlPack.Length - 1, 1);
            }

            _sqlPack += $" where ID='{entity.GetType().GetColumnValue("ID", entity)}'";
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
