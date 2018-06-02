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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new Views.MainWindow();
            MainWindow.Show();
        }
    }
}