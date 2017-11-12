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
            ProcessFile(e.FullPath);
        }

        public void ProcessFile(string fullPath)
        {
            var fileName = System.IO.Path.GetFileName(fullPath);

            var fileNameParser = new FileNameParser(fileName);

            if (!fileNameParser.IsValidFileName(Configuration.Pattern))
                return;

            var fileData = fileNameParser.GetFileNameData();

            var processedImagesFolderPath = System.IO.Path.Combine(Configuration.DirectoryPath,
                Configuration.ProcessedImagesFolderPath);
            var incorrectImagesFolderPath = System.IO.Path.Combine(Configuration.DirectoryPath,
                Configuration.IncorrectImagesFolderPath);

            if (Array.IndexOf(Configuration.FileExtensions, fileData.Extension) != -1 &&
                fileData.Prefix == Configuration.Prefix && !fullPath.StartsWith(processedImagesFolderPath)
                && !fullPath.StartsWith(incorrectImagesFolderPath))
            {
                bool result = _pdfGenerator.Addimage(fullPath);

                if (!result)
                {
                    Console.WriteLine("Image processing failed with error ({0})", fullPath);
                }
            }
        }
    }
}
