using System;
using System.IO;
using System.Timers;

namespace MergeWinService
{
    public class MergeService
    {
        private readonly PDFGenerator _pdfGenerator;
        private readonly AppDataManager _appDataManager;
        private readonly Timer _timer;

        public static AppData AppData { get; set; }

        public MergeService(PDFGenerator pdfGenerator, AppDataManager appDataManager)
        {
            _pdfGenerator = pdfGenerator;
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
                var directoryWatcher = new DirectoryWatcher(Configuration.DirectoryPath, _pdfGenerator);

                foreach (var file in Directory.GetFiles(Configuration.DirectoryPath))
                {
                    directoryWatcher.ProcessFile(file);
                }

                directoryWatcher.Start();

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
