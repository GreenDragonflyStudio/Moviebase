using System.Collections.Generic;
using System.IO;

namespace Moviebase.Core.Components.MediaScanner
{
    public interface IFileScanner
    {
        IEnumerable<FileInfo> Scan();
    }
}