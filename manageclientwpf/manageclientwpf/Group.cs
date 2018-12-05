using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace manageclientwpf
{
    public class Group : NotifyObject
    {
        public Group()
        {
            memberlist = new ObservableCollection<ExtDevice>();
        }
        private string _index;
        public string index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChanged("index");
                }
            }
        }
        private string _groupname;
        public string groupname
        {
            get { return _groupname; }
            set
            {
                if (_groupname != value)
                {
                    _groupname = value;
                    RaisePropertyChanged("groupname");
                }
            }
        }
        private string _column;
        public string column
        {
            get { return _column; }
            set
            {
                if (_column != value)
                {
                    _column = value;
                    RaisePropertyChanged("column");
                }
            }
        }
        private string _description;
        public string description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("description");
                }
            }
        }
        private ObservableCollection<ExtDevice> _memberlist;
        public ObservableCollection<ExtDevice> memberlist
        {

            get { return _memberlist; }
            set
            {
                SetAndNotifyIfChanged("memberlist", ref _memberlist, value);
            }
        }
    }
}
