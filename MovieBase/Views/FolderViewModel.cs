using System.Collections.ObjectModel;

namespace MovieBase.ViewModels
{
    public class FolderViewModel : BindableBase
    {
        private string _filterstring;

        public string FilterText
        {
            get
            {
                return _filterstring;
            }
            set
            {
                _filterstring = value;
                OnPropertyChanged(FilterText);
            }
        }

        public ObservableCollection<Models.MovieFolder> Folders { get; set; }
    }
}