namespace MergeWinService
{
    public class MergeService
    {
        private readonly PDFGenerator _pdfGenerator;
        private readonly AppDataManager _appDataManager;
        public static AppData AppData { get; set; }

        public MergeService(PDFGenerator pdfGenerator, AppDataManager appDataManager)
        {
            _pdfGenerator = pdfGenerator;
            _appDataManager = appDataManager;
        }

        public void Start()
        {
            AppData = _appDataManager.ReadFromFile();
            new DirectoryWatcher(Configuration.DirectoryPath, _pdfGenerator);
        }

        public void Stop()
        {
            _pdfGenerator.SaveDocument();
            AppData.CountGeneratedPDFFiles++;
            _appDataManager.WrireToFile(AppData);
        }
    }
}
