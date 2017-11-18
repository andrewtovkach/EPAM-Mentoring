using System.Configuration;

namespace InputWinService
{
    public static class Configuration
    {
        public static string DirectoryPath => ConfigurationManager.AppSettings["directoryPath"];

        public static string ServiceName => ConfigurationManager.AppSettings["serviceName"];

        public static string DisplayName => ConfigurationManager.AppSettings["displayName"];

        public static string Description => ConfigurationManager.AppSettings["description"];

        public static string MessageQueueName => ConfigurationManager.AppSettings["messageQueueName"];

        private static int? _timeInterval;
        public static int TimeInterval
        {
            get
            {
                return _timeInterval ?? 1;
            }
            set
            {
                _timeInterval = value;
            }
        }
        private static int? _maxProcessNumber;
        public static int MaxProcessNumber
        {
            get
            {
                return _maxProcessNumber ?? 5;
            }
            set
            {
                _maxProcessNumber = value;
            }
        }
        public static string ProcessedImagesFolderPath => ConfigurationManager.AppSettings["processedImagesFolderPath"];

        public static string IncorrectImagesFolderPath => ConfigurationManager.AppSettings["incorrectImagesFolderPath"];

        public static string GeneratedPDFFolderPath => ConfigurationManager.AppSettings["generatedPDFFolderPath"];
    }
}
