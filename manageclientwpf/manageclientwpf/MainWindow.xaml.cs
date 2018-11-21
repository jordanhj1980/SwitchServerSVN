using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocket4Net;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace manageclientwpf
{
    public partial class MainWindow : Window
    {
        private WebSocket websocket = null;
        private MainwindowData testdata;
        public ObservableCollection<KeyBoard> keyboardlist;
        //public KeyBoard SelectedData;
        public MainWindow()
        {
            InitializeComponent();
            this.testdata = new MainwindowData();
            DataContext = this.testdata;
            keyboardlist = new ObservableCollection<KeyBoard>();
            Keyboardlist.ItemsSource = keyboardlist;

            keyboardview.Visibility = System.Windows.Visibility.Hidden;
            groupview.Visibility = System.Windows.Visibility.Hidden;
        }

        private void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string webserveruri = "ws://" + ipaddr.Text.Trim();
                websocket = new WebSocket(webserveruri);
                websocket.Opened += websocket_Opened;
                websocket.Closed += websocket_Closed;
                websocket.MessageReceived += websocket_MessageReceived;
                websocket.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceivedEventArgs responseMsg = (MessageReceivedEventArgs)e; //接收服务端发来的消息
            string strMsg = responseMsg.Message;

            Console.WriteLine(strMsg);
            testdata.OutputStr += strMsg;

            TypeData level1data = ParseType(strMsg);
            if(level1data.type.Equals("MAN"))
            {
                TypeData level2data = ParseType(level1data.data);
                if(level2data.type.Equals("GETALLKEYBOARD"))
                {
                    AllKeyBoard allkeydata;
                    allkeydata = JsonConvert.DeserializeObject<AllKeyBoard>(level2data.data);
                    //keyboardlist = allkeydata.keyboardlist;
                    foreach (KeyBoard element in allkeydata.keyboardlist)
                    {
                        //keyboardlist.Add(element);
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            keyboardlist.Add(element);
                        }), null);
                    }
                }
            }
        }


        private void websocket_Closed(object sender, EventArgs e)
        {
            //websocket.Send("一个客户端 下线");
            testdata.LogState = true;
            testdata.LogButtonText = "登录";
            Console.WriteLine("client is closed!!!");
        }
        void websocket_Opened(object sender, EventArgs e)
        {
            //websocket.Send("一个客户端 上线");
            testdata.LogButtonText = "已登录";
            testdata.LogState = false;
            Console.WriteLine("client is opened!!!");
            List<string> logdata = new List<string>();
            if (testdata.UserName == "" || testdata.PassWord == "")
            {
                MessageBox.Show("用户名，密码不能为空！！");
            }
            else
            {
                logdata.Add(testdata.UserName);
                logdata.Add(testdata.PassWord);
                string logstr = "LOG#" + JsonConvert.SerializeObject(logdata);
                try
                {
                    websocket.Send(logstr);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void outputclear_Click(object sender, RoutedEventArgs e)
        {
            this.testdata.OutputStr = "";
        }

        private void inputbtn_Click(object sender, RoutedEventArgs e)
        {
            websocket.Send(testdata.InputStr);
        }

        public TypeData ParseType(string datastr)
        {
            TypeData typedata;
            int indexstart = 0;
            int indexend = 0;
            indexend = datastr.IndexOf('#');
            //获取第一个#前的数据
            typedata.type = datastr.Substring(indexstart, indexend - indexstart);
            //获取第一个#后的数据
            indexstart = indexend + 1;
            typedata.data = datastr.Substring(indexstart);

            return typedata;
        }

        private void bindingdata_Click(object sender, RoutedEventArgs e)
        {
            Keyboardlist.ItemsSource = keyboardlist;
        }

        private void adddata_Click(object sender, RoutedEventArgs e)
        {
            KeyBoard keytemp = new KeyBoard();
            keytemp.name = "test";
            keytemp.ip="192.168.2.123";
            keyboardlist.Add(keytemp);
        }

        private void Keyboardlist_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if(tvi.Header is KeyBoard)
            {
                var modelkey = tvi.Header as KeyBoard;
                testdata.SelectedKey = modelkey;
                groupview.Visibility = System.Windows.Visibility.Hidden;
                keyboardview.Visibility = System.Windows.Visibility.Visible;
            }
            else if(tvi.Header is Group)
            {
                var modelgroup = tvi.Header as Group;
                testdata.SelectedGroup = modelgroup;
                keyboardview.Visibility = System.Windows.Visibility.Hidden;
                groupview.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
    public struct TypeData
    {
        public string type;
        public string data;
    }
}
