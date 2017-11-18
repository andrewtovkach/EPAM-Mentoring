using InputWinService.Models;
using System;
using System.IO;
using System.Messaging;
using System.Timers;

namespace InputWinService
{
    public class InputService
    {
        public static SystemStatus SystemStatus;
        private MessageQueue _queue;

        public InputService()
        {
            var timer = new Timer();
            timer.Elapsed += OnTimedSendStatisticsEvent;
            timer.Elapsed += OnTimedEvent;
            timer.Interval = 1000;
            timer.Enabled = true;

            SystemStatus = SystemStatus.WaitNewFiles;
        }

        public void Start()
        {
            var messageQueueName = Configuration.MessageQueueName;

            _queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            try
            {
                var directoryWatcher = new DirectoryWatcher(Configuration.DirectoryPath);

                directoryWatcher.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void Stop()
        {
        }

        private void OnTimedSendStatisticsEvent(object source, ElapsedEventArgs e)
        {
            using (_queue)
            {
                var type = Models.Type.Statistics;

                _queue.Send(new TransferData
                {
                    Id = Guid.NewGuid(),
                    Data = new StatisticsTransferingData
                    {
                        SystemStatus = SystemStatus,
                        TimeInterval = Configuration.TimeInterval,
                        MaxProcessNumber = Configuration.MaxProcessNumber,
                    },
                    Type = type
                });

                Console.WriteLine("The request was sent - {0} - {1}", DateTime.Now, type);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _queue.Formatter = new XmlMessageFormatter(new[] { typeof(TransferData) });

            using (_queue)
            {
                var message = _queue.Receive();
                if (message != null)
                {
                    if (message.Body != null)
                    {
                        var transferData = message.Body as TransferData;
                        if (transferData == null)
                        {
                            return;
                        }

                        var statisticsTransferingData = transferData.Data as StatisticsTransferingData;

                        Console.WriteLine("The response was received - {0} - {1}", DateTime.Now, transferData.Type);

                        if (statisticsTransferingData != null)
                        {
                            Configuration.MaxProcessNumber = statisticsTransferingData.MaxProcessNumber;
                            Configuration.TimeInterval = statisticsTransferingData.TimeInterval;
                        }
                    }
                }
            }
        }

        public static void ProcessImage(string fullPath)
        {
            var messageQueueName = Configuration.MessageQueueName;

            var queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            SystemStatus = SystemStatus.ProcessFiles;

            using (queue)
            {
                var processedImagesFolderPath = Path.Combine(Configuration.DirectoryPath,
                    Configuration.ProcessedImagesFolderPath);
                var incorrectImagesFolderPath = Path.Combine(Configuration.DirectoryPath,
                    Configuration.IncorrectImagesFolderPath);
                var generatedPDFfilesPath = Path.Combine(Configuration.DirectoryPath,
                    Configuration.GeneratedPDFFolderPath);

                if (!fullPath.StartsWith(processedImagesFolderPath) && !fullPath.StartsWith(incorrectImagesFolderPath)
                    && !fullPath.StartsWith(generatedPDFfilesPath))
                {
                    queue.Send(new TransferData
                    {
                        Id = Guid.NewGuid(),
                        Data = new FileTransferingData
                        {
                            FilePath = fullPath,
                        },
                        Type = Models.Type.ImageProcess,
                        Status = Status.Submitted
                    });
                }
            }

            SystemStatus = SystemStatus.WaitNewFiles;
        }

    }
}
