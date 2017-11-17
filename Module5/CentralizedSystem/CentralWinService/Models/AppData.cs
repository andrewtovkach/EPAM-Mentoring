using System;

namespace CentralWinService.Models
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
