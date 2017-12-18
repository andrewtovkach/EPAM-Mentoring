﻿using Cache.Lib;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Log.Lib;
using MergeWinService.Interfaces;

namespace MergeWinService
{
    public static class MergeServiceFactory
    {
        public static IMergeService GetNewMergeService()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<LoggingInterceptor>()
            );

            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<CacheInterceptor>()
            );

            container.Register(
                Component.For<IPDFGenerator>()
                    .ImplementedBy<PDFGenerator>()
                    .DependsOn(Dependency.OnValue("fileName", Configuration.PDFFileName))
                    .LifestyleTransient().Interceptors<LoggingInterceptor, CacheInterceptor>()
            );

            string _appDataFilePath = "appData.json";

            container.Register(
                Component.For<IAppDataManager>()
                    .ImplementedBy<AppDataManager>()
                    .DependsOn(Dependency.OnValue("fileName", _appDataFilePath))
                    .LifestyleTransient().Interceptors<LoggingInterceptor, CacheInterceptor>()
              );

            container.Register(
                Component.For<IFileNameParser>()
                    .ImplementedBy<FileNameParser>()
                    .LifestyleTransient().Interceptors<LoggingInterceptor, CacheInterceptor>()
              );

            var pdfGenerator = container.Resolve<IPDFGenerator>();
            var appDataManager = container.Resolve<IAppDataManager>();
            var fileNameParser = container.Resolve<IFileNameParser>();

            container.Register(
                Component.For<IDirectoryWatcher>()
                    .ImplementedBy<DirectoryWatcher>()
                    .DependsOn(Dependency.OnValue("directoryPath", Configuration.DirectoryPath), 
                               Dependency.OnValue("pdfGenerator", pdfGenerator),
                               Dependency.OnValue("fileNameParser", fileNameParser))
                    .LifestyleTransient().Interceptors<LoggingInterceptor, CacheInterceptor>()
              );

            var directoryWatcher = container.Resolve<IDirectoryWatcher>();

            container.Register(
                Component.For<IMergeService>()
                    .ImplementedBy<MergeService>()
                    .DependsOn(Dependency.OnValue("pdfGenerator", pdfGenerator), 
                               Dependency.OnValue("directoryWatcher", directoryWatcher),
                               Dependency.OnValue("appDataManager", appDataManager))
                    .LifestyleTransient().Interceptors<LoggingInterceptor, CacheInterceptor>()
              );

            return container.Resolve<IMergeService>();
        }
    }
}
