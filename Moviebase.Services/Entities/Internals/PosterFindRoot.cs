using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace Moviebase.Services.Entities.Internals
{
    public class PosterFindRoot
    {
        public int id { get; set; }
        public List<Poster> posters { get; set; }
    }
}
