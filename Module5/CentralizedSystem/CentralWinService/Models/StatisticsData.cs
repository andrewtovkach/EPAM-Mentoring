using System;

namespace CentralWinService.Models
{
    [Serializable]
    public class StatisticsData
    {
        public int TimeInterval { get; set; }
        public int MaxProcessNumber { get; set; }
    }
}
