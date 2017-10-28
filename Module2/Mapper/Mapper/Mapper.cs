using System;
using System.Linq.Expressions;

namespace Mapper
{
    public class Mapper<TSource, TDestination>
    {
        private readonly Func<TSource, TDestination> _mapFunction;

        internal Mapper(Func<TSource, TDestination> func)
        {
            _mapFunction = func;
        }

        public TDestination Map(TSource source)
        {
            var destination = _mapFunction(source);

            foreach (var property in typeof(TDestination).GetProperties())
            {
                var method = typeof(Mapper<TSource, TDestination>).GetMethod("MapProperty");
                if (method == null)
                {
                    continue;
                }

                var genericMethod = method.MakeGenericMethod(property.PropertyType);
                genericMethod.Invoke(this, new object[] { destination, source, property.Name });
            }

            return destination;
        }

        public void MapProperty<TProperty>(TDestination destination, TSource source, string propertyName)
        {
            var property = typeof(TDestination).GetProperty(propertyName);
            if (property == null)
            {
                return;
            }

            var propertyInfo = typeof(TSource).GetProperty(property.Name);
            if (propertyInfo != null)
            {
                var sourceValue = propertyInfo.GetValue(source);
                property.GetSetMethod(true).Invoke(destination, new[] { sourceValue });
            }

            Expression propertyExpr = Expression.Property(
                Expression.Constant(destination), property
                );

            Expression.Lambda<Func<TProperty>>(propertyExpr).Compile();
        }
    }
}
