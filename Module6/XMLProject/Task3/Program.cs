using System;
using System.IO;
using Saxon.Api;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            Processor processor = new Processor(true);

            Xslt30Transformer transformer = processor.NewXsltCompiler().Compile(new Uri(Path.GetFullPath("XSLTFile.xslt"))).Load30();

            Serializer serializer = new Serializer();

            StreamWriter writer = new StreamWriter("result.html");

            serializer.SetOutputWriter(writer);

            using (FileStream fileStream = new FileStream("books.xml", FileMode.Open))
            {
                transformer.ApplyTemplates(fileStream, serializer);
            }

            writer.Close();
        }
    }
}
