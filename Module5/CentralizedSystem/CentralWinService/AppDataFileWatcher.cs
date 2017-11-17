using CentralWinService.Models;
using System.IO;

namespace CentralWinService
{
    public class AppDataFileWatcher : FileSystemWatcher
    {
        public AppDataFileWatcher(string directoryPath)
             : base(directoryPath)
        {
        }

        public void Start()
        {
            IncludeSubdirectories = true;
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            EnableRaisingEvents = true;
            Changed += Watcher_Changed;
        }

        private void Watcher_Changed(object source, FileSystemEventArgs e)
        {
            var fileName = System.IO.Path.GetFileName(e.FullPath);
            var expectedFileName = Configuration.StatisticsFileName;

            if (fileName == expectedFileName)
            {
                var appData = AppDataManager.ReadFromFile<StatisticsData>(expectedFileName);

                CentralService.UpdateSettings(appData);
            }
        }
    }
}
