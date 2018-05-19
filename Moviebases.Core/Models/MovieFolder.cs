namespace Moviebase.Models
{
    public class MovieFolder
    {
        public string Location { get; set; }
        public bool AutoSync { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int Count { get; } = 1;
    }
}