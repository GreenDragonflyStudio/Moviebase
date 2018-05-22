using System;

namespace Moviebase.DAL.Entities
{
    public class Folder
    {
        public int FolderId { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public bool AutoSync { get; set; }
        public DateTime LastSync { get; set; }
        public string Tag { get; set; }
        public int MovieCount { get; set; }
    }
}
