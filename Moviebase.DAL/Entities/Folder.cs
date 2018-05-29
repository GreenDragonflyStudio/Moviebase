using System;

namespace Moviebase.DAL.Entities
{
    public class Folder
    {
        public int Id { get; set; }

        public string Path { get; set; }
        public DateTime LastSync { get; set; }
        public bool Synced { get; set; }
    }
}