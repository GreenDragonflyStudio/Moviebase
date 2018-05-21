﻿using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace Moviebase.Services.Entities.Internals
{
    public class MovieSearchRoot
    {
        public int page { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
        public List<MovieSearchResult> results { get; set; }
    }
}
