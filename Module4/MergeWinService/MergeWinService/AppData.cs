using System;

namespace MergeWinService
{
    [Serializable]
    public class AppData
    {
        public int CountGeneratedPDFFiles { get; set; }

        public AppData()
        {
            CountGeneratedPDFFiles = 0;
        }
    }
}
