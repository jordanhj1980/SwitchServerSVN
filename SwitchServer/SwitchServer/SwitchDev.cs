using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Npgsql;

namespace SwitchServer
{
    /// <summary>
    /// 软交换设备基类
    /// </summary>
    abstract class SwitchDev
    {
        public string name;
        public string ip;
        public string port;
        public string index;
        public string state;
        public List<string> extidlist;
        public string reportstr;
        //public callsession callsessiondata;
        public List<ExtDevice> extlist;
        public SwitchManage switchmanage;
        public CommandSendHandler handler;
        public SwitchDev(SwitchManage switchmanage, SwitchInfo switchinfo, NpgsqlConnection conn)
        {
            this.switchmanage = switchmanage;
            this.name = switchinfo.name;
            this.ip = switchinfo.ip;
            this.port = switchinfo.port;
            this.index = switchinfo.index;
            extidlist = new List<string>();
        }
        public virtual void MessageParse(object message)
        {

        }
        /// <summary>
        /// 获取软交换包含的所有话机
        /// </summary>
        /// <param name="conn"></param>
        public void GetExtFromDB(NpgsqlConnection conn)
        {
            DataBaseCommand sqlcom = new DataBaseCommand(conn);
            extlist = sqlcom.GetExtList(Convert.ToInt16(this.index));
        }
        /// <summary>
        /// 判断命令的发起者是否属于该软交换
        /// </summary>
        /// <param name="extid"></param>
        /// <returns></returns>
        public bool IsMember(List<string> extid,out string type)
        {
            ExtDevice ext = null;
            foreach(string member in extid)
            {
                ext = this.extlist.Find(c => c.extid.Equals(member));
                if (ext == null)
                {
                    
                }
                else
                {
                    type = ext.type;
                    return true;
                }
            }
            type = "";
            return false;
        }
        public string GetLineidFromExtid(string extid)
        {
            ExtDevice extdevice;
            extdevice = this.extlist.Find(c => c.extid.Equals(extid));
            return extdevice.lineid;
        }
        public virtual void ThreadPostRequest(TypeData command)
        {

        }
        public virtual void CommandSend(List<string> extid,TypeData command)
        {

        }
    }
    /// <summary>
    /// 讯时软交换类
    /// </summary>
    class NewRockTech : SwitchDev
    {
        public NewRockTech(SwitchManage switchmanage,SwitchInfo switchinfo, NpgsqlConnection conn)
            : base(switchmanage, switchinfo, conn)
        {
            this.handler = new CommandSendHandler(CommandSend);
            this.switchmanage.CommandSendEvent += this.handler;

            GetExtFromDB(conn);
        }
        public override void CommandSend(List<string> extid,TypeData command)
        {
            string type;
            if(IsMember(extid,out type))
            {
                ThreadPostRequest(command);
            }
        }
        public override void ThreadPostRequest(TypeData command)
        {
            try
            {
                Thread t = new Thread(new ParameterizedThreadStart(PostRequest));
                t.Start(command);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 讯时软交换上报消息解析
        /// </summary>
        public override void MessageParse(object message)
        {
            TypeData RespData = (TypeData)message;
            string revdata = RespData.data;
            string datatype = RespData.type;
            string commandstr = "";
            TypeData commanddata;
            commanddata.clientsession = RespData.clientsession;
            string printstr = "";
            ReportMessage reportmessage;
            ControlRespond controlrespond = new ControlRespond();

            switch (datatype)
            {
                case "EVENT&CDR":
                    EventMessage ParseEventMessage = new EventMessage();
                    printstr = ParseEventMessage.ParseEventCdr(revdata);
                    //this.extid = ParseEventMessage.extid;
                    //this.state = ParseEventMessage.state;
                    //this.reportstr = ParseEventMessage.reportstr;
                    //this.extidlist.Clear();
                    //this.extidlist.AddRange(ParseEventMessage.extlist);
                    Console.WriteLine(ParseEventMessage.reportstr);

                    if ((ParseEventMessage.extlist.Count == 0) && (commanddata.clientsession != null) && (!ParseEventMessage.reportstr.Equals("")))
                    {
                        commanddata.clientsession.Send(ParseEventMessage.reportstr);
                    }

                    reportmessage = new ReportMessage();
                    reportmessage.extid.AddRange(ParseEventMessage.extlist);
                    reportmessage.message = ParseEventMessage.reportstr;
                    Program.clientmanage.ReportState(reportmessage);

                    switch (this.state)
                    {
                        case "INVITE"://有外线接入
                            AcceptCommand command = new AcceptCommand(ParseEventMessage.callsessiondata.visitorid);
                            commandstr = command.XmlCommandString;
                            commanddata.type = "Accept";
                            commanddata.data = commandstr;
                            //this.visitorid = ParseEventMessage.callsessiondata.visitorid;
                            ThreadPostRequest(commanddata);

                            //System.Threading.Thread.Sleep(1000);
                            ////QueueGroup command1 = new QueueGroup(ParseEventMessage.callsessiondata.visitorid, "1");
                            ////QueueExt command1 = new QueueExt(ParseEventMessage.callsessiondata.visitorid, "213");
                            //VisitorToExt command1 = new VisitorToExt(ParseEventMessage.callsessiondata.visitorid, "213");
                            //commandstr = command1.XmlCommandString;
                            //commanddata.type = "Assign";
                            //commanddata.data = commandstr;
                            //ThreadPostRequest(commanddata);
                            break;
                        case "INCOMING":
                            /*
                            VisitorToExt command1 = new VisitorToExt(ParseEventMessage.callsessiondata.visitorid, exttext.Text.Trim());
                            commandstr = command1.XmlCommandString;
                            commanddata.type = "Connect";
                            commanddata.data = commandstr;
                            this.visitorid = ParseEventMessage.callsessiondata.visitorid;
                            ThreadPostRequest(commanddata);
                            */
                            //QueueGroup command1 = new QueueGroup(ParseEventMessage.callsessiondata.visitorid, "1");
                            //QueueExt command1 = new QueueExt(ParseEventMessage.callsessiondata.visitorid, "212");
                            //commandstr = command1.XmlCommandString;
                            //commanddata.type = "Assign";
                            //commanddata.data = commandstr;
                            //ThreadPostRequest(commanddata);
                            break;
                        default:
                            break;
                    }
                    break;
                case "QueryExt":
                    QueryExtTrunkRespond ParseQueryExt = new QueryExtTrunkRespond();
                    printstr = ParseQueryExt.ParseExtRespond(revdata);
                    Console.WriteLine(revdata);
                    //string state = ParseQueryExt.ext.state;
                    AssignExtAttr(ParseQueryExt.ext);
                    reportmessage = new ReportMessage ();
                    //reportmessage.extid = ParseQueryExt.ext.extid;
                    reportmessage.extid.Add(ParseQueryExt.ext.extid);
                    //reportmessage.message = "STATE#" + state+"#" + ParseQueryExt.ext.extid;
                    reportmessage.message = ParseQueryExt.reportstr;
                    Program.clientmanage.ReportState(reportmessage);
                    //判断话机是否为转接状态，用于夜服功能
                    if ((ParseQueryExt.ext.Fwd_Type != null) && (ParseQueryExt.ext.Fwd_Type.Equals("1")))
                    {
                    }
                    break;
                case "QueryTrunk":
                    QueryExtTrunkRespond ParseQueryTrunk = new QueryExtTrunkRespond();
                    printstr = ParseQueryTrunk.ParseTrunkRespond(revdata);
                    Console.WriteLine(revdata);
                    //string state = ParseQueryExt.ext.state;
                    AssignExtAttr(ParseQueryTrunk.ext);
                    reportmessage = new ReportMessage ();
                    //reportmessage.extid = ParseQueryExt.ext.extid;
                    reportmessage.extid.Add(ParseQueryTrunk.ext.extid);
                    //reportmessage.message = "STATE#" + state+"#" + ParseQueryExt.ext.extid;
                    reportmessage.message = ParseQueryTrunk.reportstr;
                    Program.clientmanage.ReportState(reportmessage);
                    break;
                case "Transfer":
                    printstr = "收到Transfer应答";
                    break;
                case "Call":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseCallRespond(revdata);
                    if (reportmessage.message.Length > 1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }
                    break;
                case "CallOut":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseCallOutRespond(revdata);
                    if (reportmessage.message.Length > 1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }
                    break;
                case "Visitor":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseVisitorRespond(revdata);
                    if (reportmessage.message.Length > 1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }
                    break;
                case "Clear":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseClearRespond(revdata);
                    if (reportmessage.message.Length > 1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }     
                    break;
                case "Bargein":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseBargeinRespond(revdata);
                    if (reportmessage.message.Length>1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }                    
                    break;
                case "Monitor":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseMonitorRespond(revdata);
                    if (reportmessage.message.Length>1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }                    
                    break;
                case "NightServiceOn":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseNightServiceOnRespond(revdata);
                    if (reportmessage.message.Length>1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }                    
                    break;
                case "NightServiceOff":
                    controlrespond = new ControlRespond();
                    reportmessage = new ReportMessage();
                    Console.WriteLine(revdata);
                    //reportmessage.extid.Add(controlrespond.extid);
                    reportmessage.message = controlrespond.ParseNightServiceOffRespond(revdata);
                    if (reportmessage.message.Length > 1)
                    {
                        commanddata.clientsession.Send(reportmessage.message);//谁发的指令，应答还给谁。
                        Console.WriteLine("回复应答：" + reportmessage.message);
                    }
                    break;
                case "QueryDeviceInfo":
                    QueryDeviceRespond ParseQueryDevice = new QueryDeviceRespond();
                    printstr = ParseQueryDevice.ParseQueryRespond(revdata);
                    DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

                    sqlcom.UpdateSwitchDev(this.index, ParseQueryDevice.ExtList);

                    Program.switchmanage.UpdataSwitchList();

                    break;

                default:
                    Console.WriteLine("发现未解析数据！！！\n");

                    printstr = "发现未解析数据！！！\n";
                    printstr += revdata;
                    break;
            }
            Console.WriteLine(printstr);
        }
        private void AssignExtAttr(ExtDevice extdevice)
        {
            int index;
            index = this.extlist.FindIndex(c => c.extid.Equals(extdevice.extid));
            if(index>=0)
            {
                this.extlist.RemoveAt(index);
                this.extlist.Insert(index, extdevice);
            }
            else
            {
                Console.WriteLine("未找到对应的分机");
            }
            //ext = this.extlist.Find(c => c.extid.Equals(extid));
        }

        private void PostRequest(object o)
        {
            TypeData PostData = (TypeData)o;
            string xmlstr = PostData.data;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.2.218:80");
            string url = "http://" + this.ip + ":" + this.port;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentType = "text/html";
            request.ContentLength = Encoding.UTF8.GetByteCount(xmlstr);
            request.Proxy = null;
            //request.Timeout = 1000 * 1000;
            Stream myRequestStream;

            try
            {
                myRequestStream = request.GetRequestStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            //请求发送

            StreamWriter myStreamWriter = new StreamWriter(myRequestStream);
            Console.WriteLine("发送指令：");
            Console.WriteLine(xmlstr);
            myStreamWriter.Write(xmlstr);
            
            myStreamWriter.Close();

            //if (PostData.type.Equals("Transfer"))
            //{
            //    PostData.type = "EVENT&CDR";
            //}
            //接收消息
            try
            {
                Stream stre = request.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(stre);
                string resultstr = sr.ReadToEnd();
                sr.Close();
                PostData.data = resultstr;
            }
            //catch (Exception e)
            catch
            {
                //Console.WriteLine(e.Message);
            }
            try
            {
                Thread t = new Thread(new ParameterizedThreadStart(MessageParse));
                t.Start(PostData);
            }
            catch
            {

            }
        }
    }
}
