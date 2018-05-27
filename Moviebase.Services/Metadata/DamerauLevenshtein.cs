using System;

namespace Moviebase.Services.Metadata
{
    public static class DamerauLevenshtein
    {
        //public static int Calculate(string s, string t)
        //{
        //    var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

        //    var matrix = new int[bounds.Height, bounds.Width];

        //    for (int height = 0; height < bounds.Height; height++) { matrix[height, 0] = height; }
        //    for (int width = 0; width < bounds.Width; width++) { matrix[0, width] = width; }

        //    for (int height = 1; height < bounds.Height; height++)
        //    {
        //        for (int width = 1; width < bounds.Width; width++)
        //        {
        //            int cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
        //            int insertion = matrix[height, width - 1] + 1;
        //            int deletion = matrix[height - 1, width] + 1;
        //            int substitution = matrix[height - 1, width - 1] + cost;

        //            int distance = Math.Min(insertion, Math.Min(deletion, substitution));

        //            if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
        //            {
        //                distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
        //            }

        //            matrix[height, width] = distance;
        //        }
        //    }

        //    return matrix[bounds.Height - 1, bounds.Width - 1];
        //}

        public static int Calculate(string source1, string source2)
        {
            var source1Length = source1.Length;

            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length

            if (source1Length == 0)

                return source2Length;

            if (source2Length == 0)

                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length

            for (var i = 0; i <= source1Length; matrix[i, 0] = i++)
            {
                // Init Matrix
            }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++)
            {
                // Init Matrix
            }

            // Calculate rows and collumns distances

            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                                   Math.Min(matrix[i - 1, j] + 1,
                                            matrix[i, j - 1] + 1),
                                            matrix[i - 1, j - 1] + cost);
                }
            }

            // return result
            return matrix[source1Length, source2Length];
        }
        
    }
}