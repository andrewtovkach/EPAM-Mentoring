using System;
using System.Threading;
using System.Threading.Tasks;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            var semaphoreObject = new SemaphoreSlim(2, 2);
             
            for (var taskIdx = 0; taskIdx < 10; taskIdx++)
            {
                var idx = taskIdx;
                Task.Factory.StartNew(() =>
                {
                    semaphoreObject.Wait();
                    TaskOperation(idx);
                    semaphoreObject.Release();
                });
            }

            Console.ReadKey();
        }

        private static void TaskOperation(int taskIndex)
        {
            Console.WriteLine("Task index: {0}", taskIndex);
            Task.Delay(5000).Wait();
        }
    }
}
