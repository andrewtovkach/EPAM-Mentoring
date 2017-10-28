using System;

namespace Mapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();

            var foo = new Foo
            {
                A = "AAA",
                B = 2
            };

            var res = mapper.Map(foo);

            Console.WriteLine("res.A = '{0}', res.B = '{1}', res.C = '{2}', res.D = '{3}'", res.A, res.B, res.C, res.D);

            Console.ReadKey();
        }
    }
}
