using System;
using System.IO;

namespace Moviebase.DAL.Entities
{
    public class MediaFile
    {
        public int Id { get; set; }

        public string Hash { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        public DateTime LastSync { get; set; }
        public string FullPath { get; set; }
        public bool Synced { get; set; }

        public MediaFile()
        {
        }
        
        public bool Equals(MediaFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Hash, other.Hash) &&  string.Equals(FullPath, other.FullPath);
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
                hashCode = (hashCode * 397) ^ (FullPath != null ? FullPath.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
