using System;
using Topshelf;

namespace CentralWinService
{
    public static class ConfigureService
    {
        public static void Configure()
        {
            var pdfGenerator = new PDFGenerator(Configuration.PDFFileName);
            var appDataFileWatcher = new AppDataFileWatcher(AppDomain.CurrentDomain.BaseDirectory);

            HostFactory.Run(configure =>
            {
                configure.Service<CentralService>(service =>
                {
                    service.ConstructUsing(s => new CentralService(pdfGenerator, appDataFileWatcher));
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
