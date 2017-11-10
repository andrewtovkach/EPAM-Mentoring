using System;
using System.IO;

namespace MergeWinService
{
    public class DirectoryWatcher : FileSystemWatcher
    {
        private readonly PDFGenerator _pdfGenerator;

        public DirectoryWatcher(string directoryPath, PDFGenerator pdfGenerator)
             : base(directoryPath)
        {
            _pdfGenerator = pdfGenerator;

            Init();
        }

        private void Init()
        {
            IncludeSubdirectories = true;
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            EnableRaisingEvents = true;
            Created += Watcher_Created;
        }

        public void Watcher_Created(object source, FileSystemEventArgs e)
        {
            var fileName = System.IO.Path.GetFileName(e.FullPath);

            var fileNameParser = new FileNameParser(fileName);

            if (!fileNameParser.IsValidFileName(Configuration.Pattern))
                return;

            var fileData = fileNameParser.GetFileNameData();

            var processedImagesFolderPath = System.IO.Path.Combine(Configuration.DirectoryPath, Configuration.ProcessedImagesFolderPath);

            if (Array.IndexOf(Configuration.FileExtensions, fileData.Extension) != -1 &&
                fileData.Prefix == Configuration.Prefix && !e.FullPath.StartsWith(processedImagesFolderPath))
            {
                _pdfGenerator.Addimage(e.FullPath);
            }
        }
    }
}
