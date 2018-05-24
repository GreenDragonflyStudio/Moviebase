using System.IO;

namespace Moviebase.Core.Components.MediaOrganizer
{
    public interface IFolderCleaner
    {
        void Clean(DirectoryInfo directory);
    }
}