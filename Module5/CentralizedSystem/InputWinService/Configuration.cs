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
                return !_timeInterval.HasValue ? 1 : _timeInterval.Value;
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
                return !_maxProcessNumber.HasValue ? 5 : _maxProcessNumber.Value;
            }
            set
            {
                _maxProcessNumber = value;
            }
        }
    }
}
