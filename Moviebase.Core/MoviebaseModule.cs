using Moviebase.Core.Components;
using Moviebase.Services.Title;
using Ninject.Modules;

namespace Moviebase.Core
{
    public class MoviebaseModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileAnalyzer>().To<FileAnalyzer>();
            Bind<IFileOrganizer>().To<FileOrganizer>();
            Bind<IFileScanner>().To<BasicFileScanner>();
            Bind<IFolderCleaner>().To<FolderCleaner>();

            Bind<ITitleProvider>().To<CompositeTitleProvider>();
        }
    }
}
