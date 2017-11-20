using System;
using System.Messaging;
using System.Timers;
using CentralWinService.Models;
using InputWinService.Models;
using System.IO;

namespace CentralWinService
{
    public class CentralService
    {
        private readonly PDFGenerator _pdfGenerator;
        private readonly FileProcessor _fileProcessor;
        private MessageQueue _queue;

        public static AppData AppData { get; set; }
        public static StatisticsData StatisticsData { get; set; }

        public CentralService(PDFGenerator pdfGenerator, AppDataFileWatcher appDataFileWatcher)
        {
            _pdfGenerator = pdfGenerator;
            _fileProcessor = new FileProcessor(_pdfGenerator);
            appDataFileWatcher.Start();

            var timer = new Timer();
            timer.Elapsed += OnTimedSaveDocumentEvent;
            timer.Elapsed += OnTimedEvent;
            timer.Interval = 1000;
            timer.Enabled = true;
        }

        public void Start()
        {
            AppData = AppDataManager.ReadFromFile<AppData>(Configuration.AppFileName);
            StatisticsData = AppDataManager.ReadFromFile<StatisticsData>(Configuration.StatisticsFileName);

            var messageQueueName = Configuration.MessageQueueName;

            _queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            foreach (var file in Directory.GetFiles(Configuration.DirectoryPath))
            {
                _fileProcessor.ProcessFile(file);
            }
        }

        public void Stop()
        {
            var messageQueueName = Configuration.MessageQueueName;
            MessageQueue.Delete(messageQueueName);

            if (_pdfGenerator.IsSaved)
                return;

            try
            {
                _pdfGenerator.SaveDocument();
                AppData.CountGeneratedPDFFiles++;
                AppDataManager.WrireToFile(Configuration.AppFileName, AppData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _queue.Formatter = new XmlMessageFormatter(new[] { typeof(TransferData) });

            using (_queue)
            {
                var peek = _queue.Peek();
                if (peek != null)
                {
                    var data = peek.Body as TransferData;
                    if (data != null && data.Status == Status.SubmittedCentralSystem)
                    {
                        return;
                    }
                }

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

                        Console.WriteLine("The response was received - {0} - {1}", DateTime.Now, transferData.Type);

                        transferData.Status = Status.InProgress;

                        switch (transferData.Type)
                        {
                            case InputWinService.Models.Type.ImageProcess:
                                var fileTransferingData = transferData.Data as FileTransferingData;
                                var result = fileTransferingData != null && _fileProcessor.ProcessFile(fileTransferingData.FilePath);

                                if (!result && fileTransferingData != null)
                                {
                                    Console.WriteLine("Image processing failed with error ({0})", fileTransferingData.FilePath);
                                }
                                else
                                {
                                    Console.WriteLine("Image processing completed successfuly - {0}", transferData.Id);
                                }

                                transferData.Status = result ? Status.Completed : Status.Failed;

                                break;
                            case InputWinService.Models.Type.Statistics:
                                var statisticsTransferingData = transferData.Data as StatisticsTransferingData;
                                if (statisticsTransferingData != null && IsStatisticsDataChanged(statisticsTransferingData))
                                {
                                    StatisticsData.MaxProcessNumber = statisticsTransferingData.MaxProcessNumber;
                                    StatisticsData.TimeInterval = statisticsTransferingData.TimeInterval;
                                }
                                if (statisticsTransferingData != null)
                                    Console.WriteLine("TimeInterval - {0}, MaxProcessNumber - {1}, SystemStatus - {2}", statisticsTransferingData.TimeInterval, 
                                        statisticsTransferingData.MaxProcessNumber, statisticsTransferingData.SystemStatus);
                                break;
                        }
                    }
                }
            }
        }

        private static bool IsStatisticsDataChanged(StatisticsTransferingData statisticsTransferingData)
        {
            return StatisticsData.MaxProcessNumber != statisticsTransferingData.MaxProcessNumber ||
                   StatisticsData.TimeInterval != statisticsTransferingData.TimeInterval;
        }

        private void OnTimedSaveDocumentEvent(object source, ElapsedEventArgs e)
        {
            var milliseconds = _pdfGenerator.LastImageDateTime.HasValue ?
                (DateTime.Now - _pdfGenerator.LastImageDateTime.Value).Minutes : -1;

            if (milliseconds == -1 || milliseconds < StatisticsData.TimeInterval ||
                _pdfGenerator.PreprocessedImagesCount <= 0 || _pdfGenerator.IsSaved)
                return;

            try
            {
                _pdfGenerator.SaveDocument();
                AppData.CountGeneratedPDFFiles++;
                AppDataManager.WrireToFile(Configuration.AppFileName, AppData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static void UpdateSettings(StatisticsData statisticsData)
        {
            var messageQueueName = Configuration.MessageQueueName;

            var queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            using (queue)
            {
                var type = InputWinService.Models.Type.UpdateSettings;
                queue.Send(new TransferData
                {
                    Id = Guid.NewGuid(),
                    Type = type,
                    Data = new StatisticsTransferingData
                    {
                        MaxProcessNumber = statisticsData.MaxProcessNumber,
                        TimeInterval = statisticsData.TimeInterval
                    },
                    Status = Status.SubmittedCentralSystem
                });

                Console.WriteLine("The request was sent - {0} - {1}", DateTime.Now, type);
            }
        }
    }
}
