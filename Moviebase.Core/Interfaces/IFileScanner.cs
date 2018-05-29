using System.Collections.Generic;
using System.IO;

namespace Moviebase.Core.Components
{
    public interface IFileScanner
    {
        IEnumerable<FileInfo> Scan();
    }
}