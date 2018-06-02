using Moviebase.Core.Components;
using Moviebase.Services.Title;
using Ninject.Modules;

namespace Moviebase.Core.App
{
    /// <summary>
    /// Defines Moviebase's services module for dependency injection.
    /// </summary>
    public class MoviebaseModule : NinjectModule
    {
        /// <inheritdoc />
        public override void Load()
        {
            // components
            Bind<IFileScanner>().To<FileScanner>();
            Bind<IDirectoryScanner>().To<DirectoryScanner>();
            Bind<IFileAnalyzer>().To<FileAnalyzer>();
            Bind<IFileOrganizer>().To<FileOrganizer>();
            Bind<IFolderCleaner>().To<FolderCleaner>();

            // services
            Bind<ITitleProvider>().To<CompositeTitleProvider>();
            Bind<IPathTokenizer>().To<PathTokenizer>();

            // main app
            Bind<IMoviebaseDAL>().To<MoviebaseDAL>().InSingletonScope();
            Bind<IMoviebaseApp>().To<MoviebaseApp>().InSingletonScope();
        }
    }
}
