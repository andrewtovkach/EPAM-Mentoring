using System.IO;
using System.Text.RegularExpressions;
using Log.Lib;
using MergeWinService.Interfaces;
using MergeWinService.Models;

namespace MergeWinService
{
    [Logging]
    public class FileNameParser: IFileNameParser
    {
        public FileData GetFileNameData(string fileName)
        {
            var fileData = new FileData();

            var fileExt = Path.GetExtension(fileName);
            fileData.Extension = fileExt;

            if (fileExt != null)
            {
                fileName = fileName.Replace(fileExt, "");
            }
            var listStrs = fileName.Split('_');

            if (listStrs.Length <= 1)
                return fileData;

            fileData.Prefix = listStrs[0];
            fileData.Number = listStrs[1];

            return fileData;
        }

        public bool IsValidFileName(string fileName, string pattern)
        {
            return Regex.IsMatch(fileName, pattern);
        }
    }
}
