using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace manageclientwpf
{
    class DataGridPageViewModel :ViewModel
    {
        private ICommand _firstPageCommand;

        public ICommand FirstPageCommand
        {
            get
            {
                return _firstPageCommand;
            }

            set
            {
                _firstPageCommand = value;
            }
        }

        private ICommand _previousPageCommand;

        public ICommand PreviousPageCommand
        {
            get
            {
                return _previousPageCommand;
            }

            set
            {
                _previousPageCommand = value;
            }
        }

        private ICommand _nextPageCommand;

        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand;
            }

            set
            {
                _nextPageCommand = value;
            }
        }

        private ICommand _lastPageCommand;

        public ICommand LastPageCommand
        {
            get
            {
                return _lastPageCommand;
            }

            set
            {
                _lastPageCommand = value;
            }
        }

        private int _pageSize;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if(_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged("PageSize");
                }
            }
        }

        private int _currentPage;

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if(_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPage;

        public int TotalPage
        {
            get
            {
                return _totalPage;
            }

            set
            {
                if(_totalPage != value)
                {
                    _totalPage = value;
                    OnPropertyChanged("TotalPage");
                }
            }
        }

        private ObservableCollection<UserLog> _userlogview;

        public ObservableCollection<UserLog> UserlogView
        {
            get
            {
                return _userlogview;
            }
            set
            {
                if (_userlogview != value)
                {
                    _userlogview = value;
                    OnPropertyChanged("UserlogView");
                }
            }
        }

        public List<UserLog> _userloglist;

        public DataGridPageViewModel(List<UserLog> pagedata)
        {
            _currentPage = 1;

            _pageSize = 10;



            _userloglist = pagedata;

            _totalPage = _userloglist.Count / _pageSize;
            if ((_userloglist.Count % _pageSize) != 0)
            {
                _totalPage += 1;
            }
               
            _userlogview = new ObservableCollection<UserLog>();

            List<UserLog> result = _userloglist.Take(_pageSize).ToList();

            _userlogview.Clear();

            _userlogview.AddRange(result);

            _firstPageCommand = new DelegateCommand(FirstPageAction);

            _previousPageCommand = new DelegateCommand(PreviousPageAction);

            _nextPageCommand = new DelegateCommand(NextPageAction);

            _lastPageCommand = new DelegateCommand(LastPageAction);
        }

        private void FirstPageAction()
        {
            CurrentPage = 1;

            var result = _userloglist.Take(_pageSize).ToList();

            _userlogview.Clear();

            _userlogview.AddRange(result);
        }

        private void PreviousPageAction()
        {
            if(CurrentPage == 1)
            {
                return;
            }

            List<UserLog> result = new List<UserLog>();

            if(CurrentPage == 2)
            {
                result = _userloglist.Take(_pageSize).ToList();
            }
            else
            {
                result = _userloglist.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            _userlogview.Clear();

            _userlogview.AddRange(result);

            CurrentPage--;
        }

        private void NextPageAction()
        {
            if(CurrentPage == _totalPage)
            {
                return;
            }

            List<UserLog> result = new List<UserLog>();

            result = _userloglist.Skip(CurrentPage * _pageSize).Take(_pageSize).ToList();

            _userlogview.Clear();

            _userlogview.AddRange(result);

            CurrentPage++;
        }

        private void LastPageAction()
        {
            CurrentPage = TotalPage;

            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = _userloglist.Count - skipCount;

            var result = _userloglist.Skip(skipCount).Take(takeCount).ToList();

            _userlogview.Clear();

            _userlogview.AddRange(result);
        }
    }
}
