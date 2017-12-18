using System.Runtime.Caching;
using Castle.DynamicProxy;

namespace Cache.Lib
{
    public class CacheInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var reflectedType = invocation.Method.ReflectedType;
            if (reflectedType == null)
                return;

            var fullMethodName = $"{reflectedType.FullName}.{invocation.Method.Name}";
            var key = GetCacheKey(fullMethodName, invocation.Arguments);
            var value = MemoryCache.Default.Get(key);

            if (value == null)
            {
                invocation.Proceed();
                value = invocation.ReturnValue;

                if (value != null)
                {
                    MemoryCache.Default.Set(key, value, new CacheItemPolicy());
                }
            }
            else
            {
                invocation.ReturnValue = value;
            }
        }

        private static string GetCacheKey(string methodName, object[] arguments)
        {
            return methodName + "(" + string.Join(";", arguments) + ")";
        }
    }
}
