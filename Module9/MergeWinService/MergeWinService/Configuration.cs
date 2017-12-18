using System;
using System.Configuration;

namespace MergeWinService
{
    public static class Configuration
    {
        public static string DirectoryPath => ConfigurationManager.AppSettings["directoryPath"];

        public static string Pattern => ConfigurationManager.AppSettings["pattern"];

        public static string[] FileExtensions
        {
            get
            {
                var fileExtensionsValue = ConfigurationManager.AppSettings["fileExtensions"];
                return fileExtensionsValue.Split(' ');
            }
        }

        public static string ServiceName => ConfigurationManager.AppSettings["serviceName"];

        public static string DisplayName => ConfigurationManager.AppSettings["displayName"];

        public static string Description => ConfigurationManager.AppSettings["description"];

        public static string Prefix => ConfigurationManager.AppSettings["prefix"];

        public static string GeneratedPDFFolderPath => ConfigurationManager.AppSettings["generatedPDFFolderPath"];

        public static string ProcessedImagesFolderPath => ConfigurationManager.AppSettings["processedImagesFolderPath"];
        public static string IncorrectImagesFolderPath => ConfigurationManager.AppSettings["incorrectImagesFolderPath"];

        public static string PDFFileName => ConfigurationManager.AppSettings["pdfFileName"];
        public static int TimeInterval => Convert.ToInt32(ConfigurationManager.AppSettings["timeInterval"]);
    }
}
