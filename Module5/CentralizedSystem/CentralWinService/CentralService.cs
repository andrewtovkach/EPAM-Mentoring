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
        private readonly Timer _timer;
        private readonly FileProcessor _fileProcessor;
        private readonly AppDataFileWatcher _appDataFileWatcher;

        public static AppData AppData { get; set; }
        public static StatisticsData StatisticsData { get; set; }

        public CentralService(PDFGenerator pdfGenerator, AppDataFileWatcher appDataFileWatcher)
        {
            _pdfGenerator = pdfGenerator;
            _fileProcessor = new FileProcessor(_pdfGenerator);
            _appDataFileWatcher = appDataFileWatcher;
            _appDataFileWatcher.Start();

            _timer = new Timer();
            _timer.Elapsed += OnTimedSaveDocumentEvent;
            _timer.Elapsed += OnTimedEvent;
            _timer.Interval = 1000;
            _timer.Enabled = true;
        }

        public void Start()
        {
            AppData = AppDataManager.ReadFromFile<AppData>(Configuration.AppFileName);
            StatisticsData = new StatisticsData();

            var messageQueueName = Configuration.MessageQueueName;

            if (!MessageQueue.Exists(messageQueueName))
            {
                var queue = MessageQueue.Create(messageQueueName);
            }

            foreach (var file in Directory.GetFiles(Configuration.DirectoryPath))
            {
                _fileProcessor.ProcessFile(file);
            }
        }

        public void Stop()
        {
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

                        transferData.Status = Status.InProgress;

                        switch (transferData.Type)
                        {
                            case InputWinService.Models.Type.ImageProcess:
                                var fileTransferingData = transferData.Data as FileTransferingData;
                                var result = _fileProcessor.ProcessFile(fileTransferingData.FilePath);

                                if (!result)
                                {
                                    Console.WriteLine("Image processing failed with error ({0})", fileTransferingData.FilePath);
                                }

                                transferData.Status = result ? Status.Completed : Status.Failed;
                                break;
                            case InputWinService.Models.Type.Statistics:
                                var statisticsTransferingData = transferData.Data as StatisticsTransferingData;
                                if (StatisticsData.MaxProcessNumber != statisticsTransferingData.MaxProcessNumber ||
                                    StatisticsData.TimeInterval != statisticsTransferingData.TimeInterval)
                                {
                                    StatisticsData.MaxProcessNumber = statisticsTransferingData.MaxProcessNumber;
                                    StatisticsData.TimeInterval = statisticsTransferingData.TimeInterval;
                                    AppDataManager.WrireToFile(Configuration.StatisticsFileName, StatisticsData);
                                }
                                Console.WriteLine("TimeInterval - {0}, MaxProcessNumber - {1}, SystemStatus - {2}", statisticsTransferingData.TimeInterval, 
                                    statisticsTransferingData.MaxProcessNumber, statisticsTransferingData.SystemStatus);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
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
                queue.Send(new TransferData
                {
                    Id = Guid.NewGuid(),
                    Type = InputWinService.Models.Type.UpdateSettings,
                    Data = new StatisticsTransferingData
                    {
                        MaxProcessNumber = statisticsData.MaxProcessNumber,
                        TimeInterval = statisticsData.TimeInterval
                    }
                });
            }
        }
    }
}
