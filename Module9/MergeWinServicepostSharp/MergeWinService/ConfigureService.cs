using System.IO;
using Log.Lib;
using MergeWinService.Interfaces;
using Topshelf;

namespace MergeWinService
{
    [Logging]
    public static class ConfigureService
    {
        public static void Configure()
        {
            if (!Directory.Exists(Configuration.DirectoryPath))
            {
                Directory.CreateDirectory(Configuration.DirectoryPath);
            }


            HostFactory.Run(configure =>
            {
                configure.Service<IMergeService>(service =>
                {
                    service.ConstructUsing(s => MergeServiceFactory.GetNewMergeService());
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
