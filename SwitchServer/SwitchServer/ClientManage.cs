using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
using SwitchServer;
using Npgsql;
namespace SwitchServer
{
    public delegate void ReportStateHandler(ReportMessage message);
    public class ClientManage
    {
        //在线用户列表，记录已登录让控制台用户的相关信息
        public static List<LogUser> loguserlist = new List<LogUser>();
        //在线用户列表，记录已登录让控制台用户的相关信息
        public static List<LogUser> adminlist = new List<LogUser>();

        public event ReportStateHandler ReportStateEvent;
        public void ReportState(ReportMessage message)
        {
            if (ReportStateEvent != null)
            {
                ReportStateEvent(message);
            }
        }
        /// <summary>
        /// 在全局的用户列表Program.loglist中检查当前的sessionid是否已存在
        /// </summary>
        public bool SessionCheck(WebSocketSession clientsession,out string type)
        {
            LogUser finduser;
            finduser = loguserlist.Find(c => c.clientsession.SessionID.Equals(clientsession.SessionID));
            if (finduser != null)
            {
                type = "Control";//表示操作台用户
                return true;
            }
            else
            {
                finduser = adminlist.Find(c => c.clientsession.SessionID.Equals(clientsession.SessionID));
                if (finduser!=null)
                {
                    type = "Admin";
                    return true;
                }
                else
                {
                    type = "Error";
                    return false;
                }                
            }
        }
        /// <summary>
        /// 在全局的用户列表Program.loglist中删除对应sessionid的用户
        /// </summary>
        public void SessionDelete(WebSocketSession clientsession)
        {
            LogUser temp;
            if (loguserlist.Count > 0)
            {
                try
                {
                    temp = loguserlist.Where(c => c.clientsession.SessionID.Equals(clientsession.SessionID)).First();
                    temp.LogOut();
                    loguserlist.Remove(temp);
                }
                catch
                {

                }
            }
            if(adminlist.Count>0)
            {
                try
                {
                    temp = adminlist.Where(c => c.clientsession.SessionID.Equals(clientsession.SessionID)).First();
                    temp.LogOut();
                    adminlist.Remove(temp);
                }
                catch
                {

                }
            }
        }
        public bool AdminListCheck(LogInfo loginfo)
        {
            LogUser finduser;
            string respondstr;
            if (adminlist.Count != 0)
            {
                //在现有用户列表中进行查找
                finduser = adminlist.Find(delegate(LogUser c)
                {
                    return c.name.Equals(loginfo.name) && c.pwd.Equals(loginfo.pwd);
                });

                //如果找到表示已登录
                if (finduser != null)
                {
                    Console.WriteLine("用户已登录！！！");
                    respondstr = "LOG#Already";
                    loginfo.clientsession.Send(respondstr);
                    Console.WriteLine(respondstr);
                    return false;
                }
                //如果没找到表示新用户登录
                else
                {
                    //在用户列表中加入对应用户
                    Console.WriteLine("登录成功！！！");
                    respondstr = "LOG#Success#" + loginfo.type.ToString();
                    loginfo.clientsession.Send(respondstr);
                    Console.WriteLine(respondstr);
                    LogUser loguser = new LogUser(Program.clientmanage, loginfo, Program.conn);
                    loguser.GetGroupExtFromDB();
                    adminlist.Add(loguser);
                    return true;
                }
            }
            //表为空表示新用户
            else
            {
                Console.WriteLine("登录成功！！！");
                respondstr = "LOG#Success#" + loginfo.type.ToString();
                loginfo.clientsession.Send(respondstr);
                Console.WriteLine(respondstr);
                adminlist.Add(new LogUser(Program.clientmanage, loginfo, Program.conn));
                return true;
            }
        }
        public bool UserListCheck(LogInfo loginfo)
        {
            LogUser finduser;
            string respondstr;
            if (loguserlist.Count != 0)
            {
                //在现有用户列表中进行查找
                finduser = loguserlist.Find(delegate(LogUser c)
                {
                    return c.name.Equals(loginfo.name) && c.pwd.Equals(loginfo.pwd);
                });

                //如果找到表示已登录
                if (finduser != null)
                {
                    Console.WriteLine("用户已登录！！！");
                    respondstr = "LOG#Already";
                    loginfo.clientsession.Send(respondstr);
                    Console.WriteLine(respondstr);
                    return false;
                }
                //如果没找到表示新用户登录
                else
                {
                    //在用户列表中加入对应用户
                    Console.WriteLine("登录成功！！！");
                    respondstr = "LOG#Success#" + loginfo.type.ToString();
                    loginfo.clientsession.Send(respondstr);
                    Console.WriteLine(respondstr);
                    loguserlist.Add(new LogUser(Program.clientmanage, loginfo, Program.conn));

                    return true;
                }
            }
            //表为空表示新用户
            else
            {
                Console.WriteLine("登录成功！！！");
                respondstr = "LOG#Success#" + loginfo.type.ToString();
                loginfo.clientsession.Send(respondstr);
                Console.WriteLine(respondstr);
                loguserlist.Add(new LogUser(Program.clientmanage, loginfo, Program.conn));
                return true;
            }
        }
        /// <summary>
        /// 登录用户名，密码校验，维护Program.loglist
        /// </summary>
        public bool LogCheck(ref LogInfo loginfo)
        {

            DataBaseCommand sqlcmd = new DataBaseCommand(Program.conn);

            if (sqlcmd.LogInfoCheck(ref loginfo))
            {
                //用户名密码正确
                if(loginfo.type.Equals("1"))//管理用户
                {
                    loginfo.type = "Admin";
                    return AdminListCheck(loginfo);
                }
                else if(loginfo.type.Equals("2"))//控制台用户
                {
                    loginfo.type = "Control";
                    return UserListCheck(loginfo);
                }
                else
                {
                    Console.WriteLine("非法类型用户！！");
                    return false;
                }

            }
            //失败表示用户名和密码与数据库内容不一致，不允许登录
            else
            {
                Console.WriteLine("用户名或密码错误！！！");
                loginfo.clientsession.Send("LOG#Wrong");
                return false;
            }
            
        }
        public LogUser GetUserBySessionID(WebSocketSession session)
        {
            LogUser loguser;
            //ext = this.extlist.Find(c => c.extid.Equals(member));
            loguser = loguserlist.Find(c => c.clientsession.Equals(session));
            if(loguser!=null)
            {
                return loguser;
            }
            loguser = adminlist.Find(c => c.clientsession.Equals(session));
            if (loguser != null)
            {
                return loguser;
            }
            return loguser;
        }
    }
    public class LogUser
    {
        public string name;
        public string pwd;
        public string ip;
        public string type;
        public WebSocketSession clientsession;
        public ClientManage clientmanage;
        public ReportStateHandler handler;
        public List<GroupData> extlist;
        public NpgsqlConnection conn;
        public LogUser(ClientManage clientmanage,LogInfo loginfo,NpgsqlConnection conn)
        {
            this.clientmanage = clientmanage;
            this.name = loginfo.name;
            this.pwd = loginfo.pwd;
            this.ip = loginfo.ip;
            this.clientsession = loginfo.clientsession;
            this.conn = conn;
            this.handler = new ReportStateHandler(ReportState);
            this.clientmanage.ReportStateEvent += this.handler;//订阅状态上报事件

            //GetGroupExtFromDB(conn);//获取对应用户的管理成员
        }
        /// <summary>
        /// 用于在客户端退出时注销事件委托
        /// </summary>
        public void LogOut()
        {
            Console.WriteLine("用户退出！！！");
            this.clientmanage.ReportStateEvent -= this.handler;
        }
        /// <summary>
        /// 向客户端上报话机状态
        /// </summary>
        /// <param name="message"></param>
        public void ReportState(ReportMessage message)
        {
            if(IsMember(message.extid))
            {
                Console.WriteLine("向用户" + this.name + "上报状态" + message.message);
                clientsession.Send(message.message);
            }      
        }
        /// <summary>
        /// 获取该用户的成员
        /// </summary>
        /// <param name="conn"></param>
        public void GetGroupExtFromDB()
        {
            DataBaseCommand sqlcom = new DataBaseCommand(conn);
            this.extlist = sqlcom.GetGroupExt(this.name, this.pwd);
            this.extlist.AddRange(sqlcom.GetKeyExt(this.name, this.pwd));

            this.extlist.AddRange(sqlcom.GetTrunk());
        }
        /// <summary>
        /// 判断话机是否为本客户端可管理的成员
        /// </summary>
        /// <param name="extid"></param>
        /// <returns></returns>
        public bool IsMember(List<string> extid)
        {
            GroupData ext;
            ext.extid = "";
            if(this.extlist==null)
            {
                Console.WriteLine("this.extlist 为空");
                return false;
            }
            foreach(string member in extid)
            {
                ext = this.extlist.Find(c => c.extid.Equals(member));
                if (ext.extid == null)
                {
                    
                }
                else
                {
                    return true;
                }
            }
            return false;
            
        }

    }
}
