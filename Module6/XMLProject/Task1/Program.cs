using System;
using System.Xml;
using System.Xml.Schema;

namespace XMLProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Action successfulCallback = () => Console.WriteLine("There is no errors in the file");
            Action<ValidationEventArgs> failedCallback = e => Console.WriteLine("[{0}:{1}] {2}", e.Exception.LineNumber, e.Exception.LinePosition, e.Message);

            VerifyXmlFile("http://library.by/catalog", "XMLSchema.xsd", "books.xml", successfulCallback, failedCallback);

            Console.WriteLine();

            VerifyXmlFile("http://library.by/catalog", "XMLSchema.xsd", "books_with_errors.xml", successfulCallback, failedCallback);

            Console.ReadKey();
        }

        private static void VerifyXmlFile(string targetNamespace, string schemaUri, string inputUri, 
            Action successfulCallback, Action<ValidationEventArgs> failedCallback)
        {
            int countErrors = 0;

            XmlReaderSettings settings = new XmlReaderSettings();

            settings.Schemas.Add(targetNamespace, schemaUri);
            settings.ValidationEventHandler +=
                delegate(object sender, ValidationEventArgs e)
                {
                    countErrors++;
                    failedCallback(e);
                };

            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            XmlReader reader = XmlReader.Create(inputUri, settings);

            while (reader.Read()) ;

            if (countErrors == 0)
            {
                successfulCallback();
            }
        }
    }
}
