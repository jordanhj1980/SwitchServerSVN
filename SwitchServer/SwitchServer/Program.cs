using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Newtonsoft.Json;
using SuperWebSocket;
namespace SwitchServer
{
    class Program
    {
        //http服务器，用于接收软交换的信息
        public static HttpServer httpserver;
        //websocket服务器，用于接收客户端的信息
        public static SimpleWebSocketServer websocketserver;  
        //Npgsql数据库对象
        public static NpgsqlConnection conn;
        //在线用户列表，记录已登录用户的相关信息
        //public static List<LogUser> loguserlist = new List<LogUser>();
        //软交换列表
        //public static List<SwitchDev> switchlist = new List<SwitchDev>();
        //话机列表
        //public static List<ExtDevice> extlist = new List<ExtDevice>();//ExtList
        //中继列表
        //public static List<TrunkDevice> TrunkList = new List<TrunkDevice>();
        //客户端管理实体
        public static ClientManage clientmanage = new ClientManage();
        //软交换管理实体
        public static SwitchManage switchmanage;// = new SwitchManage();
        static void Main(string[] args)
        {
            httpserver = new HttpServer("192.168.2.101", "80");
            httpserver.StartHttpServer();
            websocketserver = new SimpleWebSocketServer("192.168.2.101","1020");
            websocketserver.Start();
            string connString = @"Host=localhost;Port=5432;Username=postgres;Password=hj;Database=dispatch";
            conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                Console.Write("数据库已连接");
                Console.WriteLine(connString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库连接失败");
                Console.WriteLine(ex.Message);
                return;
            }

            Initialize();
            Console.ReadLine();
            //while (true) ;//cpu占用率高！！！
        }
        /// <summary>
        /// 根据数据库内容初始化软交换对象
        /// </summary>
        static void Initialize()
        {
            switchmanage = new SwitchManage(conn);


            //test getallkeyboard
            //DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            ////string respondstr;
            
            //GetAllKeyboardsp responddata;
            //sqlcom.GetAllKeyboard(out responddata);
            //string jason = JsonConvert.SerializeObject(responddata);
            //-------------test addeditsw
            //AddEditSW temp = new AddEditSW();
            //temp.name = "xf";
            //temp.index = "26";
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //WebSocketSession session = new WebSocketSession();
            ////new ClientMessageParse(session).ParseEditSW(jasonstr);
            //new ClientMessageParse(session).ParseQueryAllDev("123");
            //-------------test queryalldev
            //CommonCommand temp = new CommonCommand();
            //temp.index = "26";
            //temp.sequence = "1";
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //WebSocketSession session = new WebSocketSession();
            //new ClientMessageParse(session).QueryAllDev(jasonstr);

            //-------------test addeditsw
            //AddEditSW temp = new AddEditSW();
            //temp.name = "xf";
            //temp.ip = "192.168.1.111";
            //temp.port = "1000";
            //temp.index = "";
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //WebSocketSession session = new WebSocketSession();
            //new ClientMessageParse(session).ParseAddSW(jasonstr);

            //----------------test editalldev
            //EditAllDev temp = new EditAllDev();
            //temp.index = "26";
            //temp.sequence = "123";
            //DevStruct temp1 = new DevStruct();
            //temp1.callno = "204";
            //temp1.description = "204";
            //DevStruct temp2 = new DevStruct();
            //temp2.callno = "205";
            //temp2.description = "205";
            //temp.devlist = new List<DevStruct>();
            //temp.devlist.Add(temp1);
            //temp.devlist.Add(temp2);
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).ParseEditAllDev(jasonstr);
            //--------------test adduser
            //AddEditUser temp = new AddEditUser();
            //temp.sequence = "123";
            //temp.name = "xf1";
            //temp.password = "xf1";
            //temp.privilege = "1";
            //temp.role = "2";
            //temp.status = "1";
            //temp.description = "1";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).ParseAddUser(jasonstr);
            //---------------test deluser
            //DelUser temp = new DelUser();
            //temp.name = "xf1";
            //temp.sequence = "123";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).ParseDelUser(jasonstr);
            //-------------test edituser
            //AddEditUser temp = new AddEditUser();
            //temp.name = "xz11";
            //temp.description = "12321412414";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).EditUser(jasonstr);
            //------------test addkeyboard
            //AddKeyBoard temp = new AddKeyBoard();
            //List<DevStruct> hotlist = new List<DevStruct>();
            //List<DevStruct> memberlist = new List<DevStruct>();
            //List<Group> grouplist = new List<Group>();
            //DevStruct member = new DevStruct();
            //Group group = new Group();
            //member.callno = "204";
            //temp.hotlinelist = new List<DevStruct>();
            //temp.hotlinelist.Add(member);

            //member = new DevStruct();
            //member.callno = "220";
            //group.memberlist = new List<DevStruct>();
            //group.memberlist.Add(member);
            //group.groupname = "test";
            //temp.grouplist = new List<Group>();
            //temp.grouplist.Add(group);


            //temp.name = "keyboard5";
            //temp.sequence = "123";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).AddKeyboard(jasonstr);
            //-----------test delkeyboard
            //DelKeyBoard temp = new DelKeyBoard();
            //temp.index = "24";
            //temp.sequence = "123";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).DelKeyboard(jasonstr);
            //----------test editkeyboard
            //AddKeyBoard temp = new AddKeyBoard();
            //List<DevStruct> hotlist = new List<DevStruct>();
            //List<DevStruct> memberlist = new List<DevStruct>();
            //List<Group> grouplist = new List<Group>();
            //DevStruct member = new DevStruct();
            //Group group = new Group();

            //temp.hotlinelist = new List<DevStruct>();

            ////member.callno = "220";
            ////member.name = "220";
            ////member.description = "220";
            ////temp.hotlinelist.Add(member);
            //member = new DevStruct();
            //member.callno = "204";
            //member.name = "204";
            //member.description = "204";
            //temp.hotlinelist.Add(member);

            
            //group.memberlist = new List<DevStruct>();
            //group.groupname = "test";
            //group.index = "12";

            //member = new DevStruct();
            //member.callno = "220";
            //member.description = "220";
            //group.memberlist.Add(member);

            ////member = new DevStruct();
            ////member.callno = "213";
            ////member.description = "213";
            ////group.memberlist.Add(member);

            
            //temp.grouplist = new List<Group>();
            //temp.grouplist.Add(group);

            //group = new Group();
            //group.groupname = "test1";
            //group.memberlist = new List<DevStruct>();
            //group.index = "15";
            //member = new DevStruct();
            //member.callno = "230";
            //group.memberlist.Add(member);
            //member = new DevStruct();
            //member.callno = "240";
            //group.memberlist.Add(member);

            //temp.grouplist.Add(group);



            //temp.name = "edit";
            //temp.sequence = "123";

            //temp.index = "29";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).EditKeyboard(jasonstr);
            //--------------test getuser
            //GetUser temp = new GetUser();
            //temp.sequence = "123";
            //WebSocketSession session = new WebSocketSession();
            //string jasonstr = JsonConvert.SerializeObject(temp);
            //new ClientMessageParse(session).GetUser(jasonstr);

        }
    }
}
