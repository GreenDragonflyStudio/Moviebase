using MediaInfoDotNet;

namespace Moviebase.Core.Utils
{
    public static class MediaHelper
    {
        public static MediaFile GetMediaInfo(string fullPath)
        {
            return new MediaFile(fullPath);
        }
    }
}
