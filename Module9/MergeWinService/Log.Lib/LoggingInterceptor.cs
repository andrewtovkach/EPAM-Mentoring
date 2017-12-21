using System;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace Log.Lib
{
    public class LoggingInterceptor : IInterceptor
    {
        public LoggingInterceptor()
        {
            Logger.InitLogger();
        }

        public void Intercept(IInvocation invocation)
        {
            string parameters;

            try
            {
                parameters = JsonConvert.SerializeObject(invocation.Arguments);
            }
            catch (Exception)
            {
                parameters = "Not serializable";
            }

            var reflectedType = invocation.Method.ReflectedType;

            if (reflectedType == null)
                return;

            var logMessage = $"{reflectedType.FullName}.{invocation.Method.Name} - {parameters}";

            invocation.Proceed();

            if (invocation.Method.ReturnType != typeof(void))
            {
                logMessage += $" = {JsonConvert.SerializeObject(invocation.ReturnValue)}";
            }

            Logger.Log.Info(logMessage);
        }
    }
}
