﻿using System.Windows;

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