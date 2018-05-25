using System;

namespace Moviebase.Core.Models
{
    public class MediaFile
    {
        #region Properties

        public MediaBinding Binding { get; set; }
        public long Bytes { get; set; }
        public string Hash { get; set; }
        public string Key { get; set; }

        public DateTime LastModifiedUtc { get; set; }
        public string Path { get; set; }

        #endregion Properties
    }

    public class MediaFoler
    {
        public string Path { get; set; }
    }
}