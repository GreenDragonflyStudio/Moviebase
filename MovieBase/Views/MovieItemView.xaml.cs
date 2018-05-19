using System.Windows;
using System.Windows.Controls;

namespace MovieBase.Views
{
    /// <summary>
    /// Interaction logic for MovieItemView.xaml
    /// </summary>
    public partial class MovieItemView : UserControl
    {
        public MovieItemView()
        {
            InitializeComponent();
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(MovieItemView), new PropertyMetadata(true));
    }
}