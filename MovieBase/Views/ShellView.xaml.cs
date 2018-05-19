using MahApps.Metro.Controls;
using Moviebase.Helper;
using Moviebase.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mMenuItem = Moviebase.ViewModels.MenuItem;

namespace Moviebase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigation.Frame = new Frame() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            Navigation.Frame.Navigated += SplitViewFrame_OnNavigated;

            // Navigate to the home page.
            this.Loaded += (sender, args) => Navigation.Navigate(new Uri("Views/HomeView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SplitViewFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            this.HamburgerMenuControl.Content = e;
            this.HamburgerMenuControl.SelectedItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetItem(e.Uri);
            this.HamburgerMenuControl.SelectedOptionsItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetOptionsItem(e.Uri);
            GoBackButton.Visibility = Navigation.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            Navigation.GoBack();
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            var menuItem = e.InvokedItem as mMenuItem;
            if (menuItem != null && menuItem.IsNavigation)
            {
                Navigation.Navigate(menuItem.NavigationDestination, menuItem);
            }
        }
    }
}