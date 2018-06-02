using System;

namespace Moviebase.Services.Helpers
{
    public static class IOHelpers
    {
        private static readonly string[] SizeSuffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB

        public static string BytesToString(long byteCount)
        {
            if (byteCount == 0) return $"0{SizeSuffix[0]}";
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{Math.Sign(byteCount) * num}{SizeSuffix[place]}";
        }
    }
}