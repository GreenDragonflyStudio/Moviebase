using System;
using Moviebase.Core.Components;
using Moviebase.DAL.Entities;

namespace Moviebase.Core
{
    public class MatchFoundEventArgs : EventArgs
    {
        public MatchFoundEventArgs(string filePath, string title, float accuracy)
        {
            FilePath = filePath;
            Title = title;
            MatchAccuracy = accuracy;
        }

        public string FilePath { get; set; }
        public string Title { get; set; }
        public double MatchAccuracy { get; }

        public bool Cancel { get; set; }
        public bool? IsMatch { get; set; }
        
        public int Progress { get; set; }
        public int Total { get; set; }
    }
}