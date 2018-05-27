using Moviebase.DAL.Entities;

namespace Moviebase.Core.Components
{
    public interface IFileOrganizer
    {
        string Organize(string filePath, Movie movie);
    }
}