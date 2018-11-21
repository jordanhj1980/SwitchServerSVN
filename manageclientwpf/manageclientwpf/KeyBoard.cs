using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace manageclientwpf
{
    public class KeyBoard : NotifyObject
    {
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
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }
        private string _mac;
        public string mac
        {
            get { return _mac; }
            set
            {
                if (_mac != value)
                {
                    _mac = value;
                    RaisePropertyChanged("mac");
                }
            }
        }
        private string _ip;
        public string ip
        {
            get { return _ip; }
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    RaisePropertyChanged("ip");
                }
            }
        }
        private ObservableCollection<Group> _grouplist;
        public ObservableCollection<Group> grouplist
        {
            get { return _grouplist; }
            set
            {
                SetAndNotifyIfChanged("grouplist", ref _grouplist, value);
            }
        }
        private ObservableCollection<ExtDevice> _hotlinelist;
        public ObservableCollection<ExtDevice> hotlinelist
        {
            get { return _hotlinelist; }
            set
            {
                SetAndNotifyIfChanged("hotlinelist", ref _hotlinelist, value);
            }
        }
    }
    public class AllKeyBoard
    {
        public string sequence;
        public ObservableCollection<KeyBoard> keyboardlist;
        public AllKeyBoard()
        {
            keyboardlist = new ObservableCollection<KeyBoard>();
        }
    }
}
