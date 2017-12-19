using System;
using System.IO;
using System.Timers;
using Log.Lib;
using MergeWinService.Interfaces;
using MergeWinService.Models;

namespace MergeWinService
{
    [Logging]
    public class MergeService: IMergeService
    {
        private readonly IPDFGenerator _pdfGenerator;
        private readonly IDirectoryWatcher _directoryWatcher;
        private readonly IAppDataManager _appDataManager;
        private readonly Timer _timer;

        public static AppData AppData { get; set; }

        public MergeService(IPDFGenerator pdfGenerator, IDirectoryWatcher directoryWatcher, IAppDataManager appDataManager)
        {
            _pdfGenerator = pdfGenerator;
            _directoryWatcher = directoryWatcher;
            _appDataManager = appDataManager;
            _timer = new Timer();
            _timer.Elapsed += OnTimedEvent;
            _timer.Interval = 5000;
        }

        public void Start()
        {
            try
            {
                AppData = _appDataManager.ReadFromFile();

                foreach (var file in Directory.GetFiles(Configuration.DirectoryPath))
                {
                    _directoryWatcher.ProcessFile(file);
                }

                _directoryWatcher.Start();

                _timer.Enabled = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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
                _appDataManager.WrireToFile(AppData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var milliseconds = _pdfGenerator.LastImageDateTime.HasValue ? 
                (DateTime.Now - _pdfGenerator.LastImageDateTime.Value).Minutes : -1;

            if (milliseconds == -1 || milliseconds < Configuration.TimeInterval ||
                _pdfGenerator.PreprocessedImagesCount <= 0 || _pdfGenerator.IsSaved)
                return;

            try
            {
                _pdfGenerator.SaveDocument();
                AppData.CountGeneratedPDFFiles++;
                _appDataManager.WrireToFile(AppData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
