using System.IO;

namespace Moviebase.Core.Components
{
    public interface IFolderCleaner
    {
        void Clean(DirectoryInfo directory);
    }
}