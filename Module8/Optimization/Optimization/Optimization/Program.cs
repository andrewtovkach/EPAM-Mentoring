using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Optimization
{
    public static class ExampleClass
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            var array = new TSource[count];
            int i = 0;
            foreach (var item in source)
            {
                array[i++] = item;
            }
            return array;
        }

        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }

        //public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        //{
        //    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, 10000);
        //    byte[] hash = pbkdf2.GetBytes(20);
        //    var arr = salt.Take(16).Concat(hash.Take(20));
        //    var passwordHash = Convert.ToBase64String(ToArray(arr, 36));
        //    return passwordHash;
        //}
    }

    public class TheEasiestBenchmark
    {
        [Benchmark(Description = "GeneratePasswordHashUsingSalt")]
        public string GeneratePasswordHashUsingSalt()
        {
            return ExampleClass.GeneratePasswordHashUsingSalt("password5", Encoding.Unicode.GetBytes("5f4dcc3b5aa765d61d8327deb882cf99"));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TheEasiestBenchmark>();
            Console.ReadKey();
        }
    }
}
