using System;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;

namespace MergeWinService
{
    public class PDFGenerator
    {
        private readonly string _fileName;
        private readonly PdfDocument _document;
        public int PreprocessedImagesCount { get; set; }
        public DateTime? LastImageDateTime { get; set; }
        public bool IsSaved { get; set; }

        public PDFGenerator(string fileName)
        {
            _fileName = fileName;
            PreprocessedImagesCount = 0;
            IsSaved = false;

            _document = new PdfDocument();
        }

        public void Addimage(string imagePath)
        {
            PdfPage page = _document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            var result = DrawImage(gfx, imagePath,
                new DrawingParams
                {
                    X = 50,
                    Y = 50,
                    Width = 500,
                    Height = 500
                });

            var processedImagesFolderPath = result ? Path.Combine(Configuration.DirectoryPath,
                Configuration.ProcessedImagesFolderPath) :
                Path.Combine(Configuration.DirectoryPath, Configuration.IncorrectImagesFolderPath);
            var newImagePath = imagePath.Replace(Configuration.DirectoryPath, processedImagesFolderPath);

            if (!Directory.Exists(processedImagesFolderPath))
            {
                Directory.CreateDirectory(processedImagesFolderPath);
            }

            File.Copy(imagePath, newImagePath);
            File.Delete(imagePath);

            if (result)
            {
                LastImageDateTime = DateTime.Now;
                PreprocessedImagesCount++;
                IsSaved = false;
            }
        }

        public void SaveDocument()
        {
            var folderPath = Path.Combine(Configuration.DirectoryPath, Configuration.GeneratedPDFFolderPath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var newFileName = $"{_fileName}{MergeService.AppData.CountGeneratedPDFFiles + 1}.pdf";
            _document.Save(Path.Combine(folderPath, newFileName));
            _document.Dispose();
            PreprocessedImagesCount = 0;
            LastImageDateTime = null;
            IsSaved = true;
        }

        private bool DrawImage(XGraphics gfx, string imagePath, DrawingParams drawingsParams)
        {
            try
            {
                XImage image = XImage.FromFile(imagePath);
                gfx.DrawImage(image, drawingsParams.X, drawingsParams.Y, drawingsParams.Width, drawingsParams.Height);
                image.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
