using System;

namespace Moviebase.Core
{
    public class AnalyzedFile
    {
        public AnalyzedFile(string filePath)
        {
            FullPath = filePath;
        }

        public bool IsKnown { get; set; }
        public string FullPath { get; }

        public int MovieId { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
    }
}