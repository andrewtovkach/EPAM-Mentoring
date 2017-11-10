using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;

namespace MergeWinService
{
    public class PDFGenerator
    {
        private readonly string _fileName;
        private readonly PdfDocument _document;
         
        public PDFGenerator(string fileName)
        {
            _fileName = fileName;
            
            _document = new PdfDocument();
        }

        public void Addimage(string imagePath)
        {
            PdfPage page = _document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx, imagePath,
                new DrawingParams {
                    X = 50,
                    Y = 50,
                    Width = 500,
                    Height = 500
                });

            var processedImagesFolderPath = Path.Combine(Configuration.DirectoryPath, Configuration.ProcessedImagesFolderPath);
            var newImagePath = imagePath.Replace(Configuration.DirectoryPath, processedImagesFolderPath);

            if (!Directory.Exists(processedImagesFolderPath))
            {
                Directory.CreateDirectory(processedImagesFolderPath);
            }

            File.Copy(imagePath, newImagePath);
        }

        public void SaveDocument()
        {
            var folderPath = Path.Combine(Configuration.DirectoryPath, Configuration.GeneratedPDFFolderPath);

            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var newFileName = $"{_fileName}{MergeService.AppData.CountGeneratedPDFFiles+1}.pdf";
            _document.Save(Path.Combine(folderPath, newFileName));
            _document.Close();
        }

        private void DrawImage(XGraphics gfx, string imagePath, DrawingParams drawingsParams)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(imagePath, true);
            XImage image = XImage.FromGdiPlusImage(img);
            gfx.DrawImage(image, drawingsParams.X, drawingsParams.Y, drawingsParams.Width, drawingsParams.Height);
        }
    }
}
