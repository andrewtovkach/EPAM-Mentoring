using System;
using Newtonsoft.Json;
using PostSharp.Aspects;

namespace Log.Lib
{
    [Serializable]
    public class LoggingAttribute : OnMethodBoundaryAspect
    {
        private string _logMessage;

        public override void OnEntry(MethodExecutionArgs args)
        {
            string parameters;

            try
            {
                parameters = JsonConvert.SerializeObject(args.Arguments);
            }
            catch (Exception)
            {
                parameters = "Not serializable";
            }

            var reflectedType = args.Method.ReflectedType;

            if (reflectedType == null)
                return;

            _logMessage = $"{reflectedType.FullName}.{args.Method.Name} - {parameters}";
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            if (args.Method.ReflectedType != Type.GetType("Void"))
            {
                _logMessage += $" = {JsonConvert.SerializeObject(args.ReturnValue)}";
            }

            Logger.Log.Info(_logMessage);
        }
    }
}