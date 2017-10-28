using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionVisitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<int, int, int, int>> sourceExpression = (a, b, c) => a + b + c + (a + 1) + (a + 5) * (a - 1);

            var paramsDictionary = new Dictionary<string, int>
            {
                { "a", 2 },
                { "b", 3 },
                { "c", 1 }
            };

            var customExpression = new CustomExpressionVisitor<Func<int, int, int, int>>(sourceExpression, paramsDictionary);
            var resultExpression = customExpression.GetResultExpression();

            Console.WriteLine("Source Expression: ");
            Console.WriteLine(sourceExpression);
            Console.WriteLine("lambda(2,3,1) = {0}", sourceExpression.Compile().Invoke(2,3,1));

            if (resultExpression != null)
            {
                Console.WriteLine("\nResult Expression: ");
                Console.WriteLine(resultExpression);
                Console.WriteLine("lambda({0}) = {1}", customExpression.GetParams(), resultExpression.Compile().Invoke(0,0,0));
            }

            Console.ReadKey();
        }
    }
}
