using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示对静态方法或实例方法的调用。
    /// </summary>
    public class MethodCallExpressionConverter : BaseExpressionConverter<MethodCallExpression>
    {
        static Dictionary<string, Action<MethodCallExpression, SqlStorage>> _Methods = new Dictionary<string, Action<MethodCallExpression, SqlStorage>>
        {
            {"Like",Like},
            {"LikeLeft",LikeLeft},
            {"LikeRight",LikeRight},
            {"In",In},
            {"Property",Property}
        };

        private static void Property(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            //ExpressionToSqlProvider.Where(expression.Arguments[1], sqlStorage);

            string tableName = sqlStorage.GetTableName(expression.Arguments[0].Type);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName += ".";
            }
            sqlStorage += " " + tableName + (expression.Arguments[1] as ConstantExpression).Value;

            sqlStorage += " ";

        }

        private static new void In(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.Where(expression.Arguments[0], sqlStorage);
            sqlStorage += " in";
            ExpressionToSqlProvider.In(expression.Arguments[1], sqlStorage);
        }

        private static void Like(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            if (expression.Object != null)
            {
                ExpressionToSqlProvider.Where(expression.Object, sqlStorage);
            }
            ExpressionToSqlProvider.Where(expression.Arguments[0], sqlStorage);
            sqlStorage += " like '%' +";
            ExpressionToSqlProvider.Where(expression.Arguments[1], sqlStorage);
            sqlStorage += " + '%'";
        }

        private static void LikeLeft(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            if (expression.Object != null)
            {
                ExpressionToSqlProvider.Where(expression.Object, sqlStorage);
            }
            ExpressionToSqlProvider.Where(expression.Arguments[0], sqlStorage);
            sqlStorage += " like '%' +";
            ExpressionToSqlProvider.Where(expression.Arguments[1], sqlStorage);
        }

        private static void LikeRight(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            if (expression.Object != null)
            {
                ExpressionToSqlProvider.Where(expression.Object, sqlStorage);
            }
            ExpressionToSqlProvider.Where(expression.Arguments[0], sqlStorage);
            sqlStorage += " like ";
            ExpressionToSqlProvider.Where(expression.Arguments[1], sqlStorage);
            sqlStorage += " + '%'";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Where(MethodCallExpression expression, SqlStorage sqlStorage)
        {
            var key = expression.Method;
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }

            Action<MethodCallExpression, SqlStorage> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {
                action(expression, sqlStorage);
                return sqlStorage;
            }

            throw new NotImplementedException("无法解析方法" + expression.Method);
        }
    }
}
