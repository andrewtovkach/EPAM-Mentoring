using MergeWinService.Models;

namespace MergeWinService.Interfaces
{
    public interface IAppDataManager
    {
        void WrireToFile(AppData appData);
        AppData ReadFromFile();
    }
}
