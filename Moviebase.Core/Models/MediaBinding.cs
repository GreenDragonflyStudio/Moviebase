namespace Moviebase.Core.Models
{
    public class MediaBinding
    {
        #region Properties

        public string Hash { get; set; }

        public Movie Movie { get; set; }
        public int? MovieId { get; set; }

        #endregion Properties
    }
}