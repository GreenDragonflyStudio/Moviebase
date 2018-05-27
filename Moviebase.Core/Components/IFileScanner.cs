using System.Collections.Generic;

namespace Moviebase.Core.Components
{
    public interface IFileScanner
    {
        IEnumerable<string> Scan(string path);
    }
}