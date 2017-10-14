using System;
using System.Linq;
using System.Threading.Tasks;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasksChain = Task.Run(() => GetRandomArray(10))
                .ContinueWith(task =>
                {
                    var resultArray = task.Result;
                    ShowArray(resultArray);
                    return GetNewRandomArray(resultArray);
                })
                .ContinueWith(task =>
                {
                    var resultArray = task.Result;
                    ShowArray(resultArray);
                    return GetSortedArray(resultArray);
                })
                .ContinueWith(task =>
                {
                    var resultArray = task.Result;
                    ShowArray(resultArray);
                    return GetAverageValue(resultArray);
                });
            Console.WriteLine("Average Value: {0}", tasksChain.Result);
            Console.ReadKey();
        }

        private static void ShowArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write("{0} ", array[i]);
            }
            Console.WriteLine();
        }

        private static int[] GetRandomArray(int length)
        {
            var random = new Random();
            var array = new int[length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(1000);
            }
            return array;
        }

        private static int[] GetNewRandomArray(int[] array)
        {
            var random = new Random();
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = array[i] * random.Next(1000);
            }
            return array;
        }

        private static int[] GetSortedArray(int[] array)
        {
            Array.Sort(array, (i1, i2) => i2.CompareTo(i1));
            return array;
        }

        private static double GetAverageValue(int[] array)
        {
            return array.Average();
        }
    }
}
