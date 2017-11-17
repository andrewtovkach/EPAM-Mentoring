using InputWinService.Models;
using System;
using System.Messaging;
using System.Timers;

namespace InputWinService
{
    public class InputService
    {
        private SystemStatus _systemStatus;

        public InputService()
        {
            var timer = new Timer();
            timer.Elapsed += OnTimedSendStatisticsEvent;
            timer.Interval = 1000;
            timer.Enabled = true;

            _systemStatus = SystemStatus.WaitNewFiles;
        }

        public void Start()
        {
            try
            {
                var directoryWatcher = new DirectoryWatcher(Configuration.DirectoryPath, _systemStatus);

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
            var messageQueueName = Configuration.MessageQueueName;

            var queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            using (queue)
            {
                queue.Send(new TransferData
                {
                    Id = Guid.NewGuid(),
                    Data = new StatisticsTransferingData
                    {
                        SystemStatus = _systemStatus,
                        TimeInterval = Configuration.TimeInterval,
                        MaxProcessNumber = Configuration.MaxProcessNumber,
                    },
                    Type = Models.Type.Statistics
                });
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var messageQueueName = Configuration.MessageQueueName;

            var queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            queue.Formatter = new XmlMessageFormatter(new[] { typeof(TransferData) });

            using (queue)
            {
                var message = queue.Receive();
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
                        Configuration.MaxProcessNumber = statisticsTransferingData.MaxProcessNumber;
                        Configuration.TimeInterval = statisticsTransferingData.TimeInterval;
                    }
                }
            }
        }
    }
}
