using System;

[Equals]
public class Point
{
    public int X { get; set; }

    public int Y { get; set; }

    [IgnoreDuringEquals]
    public int Z { get; set; }
}

namespace FodyEquals
{
    class Program
    {
        static void Main(string[] args)
        {
            var p1 = new Point {X = 2, Y = 2, Z = 3};
            var p2 = new Point { X = 1, Y = 2, Z = 4 };
            Console.WriteLine(p1.Equals(p2));
            Console.ReadKey();
        }
    }
}
