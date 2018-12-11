using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Npgsql;
using System.IO;

namespace SwitchServer
{

    class HttpServer
    {
        //NpgsqlConnection conn;
        //服务器监听状态
        public bool ServerStatus = false;
        //http监听
        private HttpListener listerner;
        private Thread processor;
        string ip;
        string port;


        List<ExtDevice> ExtList = new List<ExtDevice>();
        List<TrunkDevice> TrunkList = new List<TrunkDevice>();
        public HttpServer(string ip,string port)
        {
            this.ip = ip;
            this.port = port;
        }
        public void StartHttpServer()
        {
            try
            {
                processor = new Thread(new ThreadStart(StartListening));
                processor.Start();
                processor.IsBackground = true;
                //this.ServerStatus = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void StartListening()
        {
            //string httpserveruri = "http://192.168.2.101:80/";
            string httpserveruri = "http://"+this.ip+":"+this.port+"/";
            //string httpserveruri = "http://+:80/";
            try
            {
                //IPAddress ip = IPAddress.Parse(ServerIP);
                //配置监听IP地址和端口
                //tcpListener = new TcpListener(ip, ServerPort);
                listerner = new HttpListener();
                listerner.Prefixes.Add(httpserveruri);
                listerner.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine("http服务启动失败...");
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("http服务器启动成功 {0}",httpserveruri);
            while (true)
            {
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                HttpListenerContext ctx;
                try
                {
                    ctx = listerner.GetContext();
                    Thread t = new Thread(new ParameterizedThreadStart(RevHttpData));
                    t.Start(ctx);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }

            }
        }
        public void RevHttpData(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;
            string ipAddress;
            ipAddress = ctx.Request.RemoteEndPoint.Address.ToString();
            Console.WriteLine("收到"+ipAddress+"的数据:");

            Stream stream = ctx.Request.InputStream;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            String body = reader.ReadToEnd();

            TypeData RecvData;
            RecvData.type = "EVENT&CDR";
            RecvData.data = body;
            RecvData.clientsession = null;

            Console.WriteLine(body);
            ctx.Response.Close();
            IpTypeData iptypedata;
            iptypedata.ip = ipAddress;
            iptypedata.typedata = RecvData;
            Thread t = new Thread(new ParameterizedThreadStart(ReportMessage));
            t.Start(iptypedata);
            //对收到的数据进行对应软交换协议的解析,通过IP地址查找
            //SwitchDev switcher = SwitchManage.switchlist.Find(c => c.ip.Equals(ipAddress));
            //switcher.MessageParse(RecvData);
            //ReportMessage reportmessage = new ReportMessage ();
            //reportmessage.extid.AddRange(switcher.extidlist);
            
            //reportmessage.message = switcher.reportstr;
            //Program.clientmanage.ReportState(reportmessage);
            
            
        }
        public void ReportMessage(object o)
        {
            IpTypeData data = (IpTypeData)o;
            SwitchDev switcher = SwitchManage.switchlist.Find(c => c.ip.Equals(data.ip));
            if(switcher==null)
            {
                Console.WriteLine("软交换不存在！！");
            }
            else
            {
                switcher.MessageParse(data.typedata);
            }            
        }
        
    }

}
