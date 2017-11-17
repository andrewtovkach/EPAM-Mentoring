using System;
using System.Xml.Serialization;

namespace InputWinService.Models
{
    public enum Type
    {
        ImageProcess,
        Statistics,
        UpdateSettings
    }

    public enum Status
    {
        Submitted,
        InProgress,
        Completed,
        Failed
    }

    public enum SystemStatus
    {
        WaitNewFiles,
        ProcessFiles
    }

    [XmlInclude(typeof(FileTransferingData))]
    [XmlInclude(typeof(StatisticsTransferingData))]
    public class TransferingData
    {
    }

    [Serializable]
    public class FileTransferingData : TransferingData
    {
        public string FilePath { get; set; }
    }

    [Serializable]
    public class StatisticsTransferingData: TransferingData
    {
        public int TimeInterval { get; set; }
        public SystemStatus SystemStatus { get; set; }
        public int MaxProcessNumber { get; set; }
    }

    [Serializable]
    public class TransferData
    {
        public Guid Id { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public TransferingData Data { get; set; }
    }
}
