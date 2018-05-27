using System;
using System.IO;

namespace Moviebase.DAL.Entities
{
    public class MediaFile
    {
        public string Hash { get; set; }
        public long Length { get; set; }

        public DateTime LastModifiedUtc { get; set; }
        public string Path { get; set; }

        public MediaFile()
        {
        }

        public static MediaFile Create(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return new MediaFile
            {
                Path = filePath,
                LastModifiedUtc = fileInfo.LastWriteTimeUtc,
                Length = fileInfo.Length
            };
        }

        public bool Equals(MediaFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Hash, other.Hash) && LastModifiedUtc.Equals(other.LastModifiedUtc) && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MediaFile)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Hash != null ? Hash.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LastModifiedUtc.GetHashCode();
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
