﻿using System;

namespace CentralWinService
{
    public class FileProcessor
    {
        private readonly PDFGenerator _pdfGenerator;

        public FileProcessor(PDFGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        public bool ProcessFile(string fullPath)
        {
            var fileName = System.IO.Path.GetFileName(fullPath);

            var fileNameParser = new FileNameParser(fileName);

            if (!fileNameParser.IsValidFileName(Configuration.Pattern))
                return false;

            var fileData = fileNameParser.GetFileNameData();

            if (Array.IndexOf(Configuration.FileExtensions, fileData.Extension) != -1 &&
                fileData.Prefix == Configuration.Prefix)
            {
                bool result = _pdfGenerator.Addimage(fullPath);

                if (!result)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
