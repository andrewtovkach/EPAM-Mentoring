using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionVisitor
{
    public class ExpressionData
    {
        public ParameterExpression Param { get; set; }
        public ConstantExpression Constant { get; set; }
    }

    public class CustomExpressionVisitor<T>: System.Linq.Expressions.ExpressionVisitor
    {
        private readonly IDictionary<string, int> _paramsDictionary;
        private readonly Expression<T> _expression;

        public CustomExpressionVisitor(Expression<T> expression, IDictionary<string, int> paramsDictionary)
        {
            _expression = expression;
            _paramsDictionary = paramsDictionary;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                {
                    var expressionData = GetExpressionData(node);

                    if (expressionData.Param != null && expressionData.Constant != null && expressionData.Constant.Type == typeof(int) && 
                        (int)expressionData.Constant.Value == 1)
                    {
                        return Expression.Increment(
                            Expression.Constant(_paramsDictionary[expressionData.Param.Name])
                            );
                    }
                }
                    break;
                case ExpressionType.Subtract:
                {
                    var expressionData = GetExpressionData(node);

                    if (expressionData.Param != null && expressionData.Constant != null && expressionData.Constant.Type == typeof(int) && 
                        (int)expressionData.Constant.Value == 1)
                    {
                        return Expression.Decrement(
                            Expression.Constant(_paramsDictionary[expressionData.Param.Name])
                            );
                    }
                }
                    break;
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_paramsDictionary == null || _paramsDictionary.Count == 0)
            {
                throw new InvalidExpressionException("Set of params is incorrect");
            }

            foreach (var param in _paramsDictionary)
            {
                if (node.Type == typeof(int) && param.Key == node.Name)
                {
                    return Expression.Constant(_paramsDictionary[param.Key]);
                }
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (!node.Parameters.All(param => _paramsDictionary.ContainsKey(param.Name)))
            {
                throw new InvalidExpressionException("Set of params is incorrect");
            }

            return Expression.Lambda(Visit(node.Body), node.Parameters);
        }

        public Expression<T> GetResultExpression()
        {
            return VisitAndConvert(_expression, "");
        }

        public string GetParams()
        {
            return string.Join(",", _paramsDictionary.Values.ToArray());
        }

        private static ExpressionData GetExpressionData(BinaryExpression node)
        {
            var expressionData = new ExpressionData();

            switch (node.Left.NodeType)
            {
                case ExpressionType.Parameter:
                    expressionData.Param = (ParameterExpression) node.Left;
                    break;
                case ExpressionType.Constant:
                    expressionData.Constant = (ConstantExpression) node.Left;
                    break;
            }

            switch (node.Right.NodeType)
            {
                case ExpressionType.Parameter:
                    expressionData.Param = (ParameterExpression) node.Right;
                    break;
                case ExpressionType.Constant:
                    expressionData.Constant = (ConstantExpression) node.Right;
                    break;
            }
            return expressionData;
        }
    }
}
