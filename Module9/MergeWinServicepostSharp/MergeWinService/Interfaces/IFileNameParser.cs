using MergeWinService.Models;

namespace MergeWinService.Interfaces
{
    public interface IFileNameParser
    {
        FileData GetFileNameData(string fileName);
        bool IsValidFileName(string fileName, string pattern);
    }
}
