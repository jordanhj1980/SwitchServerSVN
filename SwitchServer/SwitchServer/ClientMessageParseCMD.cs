using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperWebSocket;
using System.Threading;
using System.Net;
using System.IO;

namespace SwitchServer
{
    partial class ClientMessageParse
    {
        /// <summary>
        /// 解析CMD消息
        /// </summary>
        public bool ParseCMDMessage(string datastr)
        {
            //Command command;
            TypeData message = ParseType(datastr);
            TypeData commanddata = ParseType(message.data);
            if (message.type.Equals("CMD"))
            {
                switch (commanddata.type)
                {
                    //case "QueryAllExt"://解析获取所有话机指令，给软交换发送对应的指令
                    //    ParseQueryAllExt(commanddata.data);             
                    //    break;
                    case "GETSTATE"://解析查询话机状态指令，给软交换发送对应的指令
                        ParseGetState(commanddata.data);
                        break;
                    case "Bargein"://解析强插指令，给软交换发送对应的指令
                        ParseBargein(commanddata.data);
                        break;
                    case "Clear"://解析强拆指令，给软交换发送对应的指令
                        ParseClear(commanddata.data);
                        break;
                    //case "GetAllExt"://获取用户的所有可管理设备
                    //    break;
                    case "Call"://解析拨号指令，给软交换发送对应的指令
                        ParseCall(commanddata.data);
                        break;
                    case "Visitor"://来电转接分机
                        ParseVisitor(commanddata.data);
                        break;
                    case "CallOut"://拨打外线电话
                        ParseCallOut(commanddata.data);
                        break;
                    case "Monitor"://监听
                        ParseMonitor(commanddata.data);
                        break;
                    case "NightServiceOn"://夜服开启
                        ParseNightServiceOn(commanddata.data);
                        break;
                    case "NightServiceOff"://夜服关闭
                        ParseNightServiceOff(commanddata.data);
                        break;
                    case "GETCDR"://获取通话记录
                        ParseGetCdr(commanddata.data);
                        break;
                    case "GetUserlog":
                        ParseGetUserlog(commanddata.data);
                        break;
                    case "Hold"://通话保持
                        ParseHold(commanddata.data);
                        break;
                    case "Unhold"://取消保持
                        ParseUnhold(commanddata.data);
                        break;
                    case "MenuToExt"://语音接入分机
                        ParseMenuToExt(commanddata.data);
                        break;
                    case "AssignGroup"://分组设置
                        ParseAssignGroup(commanddata.data);
                        break;
                    case "GETPHONEBOOK":
                        CMDGetPhoneBook(commanddata.data);
                        break;

                    default:
                        Console.WriteLine("非可解析控制指令：" + commanddata.type);
                        break;
                }
                return true;
            }
            else
            {
                Console.WriteLine("非CMD控制指令！！");
                return false;
            }

        }

