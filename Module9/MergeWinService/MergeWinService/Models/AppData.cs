using System;

namespace MergeWinService.Models
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
