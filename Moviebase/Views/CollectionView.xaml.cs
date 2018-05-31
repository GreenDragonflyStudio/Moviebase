using Moviebase.ViewModels;
using System.Windows.Controls;
using Ninject;

namespace Moviebase.Views
{
    /// <summary>
    /// Interaction logic for CollectionViewModel.xaml
    /// </summary>
    public partial class CollectionView : UserControl
    {
        public CollectionView()
        {
            InitializeComponent();
            var viewModel = App.Kernel.Get<CollectionViewModel>();
            this.DataContext = viewModel;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }
    }
}