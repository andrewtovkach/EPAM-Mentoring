using System.IO;
using Topshelf;

namespace InputWinService
{
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
                configure.Service<InputService>(service =>
                {
                    service.ConstructUsing(s => new InputService());
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
