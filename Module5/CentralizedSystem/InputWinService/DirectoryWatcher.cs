using System;
using System.IO;

namespace InputWinService
{
    public class DirectoryWatcher : FileSystemWatcher
    {
        public DirectoryWatcher(string directoryPath)
             : base(directoryPath)
        {
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
            InputService.ProcessImage(e.FullPath);
        }
    }
}
