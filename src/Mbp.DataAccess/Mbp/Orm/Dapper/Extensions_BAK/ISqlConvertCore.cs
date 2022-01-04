using Nitrogen.Ddd.Domain;
using Nitrogen.Orm.Dapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nitrogen.Orm.Dapper.Extensions
{
    public interface ISqlConvertCore<TPrimary>
        where TPrimary : class, IEntity
    {
        string SqlString { get; }

        Dictionary<string, object> DbParams { get; }

        void Clear();

        SqlConvertCore<TPrimary> Select(Expression<Func<TPrimary, object>> expression = null, string tableName = null);

        SqlConvertCore<TPrimary> Where(Expression<Func<TPrimary, bool>> expression, string tableName = null);

        SqlConvertCore<TPrimary> OrderBy(Expression<Func<TPrimary, object>> expression, string tableName = null);

        SqlConvertCore<TPrimary> OrderByDescending(Expression<Func<TPrimary, object>> expression, string tableName = null);
    }
}
