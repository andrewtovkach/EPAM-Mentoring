namespace MergeWinService.Interfaces
{
    public interface IDirectoryWatcher
    {
        void Start();
        void ProcessFile(string fullPath);
    }
}