        public bool ParseNightServiceOn(string data)
        {
            CallData call;
            SwitchDev switchdevice;
            //call = JsonConvert.DeserializeObject<CallData>(data);

            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            string type;
            switchdevice = Program.switchmanage.GetSwitchFromExtid(extlist,out type);
            string lineid = switchdevice.GetLineidFromExtid(call.fromid);
            AssignNightService command = new AssignNightService(lineid, call.toid);
            command.On();
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "NightService";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public bool ParseNightServiceOff(string data)
        {
            CallData call;
            SwitchDev switchdevice;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            string type;
            switchdevice = Program.switchmanage.GetSwitchFromExtid(extlist,out type);
            if(switchdevice==null)
            {
                Console.WriteLine("找不到软交换!!!");
            }
            string lineid = switchdevice.GetLineidFromExtid(call.fromid);
            AssignNightService command = new AssignNightService(lineid, call.toid);
            command.Off();
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "NightService";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        //public void ParseQueryAllExt(string data)
        //{
        //    Command command = new QueryDeviceInfo();
        //    string commandstr = command.XmlCommandString;
        //    TypeData com;
        //    com.type = "QueryDeviceInfo";
        //    com.data = commandstr;
        //    Program.switchmanage.CommandSend("204", com);
        //}
        public void ParseClear(string data)
        {
            string extid = data;
            Command command = new ClearCommand(extid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Clear";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add(extid);
            Program.switchmanage.CommandSend(extlist, com);
        }
        public bool ParseMonitor(string data)
        {
            CallData call;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            Command command = new MonitorCommand(call.fromid, call.toid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Monitor";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public bool ParseBargein(string data)
        {
            CallData call;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            Command command = new BargeinCommand(call.fromid, call.toid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Bargein";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public void ParseGetState(string data)
        {
            List<string> extid = new List<string>();
            extid.Add(data);
            string type;
            SwitchDev switchdev = Program.switchmanage.GetSwitchFromExtid(extid,out type);
            if(type=="ext")
            {
                Command command = new QueryExt(data);
                string commandstr = command.XmlCommandString;
                TypeData com;
                com.type = "QueryExt";
                com.data = commandstr;
                com.clientsession = this.clientsession;
                switchdev.CommandSend(extid, com);
            }
            else if(type=="trunk")
            {
                Command command = new QueryTrunk(data);
                string commandstr = command.XmlCommandString;
                TypeData com;
                com.type = "QueryTrunk";
                com.data = commandstr;
                com.clientsession = this.clientsession;
                switchdev.CommandSend(extid, com);
            }

        }
        /// <summary>
        /// 解析CMD里的Call消息
        /// </summary>
        public bool ParseCall(string data)
        {
            CallData call;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            //获取软交换对象,根据fromid找软交换，根据toid判断命令类型
            string type;
            List<string> calllist = new List<string>();
            calllist.Add(call.fromid);
            SwitchDev srcswitch = Program.switchmanage.GetSwitchFromExtid(calllist, out type);
            calllist.Clear();
            calllist.Add(call.toid);
            SwitchDev desswitch = Program.switchmanage.GetSwitchFromExtid(calllist, out type);
            Command command;
            if(srcswitch==desswitch)
            {
                command = new ExtToExt(call.fromid, call.toid);
            }
            else
            {
                command = new ExtToOuter(call.fromid, null,call.toid);
            }

            
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Call";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);

            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        /// <summary>
        /// 来电转接分机
        /// </summary>
        /// <param name="data"></param>
        public bool ParseVisitor(string data)
        {
            CallData call;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            Command command = new VisitorToExt(call.fromid, call.toid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Visitor";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            //暂时通过toid判断是哪个软交换
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            extlist.Add(call.toid);
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public bool ParseCallOut(string data)
        {
            CallOut call;
            //call = JsonConvert.DeserializeObject<CallOut>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallOut>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            Command command = new ExtToOuter(call.fromid, call.trunkid, call.toid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Call";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            //暂时通过fromid判断是哪个软交换
            List<string> extlist = new List<string>();
            extlist.Add(call.fromid);
            extlist.Add(call.toid);
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        /// <summary>
        /// 获取所有通话记录
        /// </summary>
        /// <param name="data"></param>
        public void ParseGetCdr(string data)
        {
            List<CDR> cdrlist = new List<CDR>();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            cdrlist = sqlcom.GetCDR();

            string respondstr = "CMD#GETCDR#" + JsonConvert.SerializeObject(cdrlist);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
        }
        public void ParseGetUserlog(string data)
        {
            List<UserLog> userloglist = new List<UserLog> ();

            LogUser finduser;
            finduser = ClientManage.loguserlist.Find(c => c.clientsession.SessionID.Equals(this.clientsession.SessionID));

            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            userloglist = sqlcom.GetUserLog(finduser.name);
            string respondstr = "CMD#GetUserlog#" + JsonConvert.SerializeObject(userloglist);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
        }
        public void ParseHold(string data)
        {
            HoldCommand command = new HoldCommand(data);
            TypeData com;
            com.type = "NULL";
            com.data = command.XmlCommandString;
            com.clientsession = this.clientsession;
            Console.WriteLine(com.data);
            List<string> extlist = new List<string>();
            extlist.Add(data);
            Program.switchmanage.CommandSend(extlist, com); ;
        }
        public void ParseUnhold(string data)
        {
            UnholdCommand command = new UnholdCommand(data);
            TypeData com;
            com.type = "NULL";
            com.data = command.XmlCommandString;
            com.clientsession = this.clientsession;
            Console.WriteLine(com.data);
            List<string> extlist = new List<string>();
            extlist.Add(data);
            Program.switchmanage.CommandSend(extlist, com); ;
        }
        /// <summary>
        /// 语音菜单呼叫分机
        /// </summary>
        /// <param name="data"></param>
        public bool ParseMenuToExt(string data)
        {
            CallData call;
            //call = JsonConvert.DeserializeObject<CallData>(data);
            try
            {
                call = JsonConvert.DeserializeObject<CallData>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            MenuToExt command = new MenuToExt(call.fromid,call.toid);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Menu";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add(call.toid);
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public bool ParseSetMenu(string data)
        {
            SetMenu setmenu;
            //setmenu = JsonConvert.DeserializeObject<SetMenu>(data);
            try
            {
                setmenu = JsonConvert.DeserializeObject<SetMenu>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            AssignMenu command = new AssignMenu(setmenu.menuid,setmenu.voicefile);
            string commandstr = command.XmlCommandString;
            TypeData com;
            com.type = "Menu";
            com.data = commandstr;
            com.clientsession = this.clientsession;
            List<string> extlist = new List<string>();
            extlist.Add("204");
            Program.switchmanage.CommandSend(extlist, com);
            return true;
        }
        public bool ParseAssignGroup(string data)
        {
            AssignGroupCMD assigngroup = new AssignGroupCMD();
            try
            {
                assigngroup = JsonConvert.DeserializeObject<AssignGroupCMD>(data);
            }
            catch
            {
                string respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            if((assigngroup.devlist.Count==0)||(assigngroup.devlist==null))
            {
                ClearGroup command = new ClearGroup("2");
                string commandstr = command.XmlCommandString;
                TypeData com;
                com.type = "AssignGroup";
                com.data = commandstr;
                com.clientsession = this.clientsession;
                List<string> extlist = new List<string>();
                extlist.Add("204");
                Program.switchmanage.CommandSend(extlist, com);
                return true;
            }
            else
            {
                AssignGroup command = new AssignGroup("2", assigngroup.devlist, assigngroup.distribution);
                string commandstr = command.XmlCommandString;
                TypeData com;
                com.type = "AssignGroup";
                com.data = commandstr;
                com.clientsession = this.clientsession;
                List<string> extlist = new List<string>();
                extlist.AddRange(assigngroup.devlist);
                Program.switchmanage.CommandSend(extlist, com);
                return true;
            }
            
        }
    }
}