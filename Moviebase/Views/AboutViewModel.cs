namespace Moviebase.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        private static string mVersion = "0.5.11";

        public string AppVersion { get; } = mVersion;
    }
}