using System;
using System.IO;
using System.Messaging;
using InputWinService.Models;

namespace InputWinService
{
    public class DirectoryWatcher : FileSystemWatcher
    {
        private SystemStatus _systemStatus;

        public DirectoryWatcher(string directoryPath, SystemStatus systemStatus)
             : base(directoryPath)
        {
            _systemStatus = systemStatus;
        }

        public void Start()
        {
            IncludeSubdirectories = true;
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            EnableRaisingEvents = true;
            Created += Watcher_Created;
        }

        private void Watcher_Created(object source, FileSystemEventArgs e)
        {
            var messageQueueName = Configuration.MessageQueueName;

            var queue = MessageQueue.Exists(messageQueueName) ?
                new MessageQueue(messageQueueName) : MessageQueue.Create(messageQueueName);

            _systemStatus = SystemStatus.ProcessFiles;

            using (queue)
            {
                queue.Send(new TransferData
                {
                    Id = Guid.NewGuid(),
                    Data = new FileTransferingData
                    {
                        FilePath = e.FullPath,
                    },
                    Type = Models.Type.ImageProcess,
                    Status = Status.Submitted
                });
            }

            _systemStatus = SystemStatus.WaitNewFiles;
        }
    }
}
