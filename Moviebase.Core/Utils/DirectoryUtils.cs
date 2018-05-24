using System.IO;
using System.Linq;

namespace Moviebase.Core.Utils
{
    public static class DirectoryUtils
    {
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !dir.GetFiles().Any() && !dir.GetDirectories().Any();
        }

        public static bool IsEmpty(string directoryPath)
        {
            return new DirectoryInfo(directoryPath).IsEmpty();
        }
    }
}