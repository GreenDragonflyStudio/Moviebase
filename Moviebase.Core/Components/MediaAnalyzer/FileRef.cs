using System;
using System.IO;

namespace Moviebase.Core.Components.MediaAnalyzer
{
    public sealed class FileRef : IEquatable<FileRef>
    {
        #region Constructors

        public FileRef()
        {
        }

        public FileRef(FileInfo finfo)
        {
            Path = finfo.FullName;
            LastModifiedUtc = finfo.LastWriteTimeUtc;
            Bytes = finfo.Length;
        }

        #endregion Constructors

        #region Properties

        public long Bytes { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public string Path { get; set; }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            return obj.Equals(this);
        }

        public bool Equals(FileRef other)
        {
            return string.Equals(Path, other.Path) && LastModifiedUtc.Equals(other.LastModifiedUtc) &&
                   Bytes == other.Bytes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Path?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ LastModifiedUtc.GetHashCode();
                hashCode = (hashCode * 397) ^ Bytes.GetHashCode();
                return hashCode;
            }
        }

        #endregion Methods
    }
}