using Moviebase.Core.Components;
using System;

namespace Moviebase.Core.Models
{
    public class MatchFoundEventArgs : EventArgs
    {
        #region Fields

        private readonly float _accuracy;

        #endregion Fields

        #region Constructors

        public MatchFoundEventArgs(AnalyzedItem item, Movie res, float accuracy)
        {
            LocalFile = item;
            Movie = res;
            _accuracy = accuracy;
        }

        #endregion Constructors

        #region Properties

        public bool Cancel { get; set; }
        public bool? IsMatch { get; set; }

        public AnalyzedItem LocalFile { get; }

        public double MatchAccuracy { get { return _accuracy; } }
        public Movie Movie { get; }
        public int Progress { get; set; }
        public int Total { get; set; }

        #endregion Properties
    }
}