using System.Windows;
using Moviebase.Core;
using Moviebase.Core.App;
using Moviebase.Core.Components;
using Moviebase.ViewModels;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Planning.Strategies;
using Ninject.Planning.Strategies;

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
            Kernel.Components.Add<IPlanningStrategy, AutoNotifyInterceptorRegistrationStrategy>();
            
            Kernel.Bind<CollectionViewModel>().ToSelf();
            
            // window
            Current.MainWindow = Kernel.Get<MainWindow>();
            Current.MainWindow.Show();
        }
    }
}