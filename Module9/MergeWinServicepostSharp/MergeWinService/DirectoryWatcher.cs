using System;
using System.IO;
using Log.Lib;
using MergeWinService.Interfaces;

namespace MergeWinService
{
    [Logging]
    public class DirectoryWatcher : FileSystemWatcher, IDirectoryWatcher
    {
        private readonly IPDFGenerator _pdfGenerator;
        private readonly IFileNameParser _fileNameParser;

        public DirectoryWatcher(string directoryPath, IPDFGenerator pdfGenerator, IFileNameParser fileNameParser)
             : base(directoryPath)
        {
            _pdfGenerator = pdfGenerator;
            _fileNameParser = fileNameParser;
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

            if (!_fileNameParser.IsValidFileName(fileName, Configuration.Pattern))
                return;

            var fileData = _fileNameParser.GetFileNameData(fileName);

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
