using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moviebase.Core.Components
{
    public class BasicFileScanner : IFileScanner
    {
        public List<string> Extensions { get; set; } = new List<string> {"mkv", "mp4", "avi"};

        public IEnumerable<string> Scan(string path)
        {
            if (File.Exists(path)) return new[] { path };
            if (!Directory.Exists(path)) return new[] {""};

            var ff = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return ff.Where(x => Extensions.Any(x.EndsWith));
        }
    }
}