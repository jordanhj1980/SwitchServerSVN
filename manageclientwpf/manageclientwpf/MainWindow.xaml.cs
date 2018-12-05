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
        private DataGridPageViewModel pagedata;
        //public ObservableCollection<KeyBoard> keyboardlist;
        //public KeyBoard SelectedData;
        public MainWindow()
        {
            InitializeComponent();
            this.testdata = new MainwindowData();
            DataContext = this.testdata;
            

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

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        testdata.KeyboardList.Clear();
                        foreach (KeyBoard element in allkeydata.keyboardlist)
                        {
                            testdata.KeyboardList.Add(element);
                        }
                    }), null);

                }
                else if(level2data.type.Equals("GETALLREGISTERDEV"))
                {
                    AllDev alldev;
                    alldev = JsonConvert.DeserializeObject<AllDev>(level2data.data);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        testdata.AllDevList = alldev.DevList;
                    }), null);
                }
            }
            else if(level1data.type.Equals("CMD"))
            {
                TypeData level2data = ParseType(level1data.data);
                if(level2data.type.Equals("GetUserlog"))
                {
                    //List<UserLog> templist = JsonConvert.DeserializeObject<List<UserLog>>(level2data.data);
                    testdata.UserlogList = JsonConvert.DeserializeObject<List<UserLog>>(level2data.data);
                    pagedata = new DataGridPageViewModel(testdata.UserlogList);
                    Dispatcher.Invoke(new Action(() => 
                    {
                        datagridpage.DataContext = pagedata;
                    }));
                    //datagridpage.DataContext = pagedata;
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

        private void delkey_Click(object sender, RoutedEventArgs e)
        {
            testdata.KeyboardList.Remove(testdata.SelectedKey);
        }

        private void addkey_Click(object sender, RoutedEventArgs e)
        {
            testdata.SelectedKey = new KeyBoard();
            testdata.SelectedKey.name = "新增键盘";
            keyboardview.Visibility = System.Windows.Visibility.Visible;
        }

        private void addgroup_Click(object sender, RoutedEventArgs e)
        {
            testdata.SelectedKey.grouplist.Add(new Group());
        }

        private void delgroup_Click(object sender, RoutedEventArgs e)
        {
            testdata.SelectedKey.grouplist.Remove(testdata.SelectedGroup);
        }

        private void Keyboardlist_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if(tvi.Header is KeyBoard)
            {
                var modelkey = tvi.Header as KeyBoard;
                //testdata.SelectedKey = modelkey;
                testdata.SelectedKey = ObjectCopier.Clone<KeyBoard>(modelkey);
                groupview.Visibility = System.Windows.Visibility.Hidden;
                keyboardview.Visibility = System.Windows.Visibility.Visible;
            }
            else if(tvi.Header is Group)
            {
                var modelgroup = tvi.Header as Group;
                //testdata.SelectedGroup = modelgroup;
                testdata.SelectedGroup = ObjectCopier.Clone<Group>(modelgroup);
                keyboardview.Visibility = System.Windows.Visibility.Hidden;
                groupview.Visibility = System.Windows.Visibility.Visible;

                var tv = VisualTreeHelper.GetParent(tvi);
                System.Windows.Controls.StackPanel tvpanel = tv as StackPanel;
                ItemsPresenter ip = tvpanel.TemplatedParent as ItemsPresenter;
                TreeViewItem tvii = ip.TemplatedParent as TreeViewItem;//这是父节点
                //testdata.SelectedKey = tvii.Header as KeyBoard;
                testdata.SelectedKey = ObjectCopier.Clone<KeyBoard>(tvii.Header as KeyBoard);

            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            scrollViewer.RaiseEvent(eventArg);
        }
        
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string cmdstr = "MAN#ADDKEYBOARD#" + JsonConvert.SerializeObject(testdata.SelectedKey);
            websocket.Send(cmdstr);
        }

        private void getalldeskbtn_Click(object sender, RoutedEventArgs e)
        {
            websocket.Send("MAN#GETALLKEYBOARD#{\"sequence\":\"123\"}");
        }

        private void getalldevbtnbtn_Click(object sender, RoutedEventArgs e)
        {
            websocket.Send("MAN#GETALLREGISTERDEV#{\"sequence\":\"123\"}");
        }

        private void addmember_Click(object sender, RoutedEventArgs e)
        {
            testdata.SelectedGroup.memberlist.Clear();
            foreach(ExtDevice t in testdata.AllDevList)
            {
                if(t.DevSelected==true)
                {
                    testdata.SelectedGroup.memberlist.Add(t);
                }
            }
            testdata.SelectedGroup.RaisePropertyChanged("memberlist");
        }

        private void grouplistgrid_Selected(object sender, RoutedEventArgs e)
        {
            foreach(ExtDevice element in testdata.AllDevList)
            {
                //element = testdata.AllDevList.Find(c => c.clientsession.SessionID.Equals(clientsession.SessionID));
                bool bFind = testdata.SelectedGroup.memberlist.Any<ExtDevice>(p => p.callno == element.callno);
                if(bFind)
                {
                    element.DevSelected = true;
                }
            }
        }

        private void addkeydev_Click(object sender, RoutedEventArgs e)
        {
            testdata.SelectedKey.hotlinelist.Clear();
            foreach (ExtDevice t in testdata.AllDevList)
            {
                if (t.DevSelected == true)
                {
                    testdata.SelectedKey.hotlinelist.Add(t);
                }
            }
            testdata.SelectedKey.RaisePropertyChanged("hotlinelist");
        }

        private void assigngroupbtn_Click(object sender, RoutedEventArgs e)
        {
            AssignGroupCMD assigngroup = new AssignGroupCMD();
            assigngroup.distribution = "group";
            assigngroup.devlist.Add("204");
            assigngroup.devlist.Add("205");
            string cmdstr = "CMD#AssignGroup#" + JsonConvert.SerializeObject(assigngroup);
            testdata.InputStr += cmdstr;
            websocket.Send(cmdstr);
        }

        private void cleargroupbtn_Click(object sender, RoutedEventArgs e)
        {
            AssignGroupCMD assigngroup = new AssignGroupCMD();
            assigngroup.distribution = "group";
            //assigngroup.devlist.Add("204");
            //assigngroup.devlist.Add("205");
            string cmdstr = "CMD#AssignGroup#" + JsonConvert.SerializeObject(assigngroup);
            testdata.InputStr += cmdstr;
            websocket.Send(cmdstr);
        }

        private void getuserlog_Click(object sender, RoutedEventArgs e)
        {
            string cmdstr = "CMD#GetUserlog#ALL";
            websocket.Send(cmdstr);
        }
    }
    public struct TypeData
    {
        public string type;
        public string data;
    }
}
