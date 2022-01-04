using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Expression2Sql
{
    class MemberInitExpression2Sql : BaseExpression2Sql<MemberInitExpression>
    {
        protected override SqlPack Update(MemberInitExpression expression, SqlPack sqlPack)
        {
            for (int i = 0; i < expression.Bindings.Count; i++)
            {
                MemberInfo m = expression.Bindings[i].Member;

                var bodyexpression = ((MemberAssignment)expression.Bindings[i]).Expression;

                object value = null;
                if (bodyexpression is ConstantExpression)
                {
                    value = ((((MemberAssignment)expression.Bindings[i]).Expression) as ConstantExpression).Value;
                }
                else if (bodyexpression is MethodCallExpression)
                {
                    var methonExpression = ((((MemberAssignment)expression.Bindings[i]).Expression) as MethodCallExpression);

                    value = Expression.Lambda(methonExpression).Compile().DynamicInvoke();
                }

                sqlPack += m.Name + " =";
                sqlPack.AddDbParameter(value);
                sqlPack += ",";
            }

            if (sqlPack[sqlPack.Length - 1] == ',')
            {
                sqlPack.Sql.Remove(sqlPack.Length - 1, 1);
            }
            return sqlPack;
        }
    }
}
