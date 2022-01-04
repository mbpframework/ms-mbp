using System;
using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示具有二进制运算符的表达式。
    /// </summary>
    public class BinaryExpressionConverter : BaseExpressionConverter<BinaryExpression>
    {
        private void OperatorParser(ExpressionType expressionNodeType, int operatorIndex, SqlStorage sqlStorage, bool useIs = false)
        {
            switch (expressionNodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlStorage.Sql.Insert(operatorIndex, "\nand");
                    break;
                case ExpressionType.Equal:
                    if (useIs)
                    {
                        sqlStorage.Sql.Insert(operatorIndex, " is");
                    }
                    else
                    {
                        sqlStorage.Sql.Insert(operatorIndex, " =");
                    }
                    break;
                case ExpressionType.GreaterThan:
                    sqlStorage.Sql.Insert(operatorIndex, " >");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sqlStorage.Sql.Insert(operatorIndex, " >=");
                    break;
                case ExpressionType.NotEqual:
                    if (useIs)
                    {
                        sqlStorage.Sql.Insert(operatorIndex, " is not");
                    }
                    else
                    {
                        sqlStorage.Sql.Insert(operatorIndex, " <>");
                    }
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sqlStorage.Sql.Insert(operatorIndex, "\nor");
                    break;
                case ExpressionType.LessThan:
                    sqlStorage.Sql.Insert(operatorIndex, " <");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sqlStorage.Sql.Insert(operatorIndex, " <=");
                    break;
                default:
                    throw new NotImplementedException("未实现的节点类型" + expressionNodeType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Where(BinaryExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.Where(expression.Left, sqlStorage);
            int signIndex = sqlStorage.Length;

            ExpressionToSqlProvider.Where(expression.Right, sqlStorage);
            int sqlLength = sqlStorage.Length;

            if (sqlLength - signIndex == 5 && sqlStorage.ToString().EndsWith("null"))
            {
                OperatorParser(expression.NodeType, signIndex, sqlStorage, true);
            }
            else
            {
                OperatorParser(expression.NodeType, signIndex, sqlStorage);
            }

            return sqlStorage;
        }
    }
}
