namespace Moviebase.Core
{
    public class MoviebaseSettings
    {
        public bool Reorganize { get; set; }
        public string TargetPath { get; set; }

        public string RenameTemplate { get; set; }
        public bool DeleteEmptyFolders { get; set; }
    }
}