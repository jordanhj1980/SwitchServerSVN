using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace manageclientwpf
{
    public class ExtDevice : NotifyObject
    {
        private bool _ischecked = false;
        public bool DevSelected
        {
            get { return _ischecked; }
            set
            {
                if (_ischecked != value)
                {
                    _ischecked = value;
                    RaisePropertyChanged("DevSelected");
                }

            }
        }
        private string _callno;
        public string callno
        {
            get { return _callno; }
            set
            {
                if (_callno != value)
                {
                    _callno = value;
                    RaisePropertyChanged("callno");
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
        private string _type;
        public string type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChanged("type");
                }
            }
        }
        private string _level;
        public string level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    RaisePropertyChanged("level");
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
    }
    public class AllDev
    {
        public string sequence;
        public ObservableCollection<ExtDevice> DevList;
        public AllDev()
        {
            DevList = new ObservableCollection<ExtDevice>();
        }
    }
}
