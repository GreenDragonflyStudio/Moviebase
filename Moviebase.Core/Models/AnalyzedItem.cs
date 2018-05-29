using System;
using System.IO;

namespace Moviebase.Core.Components
{
    public class AnalyzedItem
    {
        public AnalyzedItem(FileInfo file)
        {
            Path = file;
        }

        public FileInfo Path { get; }
        public bool IsKnown { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int? Year { get; set; }
        public string Hash { get; set; }
        public int? MovieId { get; set; }
        public TimeSpan Duration { get; set; }
    }
}