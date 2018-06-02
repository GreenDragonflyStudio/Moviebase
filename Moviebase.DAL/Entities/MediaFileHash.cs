namespace Moviebase.DAL.Entities
{
    public class MediaFileHash
    {
        public int Id { get; set; }

        public string Hash { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }
}
