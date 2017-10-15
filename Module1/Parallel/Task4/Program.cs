using System;
using System.Threading;
using System.Threading.Tasks;

namespace Task4
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => TaskOperation())
                .ContinueWith(task =>
                {
                    Console.WriteLine("The continuation task is executing...");
                    return task;
                });

            Task.Run(() => TaskOperationWithException())
               .ContinueWith(task =>
               {
                    Console.WriteLine("The parent task has completed with error");
               }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Run(() => TaskOperationWithException())
               .ContinueWith(task =>
                {
                    Console.WriteLine("The parent task has completed with error");
                    Console.WriteLine("Parent task thread is reusing...");
                    task.Wait();
                }, TaskContinuationOptions.OnlyOnFaulted);

            var cancellationToken = new CancellationToken(true);

            Task.Run(() => TaskOperation(), cancellationToken)
                .ContinueWith(task =>
                {
                    Console.WriteLine("The parent task was cancelled");
                }, TaskContinuationOptions.OnlyOnCanceled);

            Console.ReadKey();
        }

        private static void TaskOperation()
        {
            Task.Delay(5000).Wait();
        }

        private static void TaskOperationWithException()
        {
            Task.Delay(5000).Wait();
            throw new Exception();
        }
    }
}
