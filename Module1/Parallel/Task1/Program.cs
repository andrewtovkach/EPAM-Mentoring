using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasksList = new List<Task>();

            for (int taskIdx = 0; taskIdx < 100; taskIdx++)
            {
                var idx = taskIdx;
                tasksList.Add(Task.Run(() => TaskOperation(idx)));
            }

            Task.WaitAll(tasksList.ToArray());

            Console.ReadKey();
        }

        private static void TaskOperation(int taskIndex)
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine($"Task #{taskIndex} – iteration number #{i}");
            }
        }
    }
}
