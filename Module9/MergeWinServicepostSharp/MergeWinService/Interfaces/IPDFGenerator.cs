using System;

namespace MergeWinService.Interfaces
{
    public interface IPDFGenerator
    {
        bool Addimage(string imagePath);
        void SaveDocument();
        int PreprocessedImagesCount { get; set; }
        DateTime? LastImageDateTime { get; set; }
        bool IsSaved { get; set; }
    }
}
