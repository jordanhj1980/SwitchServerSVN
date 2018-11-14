﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
using SuperSocket;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading;
namespace SwitchServer
{
    public class SimpleWebSocketServer
    {
        private string ip;
        private int port;
        private WebSocketServer ws = null;//SuperWebSocket中的WebSocketServer对象

        
        
        public SimpleWebSocketServer(string ip,string port)
        {
            this.ip = ip;
            this.port = Convert.ToInt32(port);
            ws = new WebSocketServer();//实例化WebSocketServer


            //添加事件侦听
            
            ws.NewSessionConnected += ws_NewSessionConnected;//有新会话握手并连接成功
            ws.SessionClosed += ws_SessionClosed;//有会话被关闭 可能是服务端关闭 也可能是客户端关闭
            ws.NewMessageReceived += ws_NewMessageReceived;//有新文本消息被接收
            ws.NewDataReceived += ws_NewDataReceived;//有新二进制消息被接收
        }
        void ws_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("{0:HH:MM:ss}  与客户端:{1}创建新会话", DateTime.Now, session.RemoteEndPoint);
        }

        void ws_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            LogUser loguser = Program.clientmanage.GetUserBySessionID(session);
            Program.clientmanage.SessionDelete(session);          
            if(loguser!=null)
            {
                DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
                sqlcom.InsertUserLog(loguser.name, "logout");
            }
            Console.WriteLine("{0:HH:MM:ss}  与客户端:{1}的会话被关闭 原因：{2}", DateTime.Now, session.RemoteEndPoint, value);
            Console.WriteLine("当前登录操作用户数：" + ClientManage.loguserlist.Count);
            Console.WriteLine("当前登录管理用户数：" + ClientManage.adminlist.Count);
        }

        void ws_NewMessageReceived(WebSocketSession session, string value)
        {
            ClientMessageParse MessageParse = new ClientMessageParse(session);
            string type;
            string name;
            Console.WriteLine(value);
            if (Program.clientmanage.SessionCheck(session,out type))
            {
                if(type.Equals("Admin"))
                {
                    //已登录管理用户，解析MAN命令
                    Console.WriteLine("管理用户命令");
                    MessageParse.ParseMANMessage(value);
                }
                else if(type.Equals("Control"))
                {
                    //已登录调度台，解析CMD命令
                    Console.WriteLine("操作台用户命令");
                    MessageParse.ParseCMDMessage(value);
                }
                else
                {
                    Console.WriteLine("非法用户类型命令");
                }
                
                //Console.WriteLine("{0:HH:MM:ss}  获取到客户端:{1} 发送的文本消息长度为:{2}", DateTime.Now, session.RemoteEndPoint, value.Length);
            }
            else
            {
                //未登录用户，解析LOG命令
                if (MessageParse.ParseLOGMessage(value,out type,out name))
                {
                    DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
                    sqlcom.InsertUserLog(name, "login");
                    if(type.Equals("Admin"))
                    {
                        Console.WriteLine("新的管理用户登录");
                    }
                    else if(type.Equals("Control"))
                    {
                        List<GroupData> groupdatalist = new List<GroupData>();
                        groupdatalist = MessageParse.GetDeskGroup();
                        //groupdatalist = sqlcmd.GetGroupExt(name, pwd);
                        Console.WriteLine("当前登录用户数：" + ClientManage.loguserlist.Count);
                        //session.Send("LOG#Success");
                        string commandstr = "GroupExt#" + JsonConvert.SerializeObject(groupdatalist);
                        Console.WriteLine(commandstr);
                        session.Send(commandstr);
                    }
                    else
                    {
                        Console.WriteLine("非法类型用户！！！");
                    }
                    
                }
                else
                {
                    //session.Send("LOG#Fail");
                    session.Close();
                }
            }
        }

        void ws_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("{0:HH:MM:ss} 收到来自客户端的二进制流。 长度:{1}", DateTime.Now, value.Length);
            //session.Send(value, 0, value.Length);//将流发送回去

        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            SuperSocket.SocketBase.Config.RootConfig r = new SuperSocket.SocketBase.Config.RootConfig();
            SuperSocket.SocketBase.Config.ServerConfig s = new SuperSocket.SocketBase.Config.ServerConfig();

            s.Name = "SuperWebSocket";
            s.Ip = "Any";
            s.Port = this.port;
            s.Mode = SuperSocket.SocketBase.SocketMode.Tcp;
            //s.ClearIdleSession = true;
            //s.ClearIdleSessionInterval = 50;
            s.ReceiveBufferSize = 50000;
            s.SendBufferSize = 50000;


            //s.Security = "tls";
            //SuperSocket.SocketBase.Config.CertificateConfig cert = new SuperSocket.SocketBase.Config.CertificateConfig();

            //string pathToBaseDir = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            //cert.FilePath = System.IO.Path.Combine(pathToBaseDir, "ServeurWS_TemporaryKey.pfx");
            //System.Diagnostics.Debug.Assert(System.IO.File.Exists(cert.FilePath));
            //cert.Password = "&Syqd4xWq62";

            //s.Certificate = cert;

            SuperSocket.SocketEngine.SocketServerFactory f = new SuperSocket.SocketEngine.SocketServerFactory();

            try
            {
                ws.Setup(r, s, f);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //if (!ws.Setup(this.ip, this.port))
            //{
            //    Console.WriteLine("SimpleWebSocket 设置WebSocket服务侦听地址失败");
            //    return;
            //}

            if (!ws.Start())
            {
                Console.WriteLine("SimpleWebSocket 启动WebSocket服务侦听失败");
                return;
            }

            Console.WriteLine("SimpleWebSocket 启动成功 IP:{0}:{1}",this.ip,this.port);
        }

        /// <summary>
        /// 停止侦听服务
        /// </summary>
        public void Stop()
        {
            if (ws != null)
            {
                ws.Stop();
            }
        }
    }
}
