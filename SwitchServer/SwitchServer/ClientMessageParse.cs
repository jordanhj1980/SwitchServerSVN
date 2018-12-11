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
    /// <summary>
    /// 客户端消息解析类
    /// </summary>
    partial class ClientMessageParse
    {
        public WebSocketSession clientsession;
        public ClientMessageParse(WebSocketSession session)
        {
            this.clientsession = session;
        }
        /// <summary>
        /// 解析客户端登录消息
        /// </summary>
        public bool ParseLOGMessage(string datastr,out string type,out string name)
        {
            TypeData command = ParseType((string)datastr);
            bool result;
            switch(command.type)
            {
                case "LOG":
                    LogInfo logdata = ParseLogInfo(command);
                    result = Program.clientmanage.LogCheck(ref logdata);
                    type=logdata.type;
                    name = logdata.name;
                    return result;
                default:
                    type = "0";
                    Console.WriteLine("非登录指令！！！");
                    name = "";
                    return false;              
            }
        }
        
        /// <summary>
        /// 解析接收客户端消息的类型
        /// </summary>
        public TypeData ParseType(string datastr)
        {
            TypeData typedata;
            typedata.type = "Heart";
            typedata.data = "Heart";
            typedata.clientsession = this.clientsession;
            int indexstart = 0;
            int indexend = 0;
            try
            {
                indexend = datastr.IndexOf('#');
                //获取第一个#前的数据
                typedata.type = datastr.Substring(indexstart, indexend - indexstart);
                //获取第一个#后的数据
                indexstart = indexend + 1;
                typedata.data = datastr.Substring(indexstart);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ParseType wrong:{0}", ex.Message);
            }
            

            return typedata;
        }
        /// <summary>
        /// 解析用户登录数据，name和pwd是多少
        /// </summary>
        public LogInfo ParseLogInfo(TypeData sdata)
        {
            List<string> namepwd = new List<string>();
            namepwd = JsonConvert.DeserializeObject<List<string>>(sdata.data);
            LogInfo loginfo = new LogInfo();
            loginfo.name = namepwd[0];
            loginfo.pwd = namepwd[1];
            loginfo.clientsession = this.clientsession;
            loginfo.ip = this.clientsession.RemoteEndPoint.ToString();

            return loginfo;
        }
        
        /// <summary>
        /// 从数据库中获取对应用户下的组和设备
        /// </summary>
        public List<GroupData> GetDeskGroup()
        {
            DataBaseCommand sqlcmd = new DataBaseCommand(Program.conn);
            List<GroupData> templist;
            LogUser finduser;
            finduser = ClientManage.loguserlist.Find(c => c.clientsession.SessionID.Equals(this.clientsession.SessionID));
            string name = finduser.name;
            string pwd = finduser.pwd;
            string desk_index = sqlcmd.GetDeskIndex(name, pwd);
            templist = sqlcmd.GetKeyExt(desk_index);
            //templist.AddRange(sqlcmd.GetTrunk());
            //templist.AddRange(sqlcmd.GetBroadCast(name, pwd));
            templist.AddRange(sqlcmd.GetGroupExt(desk_index));
            return templist;
        }
        public List<trunkdev> GetGroupTrunk()
        {
            DataBaseCommand sqlcmd = new DataBaseCommand(Program.conn);
            List<trunkdev> templist= new List<trunkdev> ();
            LogUser finduser;
            finduser = ClientManage.loguserlist.Find(c => c.clientsession.SessionID.Equals(this.clientsession.SessionID));
            string name = finduser.name;
            string pwd = finduser.pwd;
            string desk_index = sqlcmd.GetDeskIndex(name, pwd);
            sqlcmd.GetTrunkList(desk_index, out templist);
            return templist;
        }
        public List<Broadcast>GetGroupBroadcast()
        {
            DataBaseCommand sqlcmd = new DataBaseCommand(Program.conn);
            List<Broadcast> templist = new List<Broadcast>();
            LogUser finduser;
            finduser = ClientManage.loguserlist.Find(c => c.clientsession.SessionID.Equals(this.clientsession.SessionID));
            string name = finduser.name;
            string pwd = finduser.pwd;
            string desk_index = sqlcmd.GetDeskIndex(name, pwd);
            
            templist.AddRange(sqlcmd.GetBroadCast(desk_index));

            return templist;
        }
    }
}
