using System.IO;
using System.Text.RegularExpressions;

namespace MergeWinService
{
    public class FileNameParser
    {
        private string _fileName;

        public FileNameParser(string fileName)
        {
            _fileName = fileName;
        }

        public FileData GetFileNameData()
        {
            var fileData = new FileData();

            var fileExt = Path.GetExtension(_fileName);
            fileData.Extension = fileExt;

            if (fileExt != null)
            {
                _fileName = _fileName.Replace(fileExt, "");
            }
            var listStrs = _fileName.Split('_');

            if (listStrs.Length <= 1)
                return fileData;

            fileData.Prefix = listStrs[0];
            fileData.Number = listStrs[1];

            return fileData;
        }

        public bool IsValidFileName(string pattern)
        {
            return Regex.IsMatch(_fileName, pattern);
        }
    }
}
