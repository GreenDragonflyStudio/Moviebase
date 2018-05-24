using Moviebase.DAL.Entities;
using System.Collections.ObjectModel;

namespace Moviebase.ViewModels
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

        public ObservableCollection<Folder> Folders { get; set; }
    }
}