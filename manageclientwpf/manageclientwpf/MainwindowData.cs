using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using manageclientwpf;
using System.Collections.ObjectModel;
namespace manageclientwpf
{
    class MainwindowData:NotifyObject
    {
        private KeyBoard _selectedkey;
        public KeyBoard SelectedKey
        {
            get { return _selectedkey; }
            set
            {
                if (_selectedkey != value)
                {
                    _selectedkey = value;
                    RaisePropertyChanged("SelectedKey");
                }
            }
        }
        private Group _selectedgroup;
        public Group SelectedGroup
        {
            get { return _selectedgroup; }
            set
            {
                if (_selectedgroup != value)
                {
                    _selectedgroup = value;
                    RaisePropertyChanged("SelectedGroup");
                }
            }
        }
        private string _username;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }
        private string _password;
        public string PassWord
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged("PassWord");
                }
            }
        }

        private string _inputstr;
        public string InputStr
        {
            get { return _inputstr; }
            set
            {
                if (_inputstr != value)
                {
                    _inputstr = value;
                    RaisePropertyChanged("InputStr");
                }
            }
        }

        private string _outputstr;
        public string OutputStr
        {
            get { return _outputstr; }
            set
            {
                if (_outputstr != value)
                {
                    _outputstr = value;
                    RaisePropertyChanged("OutputStr");
                }
            }
        }
        private string _ipaddr;
        public string IpAddr
        {
            get { return _ipaddr; }
            set
            {
                if (_ipaddr != value)
                {
                    _ipaddr = value;
                    RaisePropertyChanged("IpAddr");
                }
            }
        }
        private bool _logstate;
        public bool LogState
        {
            get { return _logstate; }
            set
            {
                if (_logstate != value)
                {
                    _logstate = value;
                    RaisePropertyChanged("LogState");
                }
            }
        }
        private string _logbuttontext;
        public string LogButtonText
        {
            get { return _logbuttontext; }
            set
            {
                if (_logbuttontext != value)
                {
                    _logbuttontext = value;
                    RaisePropertyChanged("LogButtonText");
                }
            }
        }
        private ObservableCollection<KeyBoard> _keyboardlist;
        public ObservableCollection<KeyBoard> keyboardlist
        {
            get { return _keyboardlist; }
            set
            {
                SetAndNotifyIfChanged("keyboardlist", ref _keyboardlist, value);
            }
        }
        public MainwindowData()
        {
            UserName = "hj";
            PassWord = "hj";
            IpAddr = "192.168.2.101:1020";
            InputStr = "MAN#GETALLKEYBOARD#{\"sequence\":\"123\"}";
            OutputStr = "";
            LogState = true;
            LogButtonText = "登录";
            SelectedKey = new KeyBoard();
            SelectedKey.name = "1234455";
            //keyboardlist = new ObservableCollection<KeyBoard>();
        }
    }
}
