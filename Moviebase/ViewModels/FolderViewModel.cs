using System.Collections.ObjectModel;
using Moviebase.DAL.Entities;

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