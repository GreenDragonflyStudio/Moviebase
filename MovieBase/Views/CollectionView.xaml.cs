using MovieBase.ViewModels;
using System.Windows.Controls;

namespace MovieBase.Views
{
    /// <summary>
    /// Interaction logic for CollectionViewModel.xaml
    /// </summary>
    public partial class CollectionView : UserControl
    {
        public CollectionView()
        {
            InitializeComponent();
            var viewModel = new CollectionViewModel();
            this.DataContext = viewModel;
            this.Loaded += (sender, args) => viewModel.Start();
            this.Unloaded += (sender, args) => viewModel.Stop();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }
    }
}