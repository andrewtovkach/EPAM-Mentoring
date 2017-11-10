using System.IO;
using Topshelf;

namespace MergeWinService
{
    public static class ConfigureService
    {
        public static void Configure()
        {
            if (!Directory.Exists(Configuration.DirectoryPath))
            {
                Directory.CreateDirectory(Configuration.DirectoryPath);
            }

            string _appDataFilePath = "appData.json";
            AppDataManager appDataManager = new AppDataManager(_appDataFilePath);

            var pdfGenerator = new PDFGenerator(Configuration.PDFFileName);

            HostFactory.Run(configure =>
            {
                configure.Service<MergeService>(service =>
                {
                    service.ConstructUsing(s => new MergeService(pdfGenerator, appDataManager));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                configure.RunAsLocalSystem();
                configure.SetServiceName(Configuration.ServiceName);
                configure.SetDisplayName(Configuration.DisplayName);
                configure.SetDescription(Configuration.Description);
            });
        }
    }
}
