using Moviebase.ViewModels;
using System.Windows.Controls;

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
            this.DataContext = new CollectionViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }
    }
}