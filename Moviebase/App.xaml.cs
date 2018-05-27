using System.Windows;
using Moviebase.Core;
using Moviebase.Core.Components;
using Ninject;

namespace Moviebase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly IKernel Kernel = new StandardKernel(new MoviebaseModule());

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // container
   

            // window
            Current.MainWindow = Kernel.Get<MainWindow>();
            Current.MainWindow.Show();
        }
    }
}