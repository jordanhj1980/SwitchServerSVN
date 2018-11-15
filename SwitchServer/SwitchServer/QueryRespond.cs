using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
//using WindowsFormsApplication1;
namespace SwitchServer
{
    class EventMessage
    {

        public callsession callsessiondata;
        //public string messagetype;
        public CallData calldata;
        public CDR cdr;
        public string reportstr = "";
        public string extid = "";
        public List<string> extlist= new List<string> ();
        public string state = "NULL";
        /// <summary>
        /// 事件EVENT和通话记录CDR消息解析
        /// </summary>
        public string ParseEventCdr(string o)
        {
            string recvdata = o;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(recvdata);
            XmlNode root = doc.DocumentElement;
            string MessageType = root.Name;
            switch (MessageType)
            {
                case "Event":
                    return ParseEvent(recvdata);
                case "Cdr":
                    return ParseCdr(recvdata);
                default:
                    Console.WriteLine(MessageType);
                    Console.WriteLine(recvdata);
                    return "未解析指令";
            }

        }
        /// <summary>
        /// 事件EVENT消息解析
        /// </summary>
        private string ParseEvent(string o)
        {
            string recvdata = o;
            string printstr;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(recvdata);
            XmlNode root = doc.DocumentElement;
            string EventAttr = ((XmlElement)root).GetAttribute("attribute");
            switch (EventAttr)
            {
                //系统事件----------------------------------------------------------------
                case "BOOTUP":
                    this.state = "BOOTUP";
                    printstr = "OM设备重启完成！";
                    break;
                case "CONFIG_CHANGE":
                    this.state = "CONFIG_CHANGE";
                    printstr = "OM设备参数发生变化！";
                    break;
                //------------------------------------------------------------------------
                //分机状态变更事件--------------------------------------------------------
                case "BUSY":
                    this.state = "BUSY";
                    printstr = "收到分机忙事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机忙";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");
                    //生成向客户端上报的字段
                    reportstr = "STATE#BUSY#" + this.extid;
                    break;
                case "IDLE":
                    this.state = "IDLE";
                    printstr = "收到分机空闲事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机空闲";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");
                    reportstr = "STATE#IDLE#" + this.extid;
                    break;
                case "ONLINE":
                    this.state = "ONLINE";
                    printstr = "收到分机上线时间：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机上线";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");
                    reportstr = "STATE#ONLINE#" + this.extid;
                    break;
                case "OFFLINE":
                    this.state = "OFFLINE";
                    printstr = "收到分机离线事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机离线";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");
                    reportstr = "STATE#OFFLINE#" + this.extid;
                    break;
                //------------------------------------------------------------------------
                //呼叫状态变更事件--------------------------------------------------------
                case "RING":
                    this.state = "RING";
                    printstr = "收到振铃事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机振铃";
                    this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");//第一项为被呼方

                    switch(((XmlElement)root.LastChild).Name)//第二项为呼叫方
                    {
                        case "visitor"://来电转分机，分机振铃:
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("from");
                            break;
                        case "ext"://分机呼分机，分机振铃:
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("id");
                            break;
                        case "outer"://通过 API 实现分机外呼，且呼叫方式为 “先呼被叫，被叫回铃再呼主叫”时，主叫分机振铃:
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("to");
                            //this.calldata.toid = ((XmlElement)root.FirstChild).GetAttribute("to");
                            break;
                        case "menu"://menu呼分机，分机振铃
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("id");
                            break;
                        default:
                            Console.WriteLine("RING 存在未解析字段");
                            break;
                    }
                    reportstr = "STATE#RING#" + JsonConvert.SerializeObject(this.calldata);
                    break;
                case "ALERT":
                    this.state = "ALERT";
                    printstr = "收到回铃事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机回铃";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");

                    switch (((XmlElement)root.FirstChild).Name)//解析第一项
                    {
                        case "visitor":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("from");
                            break;
                        case "ext":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");
                            break;
                        default:
                            Console.WriteLine("ALERT 第一项存在未解析字段");
                            break;
                    }

                    switch(((XmlElement)root.LastChild).Name)
                    {
                        case "ext":
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("id");
                            break;
                        case "outer":
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("to");
                            break;
                        default:
                            Console.WriteLine("ALERT 第二项存在未解析字段");
                            break;

                    }
                    reportstr = "STATE#ALERT#" + JsonConvert.SerializeObject(this.calldata);
                    break;
                case "ANSWER":
                    this.state = "ANSWER";
                    printstr = "收到呼叫应答事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机摘机";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");

                    this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");
                    switch(((XmlElement)root.LastChild).Name)
                    {
                        case "ext":
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("id");
                            break;
                        case "visitor":
                            this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("from");
                            break;
                        default:
                            Console.WriteLine("ANSWER 存在未解析字段");
                            break;
                    }

                    reportstr = "STATE#ANSWER#" + JsonConvert.SerializeObject(this.calldata);
                    break;
                case "ANSWERED":
                    this.state = "ANSWERED";
                    printstr = "收到被应答事件：" + ((XmlElement)root.LastChild).GetAttribute("id") + "号分机" +
                                "检测到" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机摘机";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");

                    switch(((XmlElement)root.FirstChild).Name)
                    {
                        case "outer":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("to");
                            break;
                        case "ext":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");
                            break;
                        case "visitor":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("from");
                            break;
                        default:
                            Console.WriteLine("ANSWERED 存在未解析字段");
                            break;
                    }

                    this.calldata.toid = ((XmlElement)root.LastChild).GetAttribute("id");

                    reportstr = "STATE#ANSWERED#" + JsonConvert.SerializeObject(this.calldata);
                    break;
                case "BYE":
                    this.state = "BYE";
                    printstr = "收到通话结束事件：" + ((XmlElement)root.FirstChild).GetAttribute("id") + "号分机挂机";
                    this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");

                    switch(((XmlElement)root.FirstChild).Name)
                    {
                        case "visitor":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("from");
                            this.calldata.toid = ((XmlElement)root.FirstChild).GetAttribute("to");
                            break;
                        case "outer":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("to");
                            break;
                        case "ext":
                            this.calldata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");                      
                            break;
                        default:
                            Console.WriteLine("BYE 存在未解析字段");
                            break;
                    }
                    if ((XmlElement)root.FirstChild.NextSibling!=null)
                    {
                        switch (((XmlElement)root.FirstChild.NextSibling).Name)
                        {
                            case "ext":
                                this.calldata.toid = ((XmlElement)root.FirstChild.NextSibling).GetAttribute("id");
                                break;
                            case "outer":
                                this.calldata.toid = ((XmlElement)root.FirstChild.NextSibling).GetAttribute("to");
                                break;
                            case "visitor":
                                this.calldata.toid = ((XmlElement)root.FirstChild.NextSibling).GetAttribute("from");
                                break;
                            default:
                                Console.WriteLine("BYE 存在未解析字段");
                                break;
                        }
                    }
                    
                    reportstr = "STATE#BYE#" + JsonConvert.SerializeObject(this.calldata);
                    break;
                case "DIVERT":
                    this.state = "DIVERT";
                    printstr = "收到呼叫转移事件：";
                    break;
                case "TRANSIENT":
                    this.state = "TRANSIENT";
                    printstr = "收到呼叫临时事件：";
                    break;
                case "FAILED":
                    this.state = "FAILED";
                    try
                    {
                        printstr = "收到呼叫失败事件：" + "失败原因是" + ((XmlElement)root.LastChild).GetAttribute("code") + ":" +
                                                 ((XmlElement)root.LastChild).GetAttribute("reason");

                        this.extid = ((XmlElement)root.FirstChild).GetAttribute("id");
                        FailCalldata faildata;
                        faildata.fromid = "";//呼叫方
                        faildata.toid = "";//被呼方
                        faildata.reason = "";//呼叫失败原因
                        faildata.reason = ((XmlElement)root.LastChild).GetAttribute("code");
                        faildata.fromid = ((XmlElement)root.FirstChild).GetAttribute("id");

                        if ((XmlElement)root.FirstChild.NextSibling != null)
                        {
                            switch (((XmlElement)root.FirstChild.NextSibling).Name)
                            {
                                case "ext":
                                    faildata.toid = ((XmlElement)root.FirstChild.NextSibling).GetAttribute("id");
                                    break;
                                case "code":
                                    faildata.toid = faildata.fromid;
                                    break;
                                default:
                                    Console.WriteLine("FAILED 存在未解析字段");
                                    break;
                            }
                        }

                        reportstr = "STATE#FAIL#" + JsonConvert.SerializeObject(faildata);
                        Console.Write(root.InnerXml);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("FAIL wrong:"+ex.Message);
                        Console.WriteLine(root.InnerXml);
                        printstr = "FAIL wrong:" + ex.Message;
                    }
                    
                    break;
                //------------------------------------------------------------------------
                //来电呼入控制流程事件----------------------------------------------------
                case "INVITE":
                    this.state = "INVITE";
                    printstr = "收到来电呼叫请求事件";
                    EventInvite(recvdata);
                    break;
                case "INCOMING":
                    this.state = "INCOMING";
                    printstr = "收到来电呼入事件";
                    //EventInvite(recvdata);
                    break;
                //------------------------------------------------------------------------
                //按键信息事件------------------------------------------------------------
                case "DTMF":
                    this.state = "DTMF";
                    printstr = "收到按键信息事件";
                    break;
                //------------------------------------------------------------------------
                //语音文件播放完毕事件事件------------------------------------------------
                case "EndOfAnn":
                    this.state = "EndOfAnn";
                    printstr = "收到语音文件播放完毕事件";
                    break;
                //------------------------------------------------------------------------
                //分机组队列事件----------------------------------------------------------
                case "QUEUE":
                    this.state = "QUEUE";
                    printstr = "收到分机组队列事件";
                    break;
                //------------------------------------------------------------------------

                default:
                    printstr = "收到未处理事件";
                    break;
            }
            extlist.Add(this.extid);
            extlist.Add(calldata.fromid);
            extlist.Add(calldata.toid);
            return printstr;

        }
        /// <summary>
        /// INVITE事件解析
        /// </summary>
        private void EventInvite(string o)
        {
            string recvdata = o;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(recvdata);
            XmlNode root = doc.DocumentElement;
            XmlNodeList nodelist = root.ChildNodes;
            foreach (XmlElement element in nodelist)
            {
                switch (element.Name)
                {
                    case "trunk":
                        this.callsessiondata.trunkid = element.GetAttribute("id");
                        break;
                    case "visitor":
                        this.callsessiondata.visitorid = element.GetAttribute("id");
                        this.callsessiondata.fromnumber = element.GetAttribute("from");
                        this.callsessiondata.tonumber = element.GetAttribute("to");
                        this.callsessiondata.callid = element.GetAttribute("callid");
                        break;
                    default:
                        break;
                }
            }
            ReportMessage message = new ReportMessage ();
            message.extid .Add("220");
            message.message = "STATE#INVITE#" + JsonConvert.SerializeObject(this.callsessiondata);
            Program.clientmanage.ReportState(message);
            Console.WriteLine(message.message);
        }
        /// <summary>
        /// 通话记录CDR消息解析
        /// </summary>
        private string ParseCdr(string o)
        {
            string revdata = (string)o;
            string printstr = "";
            string cdrtype = "";
            string typestr = "";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(revdata);
            XmlNode root = doc.DocumentElement;
            string id = ((XmlElement)root).GetAttribute("id");
            this.cdr.Cdrid = id;
            XmlNodeList attrnode = root.ChildNodes;
            foreach (XmlNode attr in attrnode)
            {
                if (attr.Name.Equals("Type"))
                {
                    this.cdr.type = attr.InnerText;
                    cdrtype = attr.InnerText;
                }
                else if(attr.Name.Equals("callid"))
                {
                    this.cdr.callid = attr.InnerText;
                }
                else if(attr.Name.Equals("TimeStart"))
                {
                    this.cdr.TimeStart = attr.InnerText;
                }
                else if(attr.Name.Equals("TimeEnd"))
                {
                    this.cdr.TimeEnd = attr.InnerText;
                }
                else if(attr.Name.Equals("CPN"))
                {
                    this.cdr.CPN = attr.InnerText;
                }
                else if(attr.Name.Equals("CDPN"))
                {
                    this.cdr.CDPN = attr.InnerText;
                }
                else if(attr.Name.Equals("Duration"))
                {
                    this.cdr.Duration = attr.InnerText;
                }
                else if(attr.Name.Equals("visitor"))
                {

                }
                else if(attr.Name.Equals("Route"))
                {

                }
                else if(attr.Name.Equals("TrunkNumber"))
                {

                }
                else if(attr.Name.Equals("Recording"))
                {

                }
                else if(attr.Name.Equals("RecCodec"))
                {

                }
                else
                {
                    Console.WriteLine("CDR存在未解析字段：" + attr.Name);
                }

            }
            switch (cdrtype)
            {
                case "IN":
                    typestr = "呼入话单";
                    break;
                case "OU":
                    typestr = "呼出话单";
                    break;
                case "LO":
                    typestr = "内部互拨话单";
                    break;
                case "FI":
                case "FW":
                    typestr = "呼叫转移话单";
                    break;
                case "CB":
                    typestr = "双向外呼话单";
                    break;
                default:
                    typestr = "未知";
                    break;
            }
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            sqlcom.InsertCDR(this.cdr);
            printstr = "CDR:" + id + "\tTYPE:" + typestr + "\n";
            return printstr;
        }

    }
    class QueryDeviceRespond
    {

        public string Manufacturer;
        public string Model;
        public string Version;
        public string Mac;

        //public List<Device> DeviceList = new List<Device>();
        //public List<List<string>> Group;
        public List<ExtDevice> ExtList = new List<ExtDevice>();
        public List<TrunkDevice> TrunkList = new List<TrunkDevice>();
        public void GetDevice(XmlNode devices)
        {
            XmlNodeList devicelist = devices.ChildNodes;
            XmlElement temp;
            ExtDevice extelement = new ExtDevice();
            //TrunkDevice trunkelement = new TrunkDevice();
            foreach(XmlNode deviceelement in devicelist)
            {
                switch(deviceelement.Name)
                {
                    case "ext":
                        temp = (XmlElement)deviceelement;
                        extelement = new ExtDevice();
                        extelement.lineid = temp.GetAttribute("lineid");
                        extelement.extid = temp.GetAttribute("id");
                        extelement.type = "ext";
                        ExtList.Add(extelement);
                        
                        break;
                    case "line":
                        temp = (XmlElement)deviceelement;
                        //trunkelement.lineid = temp.GetAttribute("lineid");
                        //trunkelement.trunkid = temp.GetAttribute("id");
                        //TrunkList.Add(trunkelement);
                        extelement = new ExtDevice();
                        extelement.lineid = temp.GetAttribute("lineid");
                        extelement.extid = temp.GetAttribute("id");
                        extelement.type = "trunk";
                        ExtList.Add(extelement);
                        break;
                    //case "group":
                    //    break;
                    default:
                        Console.WriteLine("文件包含未解析项目！！！");
                        break;
                }

            }

        }
        public string ParseQueryRespond(string o)
        {
            string revdata = (string)o;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(revdata);
            XmlNode root = doc.DocumentElement;
            //XmlNode root1 = root.FirstChild;
            if(!root.Name.Equals("DeviceInfo"))
            {
                Console.WriteLine("解析错误，非查询应答！！！");
                return "解析错误，非查询应答！！！";
            }
            XmlNodeList lv1nodes = root.ChildNodes;
            foreach (XmlNode lv1element in lv1nodes)
            {
                switch(lv1element.Name)
                {
                    case "manufacturer":
                        Manufacturer = lv1element.InnerText;
                        break;
                    case "model":
                        Model = lv1element.InnerText;
                        break;
                    case "version":
                        Version = lv1element.InnerText;
                        break;
                    case "mac":
                        Mac = lv1element.InnerText;
                        break;
                    case "devices":
                        GetDevice(lv1element);
                        break;
                    default:
                        Console.WriteLine("文件包含未解析项目！！！");
                        return "文件包含未解析项目！！！";

                }
            }
            Console.WriteLine("完成设备查询应答解析");
            return "完成设备查询应答解析";
        }
    }

    class QueryExtTrunkRespond
    {
        public string reportstr = "";
        private string jasonstr = "";
        CallData calldata;
        public ExtDevice ext = new ExtDevice();
        public string ParseExtRespond(string o)
        {
            this.ext.type = "ext";
            string revdata =o;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(revdata);
            }
            //catch (System.Xml.XmlException)
            catch(System.Xml.XmlException e)
            {
                Console.WriteLine("错误简要信息："+e.Message);
                return "查无结果！！！";
            }
            
            XmlNode root = doc.DocumentElement;
            if (!root.Name.Equals("Status"))
            {
                Console.WriteLine("解析错误，非查询分机应答！！！");
                return "解析错误，非查询分机应答！！！" + root.Name;
            }
            XmlNode lv1nodes = root.FirstChild;
            if(!lv1nodes.Name.Equals("ext"))
            {
                Console.WriteLine("格式错误，不是ext开头！！");
                return "格式错误，不是ext开头！！" + lv1nodes.Name;
            }
            ext.extid = ((XmlElement)lv1nodes).GetAttribute("id");
            XmlNodeList lv2nodes = lv1nodes.ChildNodes;

            string extattr = "分机号：" + ext.extid + "\n";
            calldata.fromid = ext.extid;
            jasonstr = ext.extid;
            foreach (XmlElement lv2element in lv1nodes)
            {
                switch (lv2element.Name)
                {
                    case "lineid":
                        ext.lineid = lv2element.InnerText;
                        extattr += "分机线路编号: " + ext.lineid + "\n";
                        break;
                    case "group":
                        //this.groupid.Add(((XmlElement)lv2element).GetAttribute("id"));
                        extattr += "分机组号: \n";
                        break;
                    case "staffid":
                        ext.staffid = lv2element.InnerText;
                        extattr += "工号：" + ext.staffid + "\n";
                        break;
                    case "Call_Pickup":
                        ext.Call_pickup = lv2element.InnerText;
                        extattr += "代接权限：" + ext.Call_pickup + "\n";
                        break;
                    case "Fwd_Type":
                        ext.Fwd_Type = lv2element.InnerText;
                        extattr += "呼叫转移方式：" + ext.Fwd_Type + "\n";
                        break;
                    case "Fwd_Number":
                        ext.Fwd_Number = lv2element.InnerText;
                        extattr += "呼叫转移号码：" + ext.Fwd_Number + "\n";
                        break;
                    case "Call_Restriction":
                        ext.Call_Restriction = lv2element.InnerText;
                        extattr += "呼叫权限：" + ext.Call_Restriction + "\n";
                        break;
                    case "Off_Line_Num":
                        //ext.Off_Line_Num = lv2element.InnerText;
                        extattr += "Off_line_Num：\n";
                        break;
                    case "mobile":
                        ext.mobile = lv2element.InnerText;
                        extattr += "分机绑定的手机号：" + ext.mobile + "\n";
                        break;
                    case "fork":
                        ext.fork = lv2element.InnerText;
                        extattr += "同震号码：" + ext.fork + "\n";
                        break;
                    case "email":
                        //ext.email = lv2element.InnerText;
                        extattr += "员工电子邮件：\n";
                        break;
                    case "record":
                        ext.record = lv2element.InnerText;
                        extattr += "实时录音功能开关：" + ext.record + "\n";
                        break;
                    case "api":
                        ext.api = lv2element.InnerText;
                        extattr += "API功能开关：" + ext.api + "\n";
                        break;
                    case "voicefile":
                        //ext.voicefile = lv2element.InnerText;
                        extattr += "语音文件：\n";
                        break;
                    case "state":
                        ext.state = lv2element.InnerText;
                        extattr += "线路状态：" + ext.state + "\n";
                        break;
                    case "AutoAnswer":
                        ext.AutoAnswer = lv2element.InnerText;
                        extattr += "自动应答：" + ext.AutoAnswer + "\n";
                        break;
                    case "outer":
                        //this.outer.id = ((XmlElement)lv2element).GetAttribute("id");
                        //this.outer.from = ((XmlElement)lv2element).GetAttribute("from");
                        //this.outer.to = ((XmlElement)lv2element).GetAttribute("to");
                        //this.outer.trunk = ((XmlElement)lv2element).GetAttribute("trunk");
                        //this.outer.callid = ((XmlElement)lv2element).GetAttribute("callid");
                        //this.outer.state = lv2element.FirstChild.InnerText;
                        extattr += "outer：\n";

                        calldata.toid = lv2element.GetAttribute("to");
                        break;
                    case "visitor":
                        extattr += "visitor：\n";
                        calldata.toid = lv2element.GetAttribute("from");
                        break;
                    case "ext":
                        extattr += "ext：\n";
                        calldata.toid = lv2element.GetAttribute("id");
                        break;
                    default:
                        Console.WriteLine("文件包含未解析项目！！！" + lv2element.Name);
                        return "文件包含未解析项目！！！" + lv2element.Name;
                       // break;

                }
            }
            Console.WriteLine("完成分机查询应答解析");

            switch (ext.state)//将软交换上报的状态分类映射，减少向客户端上报状态的种类
            {
                case "ready":
                    ext.state = "IDLE";
                    jasonstr = ext.extid;
                    break;
                case "active":
                    ext.state = "ANSWER";
                    jasonstr = JsonConvert.SerializeObject(calldata);
                    break;
                case "progress":
                    ext.state = "RING";
                    jasonstr = JsonConvert.SerializeObject(calldata);
                    break;
                case "offline":
                    ext.state = "OFFLINE";
                    jasonstr =ext.extid;
                    break;
                case "offhook":
                    ext.state = "OFFHOOK";
                    jasonstr = ext.extid;
                    break;
                default:
                    Console.WriteLine("非解析状态！！");
                    break;
            }
            reportstr = "STATE#" + ext.state + "#" + jasonstr;
            return "返回" + ext.lineid + "号分机属性!" + "\n";// +extattr;
        }
        public string ParseTrunkRespond(string o)
        {
            this.ext.type = "trunk";
            string revdata = o;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(revdata);
            }
            //catch (System.Xml.XmlException)
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine("错误简要信息：" + e.Message);
                return "查无结果！！！";
            }

            XmlNode root = doc.DocumentElement;
            if (!root.Name.Equals("Status"))
            {
                Console.WriteLine("解析错误，非查询中继应答！！！");
                return "解析错误，非查询中继应答！！！" + root.Name;
            }
            XmlNode lv1nodes = root.FirstChild;
            if (!lv1nodes.Name.Equals("trunk"))
            {
                Console.WriteLine("格式错误，不是trunk开头！！");
                return "格式错误，不是trunk开头！！" + lv1nodes.Name;
            }
            ext.extid = ((XmlElement)lv1nodes).GetAttribute("id");
            XmlNodeList lv2nodes = lv1nodes.ChildNodes;

            string extattr = "中继号：" + ext.extid + "\n";
            calldata.fromid = ext.extid;
            jasonstr = ext.extid;
            foreach (XmlElement lv2element in lv1nodes)
            {
                switch (lv2element.Name)
                {
                    case "lineid":
                        ext.lineid = lv2element.InnerText;
                        extattr += "分机线路编号: " + ext.lineid + "\n";
                        break;                   
                    case "state":
                        ext.state = lv2element.InnerText;
                        extattr += "线路状态：" + ext.state + "\n";
                        break;                    
                    case "visitor":
                        extattr += "visitor：\n";
                        calldata.toid = lv2element.GetAttribute("from");
                        break;

                    default:
                        Console.WriteLine("文件包含未解析项目！！！" + lv2element.Name);
                        return "文件包含未解析项目！！！" + lv2element.Name;
                    // break;

                }
            }
            Console.WriteLine("完成分机查询应答解析");

            switch (ext.state)//将软交换上报的状态分类映射，减少向客户端上报状态的种类
            {
                case "ready":
                    ext.state = "IDLE";
                    jasonstr = ext.extid;
                    break;
                case "active":
                    ext.state = "ANSWER";
                    jasonstr = ext.extid;;
                    break;
                case "unwired":
                    ext.state = "OFFLINE";
                    jasonstr = ext.extid;;
                    break;
                case "offline":
                    ext.state = "OFFLINE";
                    jasonstr = ext.extid;
                    break;

                default:
                    Console.WriteLine("非解析状态！！");
                    break;
            }
            reportstr = "STATE#" + ext.state + "#" + jasonstr;
            return "返回" + ext.lineid + "号中继属性!" + "\n" + extattr;
        }
    }
}
