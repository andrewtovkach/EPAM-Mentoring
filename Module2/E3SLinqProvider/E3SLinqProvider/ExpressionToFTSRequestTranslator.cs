using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace E3SLinqProvider
{
    public enum FunctionEnum
    {
        None,
        StartsWith,
        EndsWith,
        Contains
    }

    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
    {
        StringBuilder resultString;
        private FunctionEnum currentFunction;
        private IList<string> queryStrings;

        public IList<string> Translate(Expression exp)
        {
            resultString = new StringBuilder();
            queryStrings = new List<string>();

            Visit(exp);

            return queryStrings;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(string))
            {
                var memberExpression = node.Object as MemberExpression;
                if (memberExpression != null)
                {
                    resultString.Append(memberExpression.Member.Name).Append(":");
                }

                IDictionary<string, FunctionEnum> methodsMapper = new Dictionary<string, FunctionEnum>();

                foreach (FunctionEnum functionEnum in Enum.GetValues(typeof(FunctionEnum)))
                {
                    methodsMapper.Add(functionEnum.ToString(), functionEnum);
                }

                currentFunction = methodsMapper[node.Method.Name];

                resultString.Append("(");
                var predicate = node.Arguments[0];
                Visit(predicate);
                resultString.Append(")");

                queryStrings.Add(resultString.ToString());
                resultString.Clear();

                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    currentFunction = FunctionEnum.None;
                    if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Left);
                        resultString.Append("(");
                        Visit(node.Right);
                        resultString.Append(")");
                        queryStrings.Add(resultString.ToString());
                        resultString.Clear();
                    }
                    else
                    {
                        Visit(node.Right);
                        resultString.Append("(");
                        Visit(node.Left);
                        resultString.Append(")");
                        queryStrings.Add(resultString.ToString());
                        resultString.Clear();
                    }
                    break;
                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    Visit(node.Right);
                    break;

                default:
                    throw new NotSupportedException($"Operation {node.NodeType} is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            switch (currentFunction)
            {
                case FunctionEnum.StartsWith:
                    resultString.Append($"{node.Value}*");
                    break;
                case FunctionEnum.EndsWith:
                    resultString.Append($"*{node.Value}");
                    break;
                case FunctionEnum.Contains:
                    resultString.Append($"*{node.Value}*");
                    break;
                case FunctionEnum.None:
                    resultString.Append(node.Value);
                    break;
            }

            return node;
        }
    }
}
