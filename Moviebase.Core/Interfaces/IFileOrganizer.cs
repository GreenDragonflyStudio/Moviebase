using Moviebase.Core.Models;
using System.IO;

namespace Moviebase.Core.Components
{
    public interface IFileOrganizer
    {
        string Organize(FileInfo itemPath, Movie movie);
    }
}