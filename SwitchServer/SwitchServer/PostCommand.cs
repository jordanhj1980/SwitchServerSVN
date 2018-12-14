using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SwitchServer
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class Command
    {
        public XmlDocument command = new XmlDocument();
        public XmlDeclaration dec;
        public XmlElement commandhead;
        public List<XmlElement> commandbody = new List<XmlElement>();
        public string XmlCommandString;
        /// <summary>
        /// 基类构造函数，添加XML头信息
        /// </summary>
        public Command()
        {
            dec = command.CreateXmlDeclaration("1.0", "utf-8", null);
            command.AppendChild(dec);
        }
        /// <summary>
        /// 生成完整XML信息
        /// </summary>
        public virtual void BuildXML()
        {
            if(commandbody!=null)
            {
                foreach(XmlElement bodyelement in commandbody)
                {
                    commandhead.AppendChild(bodyelement);
                }
                command.AppendChild(commandhead);
                XmlCommandString = command.InnerXml;
            }
            else
            {
                XmlCommandString = "";
            }
        }
    }
    /// <summary>
    /// 查询命令基类
    /// </summary>
    public class QueryCommand : Command
    {

        public QueryCommand()
        {
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Query");
        }
    }
    /// <summary>
    /// 查询设备信息
    /// </summary>
    public class QueryDeviceInfo : QueryCommand
    {
        public QueryDeviceInfo()
        {
            commandbody.Add(command.CreateElement("DeviceInfo"));
            BuildXML();
        }
    }
    /// <summary>
    /// 查询指定分机
    /// </summary>
    public class QueryExt : QueryCommand
    {
        public string extid;
        private XmlElement bodyelement;
        public QueryExt(string ext)
        {
            this.extid = ext;
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 查询指定中继
    /// </summary>
    public class QueryTrunk : QueryCommand
    {
        public string trunkid;
        private XmlElement bodyelement;
        public QueryTrunk(string trunk)
        {
            this.trunkid = trunk;
            bodyelement = command.CreateElement("trunk");
            bodyelement.SetAttribute("id", trunkid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 查询来电信息
    /// </summary>
    public class QueryVisitor : QueryCommand
    {
        public string visitorid;
        private XmlElement bodyelement;
        public QueryVisitor(string visitor)
        {
            this.visitorid = visitor;
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 查询去电信息
    /// </summary>
    public class QueryOuter : QueryCommand
    {
        public string outerid;
        private XmlElement bodyelement;
        public QueryOuter(string outer)
        {
            this.outerid = outer;
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("id", outerid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 查询分机组
    /// </summary>
    public class QueryGroup : QueryCommand
    {
        public string groupid;
        private XmlElement bodyelement;
        public QueryGroup(string group)
        {
            this.groupid = group;
            bodyelement = command.CreateElement("group");
            bodyelement.SetAttribute("id", groupid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 查询语音菜单
    /// </summary>
    public class QueryMenu : QueryCommand
    {
        public string menuid;
        private XmlElement bodyelement;
        public QueryMenu(string menu)
        {
            this.menuid = menu;
            bodyelement = command.CreateElement("menu");
            bodyelement.SetAttribute("id", menuid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 呼叫保持命令
    /// </summary>
    public class HoldCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public HoldCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Hold");
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 呼叫接回命令
    /// </summary>
    public class UnholdCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public UnholdCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Unhold");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 开启静音
    /// </summary>
    public class MuteCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public MuteCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Mute");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 解除静音
    /// </summary>
    public class UnmuteCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public UnmuteCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Unmute");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 监听
    /// </summary>
    public class MonitorCommand : Command
    {
        string fromid; //监听方
        string toid;    //被监听方
        private XmlElement bodyelement;
        public MonitorCommand(string fromid,string toid)
        {
            this.fromid = fromid;
            this.toid = toid;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Monitor");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", this.fromid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", this.toid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 从监听到插播
    /// </summary>
    public class TalkCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public TalkCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Talk");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 从插播到监听
    /// </summary>
    public class ListenCommand : Command
    {
        string extid;
        private XmlElement bodyelement;
        public ListenCommand(string id)
        {
            this.extid = id;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Listen");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    //强插
    //强插方必须开启“强插”开关；
    //被强插方和被强插方的通话方（若为分机）必须关闭“禁止被强插”开关；
    //如果分机在进行三方会议时，则不能被强插；
    //强插范围：只限于同一台OM设备上的分机相互强插，暂不支持组网的OM设备之间相互强插；
    //强插方分机挂机，不影响其他两方通话，而被强插方分机挂机，会同时释放其他两方通话；
    //强插方退出三方通话后，可再次执行强插进入三方通话；而对于被强插方及其通话方，退出后不可再次强插进入通话。
    /// <summary>
    /// 强插
    /// </summary>
    public class BargeinCommand : Command
    {
        string fromid; //强插方
        string toid;    //被强插方
        private XmlElement bodyelement;
        public BargeinCommand(string fromid, string toid)
        {
            this.fromid = fromid;
            this.toid = toid;
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Bargein");

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", this.fromid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", this.toid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    //强拆（部分实现）
    //此类API用于强制挂断OM上的指定通话。该API可强拆的对象包括：分机(ext)、来电(visitor)、去电(outer)。
    //目前只实现强拆分机
    /// <summary>
    /// 强拆
    /// </summary>
    public class ClearCommand : Command
    {
        
        private XmlElement bodyelement;
        public ClearCommand(CallData data)
        {
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Clear");
            if((data.visitorid!="")&&(data.visitorid!=null))
            {
                bodyelement = command.CreateElement("visitor");
                bodyelement.SetAttribute("id", data.visitorid);
                commandbody.Add(bodyelement);
                BuildXML();
                return;
            }
            if((data.outerid!="")&&(data.outerid!=null))
            {
                bodyelement = command.CreateElement("outer");
                bodyelement.SetAttribute("id", data.outerid);
                commandbody.Add(bodyelement);
                BuildXML();
                return;
            }
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", data.toid);
            commandbody.Add(bodyelement);
            BuildXML();
            
        }
    }
    /// <summary>
    /// 连接命令基类
    /// </summary>
    public class ConnectCommand : Command
    {
        public ConnectCommand()
        {
            commandhead = command.CreateElement("Transfer");
            commandhead.SetAttribute("attribute", "Connect");
        }
        
    }
    /// <summary>
    /// 分机呼叫分机
    /// </summary>
    //该API用于将一个分机呼叫另一个分机，从而使两者建立通话。
    public class ExtToExt : ConnectCommand
    {
        private XmlElement bodyelement;
        string fromid;
        string toid;
        public ExtToExt(string fromid, string toid)
        {
            this.fromid = fromid;
            this.toid = toid;
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", fromid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", toid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 分机呼外部电话(部分实现)
    /// </summary>
    //该API用于让分机向外部电话发起呼叫，从而使二者建立通话。
    public class ExtToOuter : ConnectCommand
    {
        private XmlElement bodyelement;
        string extid;
        string outerid;
        public ExtToOuter(string extid,string trunkid, string outerid)
        {
            this.extid = extid;
            this.outerid = outerid;
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            if((trunkid==null)||(trunkid==""))
            {

            }
            else
            {
                bodyelement = command.CreateElement("trunk");
                bodyelement.SetAttribute("id", trunkid);
                commandbody.Add(bodyelement);
            }
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", outerid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 来电转分机（部分实现）
    /// </summary>
    //该API用于将来电转接给分机，从而使来电和分机建立通话。
    public class VisitorToExt : ConnectCommand
    {
        private XmlElement bodyelement;
        string visitorid;
        string extid;
        public VisitorToExt(string visitorid, string extid)
        {
            this.visitorid = visitorid;
            this.extid = extid;
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 来电转外部电话
    /// </summary>
    //该API用于让来电向外部电话发起呼叫，从而实现两个外部电话能够以OM为中转站建立通话。
    //执行外呼之前，需开启外线的外转外权限。
    public class VisitorToOuter : ConnectCommand
    {
        private XmlElement bodyelement;
        string visitorid;
        string outerid;
        public VisitorToOuter(string visitorid, string outerid)
        {
            this.visitorid = visitorid;
            this.outerid = outerid;
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", outerid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 来电转语音菜单
    /// </summary>
    //该API用于将来电转接到语音菜单（menu），从而实现：
    //向通话方播放语音文件。其中，语音文件的播放次数和内容都可以控制。
    //能够检查通话方输入的按键信息，且按键满足menu配置的按键汇报条件后，OM会将按键信息汇报给应用服务器。
    public class VisitorToMenu : ConnectCommand
    {
        private XmlElement bodyelement;
        string visitorid;
        string menuid;
        public VisitorToMenu(string visitorid, string menuid)
        {
            this.visitorid = visitorid;
            this.menuid = menuid;
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("id", menuid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 去电转分机
    /// </summary>
    //该API用于将去电（已从OM呼出的通话）转接给分机，从而使两者能够建立通话。
    public class OuterToExt : ConnectCommand
    {
        private XmlElement bodyelement;
        string outerid;
        string extid;
        public OuterToExt(string outerid, string extid)
        {
            this.outerid = outerid;
            this.extid = extid;
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("id", outerid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 去电转外部电话
    /// </summary>
    //该API用于让去电（已从OM呼出的通话）向外部电话发起呼叫，从而实现两个外部电话能够以OM为中转站建立通话。
    public class OuterToOuter : ConnectCommand
    {
        private XmlElement bodyelement;
        string fromid;
        string toid;
        public OuterToOuter(string fromid, string toid)
        {
            this.fromid = fromid;
            this.toid = toid;
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("id", fromid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", toid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 去电转语音菜单
    /// </summary>
    //该API用于将去电转接到语音菜单（menu），从而实现：
    //向通话方播放语音文件。其中，语音文件的播放次数和内容都可以控制。
    //能够检查通话方输入的按键信息，且按键满足menu配置的按键汇报条件后，OM会将按键信息汇报给应用服务器。
    public class OuterToMenu : ConnectCommand
    {
        private XmlElement bodyelement;
        string outerid;
        string menuid;
        public OuterToMenu(string outerid, string menuid)
        {
            this.outerid = outerid;
            this.menuid = menuid;
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("id", outerid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("menu");
            bodyelement.SetAttribute("id", menuid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 语音菜单呼分机
    /// </summary>
    //该API用于语音菜单（menu）呼分机，从而实现：
    //向分机播放语音文件。其中，语音文件的播放次数和内容都可以控制。
    //能够检查分机输入的按键信息，且按键满足menu配置的按键汇报条件后，OM会将按键信息汇报给应用服务器。
    public class MenuToExt : ConnectCommand
    {
        private XmlElement bodyelement;
        string menuid;
        string extid;
        public MenuToExt(string menuid, string extid)
        {
            this.menuid = menuid;
            this.extid = extid;
            bodyelement = command.CreateElement("menu");
            bodyelement.SetAttribute("id", menuid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);

            //bodyelement = command.CreateElement("AutoAnswer");
            //bodyelement.InnerText = "yes";
            //commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 语音菜单呼外部电话
    /// </summary>
    //该API用于语音菜单（menu）向外部电话发起呼叫，从而实现：
    //当外部电话接通后，向其播放语音文件。其中，语音文件的播放次数和内容都可以控制。
    //能够检查外部电话输入的按键信息，且按键满足menu配置的按键汇报条件后，OM会将按键信息汇报给应用服务器。
    public class MenuToOuter : ConnectCommand
    {
        private XmlElement bodyelement;
        string menuid;
        string outerid;
        public MenuToOuter(string menuid, string outerid)
        {
            this.menuid = menuid;
            this.outerid = outerid;
            bodyelement = command.CreateElement("menu");
            bodyelement.SetAttribute("id", menuid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", outerid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 双向外呼（回拨）
    /// </summary>
    //该API用于让OM依次向外发起两路呼叫，并以OM作为中间点建立连接，从而实现两个外部电话之间建立通话。
    //呼叫顺序
    //先呼主叫，主叫摘机后听到回铃音，然后再呼被叫，被叫摘机后双方建立通话。
    //中继资源要求
    //至少要有两条空闲可用的中继资源；
    //第一路外呼默认使用IP中继，可通过参数CB_TYPE配置调整为模拟中继；
    //第二路外呼按照OM的外呼规则执行。
    //执行权限
    //执行双向外呼之前，需要开启线路的外转外权限。
    public class OuterToOuter2 : ConnectCommand
    {
        private XmlElement bodyelement;
        string fromid;
        string toid;
        public OuterToOuter2(string fromid, string toid)
        {
            this.fromid = fromid;
            this.toid = toid;
            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", fromid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("outer");
            bodyelement.SetAttribute("to", toid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 语音插播
    /// </summary>
    //该API用于向正在通话的一方插播语音。执行插播命令后，被插播方听语音，其通话方静音，语音播放完毕后自动恢复原有通话。
    //如果需要双向插播，可同时执行两次命令。
    public class ExtToVoice : ConnectCommand
    {
        private XmlElement bodyelement;
        string extid;
        string voicefile;
        public ExtToVoice(string extid, string voicefile)
        {
            this.extid = extid;
            this.voicefile = voicefile;
            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("voicefile");
            bodyelement.InnerText = voicefile;
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 队列命令基类
    /// </summary>
    public class QueueCommand :ConnectCommand
    {
        public QueueCommand()
        {
            commandhead = command.CreateElement("Transfer");
            commandhead.SetAttribute("attribute", "Queue");
        }
    }
    /// <summary>
    /// 来电转分机
    /// </summary>
    public class QueueExt : QueueCommand
    {
        private XmlElement bodyelement;
        public QueueExt(string visitorid, string extid)
        {
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("ext");
            bodyelement.SetAttribute("id", extid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 来电转分机组队列
    /// </summary>
    public class QueueGroup : QueueCommand
    {
        private XmlElement bodyelement;
        public QueueGroup(string visitorid, string groupid)
        {
            bodyelement = command.CreateElement("visitor");
            bodyelement.SetAttribute("id", visitorid);
            commandbody.Add(bodyelement);

            bodyelement = command.CreateElement("group");
            bodyelement.SetAttribute("id", groupid);
            commandbody.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 配置命令基类
    /// </summary>
    public class Assign : Command
    {
        public XmlElement lv1body;
        public List<XmlElement> lv2body = new List<XmlElement>();
        public Assign()
        {
            commandhead = command.CreateElement("Control");
            commandhead.SetAttribute("attribute", "Assign");
        }

        public override void BuildXML()
        {
            if (lv2body != null)
            {
                foreach (XmlElement bodyelement in lv2body)
                {
                    lv1body.AppendChild(bodyelement);
                }
                commandhead.AppendChild(lv1body);
                command.AppendChild(commandhead);
                XmlCommandString = command.InnerXml;
            }
            else
            {
                commandhead.AppendChild(lv1body);
                command.AppendChild(commandhead);
                XmlCommandString = "";
            }
        }
    }
    /// <summary>
    /// 配置语音菜单
    /// </summary>
    public class AssignMenu : Assign
    {
        XmlElement bodyelement;
        public AssignMenu(string menuid, string voicefile)
        {
            lv1body = command.CreateElement("menu");
            lv1body.SetAttribute("id", menuid);

            bodyelement = command.CreateElement("voicefile");
            bodyelement.InnerText = voicefile;
            lv2body.Add(bodyelement);

            bodyelement = command.CreateElement("exit");
            bodyelement.InnerText = "#";
            lv2body.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 配置分机组
    /// </summary>
    public class AssignGroup : Assign
    {
        XmlElement bodyelement;
        public AssignGroup(string groupid , List<string> extlist ,string type)
        {
            lv1body = command.CreateElement("group");
            lv1body.SetAttribute("id", groupid);
            bodyelement = command.CreateElement("distribution");
            bodyelement.InnerText = type;
            lv2body.Add(bodyelement);

            foreach(string ext in extlist)
            {
                bodyelement = command.CreateElement("ext");
                bodyelement.InnerText = ext;
                lv2body.Add(bodyelement);
            }

            BuildXML();
        }
    }
    public class ClearGroup : Assign
    {
        XmlElement bodyelement;
        public ClearGroup(string groupid)
        {
            lv1body = command.CreateElement("group");
            lv1body.SetAttribute("id", groupid);
            bodyelement = command.CreateElement("distribution");           
            BuildXML();
        }
    }
    /// <summary>
    /// 配置自动应答，IP电话有效
    /// </summary>
    public class AssignAutoAnswer : Assign
    {
        XmlElement bodyelement;
        public AssignAutoAnswer(string lineid)
        {
            lv1body = command.CreateElement("ext");
            lv1body.SetAttribute("lineid", lineid);
            
        }
        public void On()
        {
            bodyelement = command.CreateElement("AutoAnswer");
            bodyelement.InnerText = "yes";
            lv2body.Add(bodyelement);
            BuildXML();
        }
        public void Off()
        {
            bodyelement = command.CreateElement("AutoAnswer");
            bodyelement.InnerText = "no";
            lv2body.Add(bodyelement);
            BuildXML();
        }
    }
    /// <summary>
    /// 配置夜服（自动转接）功能，开启On（），关闭Off（）
    /// </summary>
    public class AssignNightService : Assign
    {
        XmlElement lv2bodyelement;
        string fromlineid;
        string toid;
        public AssignNightService(string fromlineid, string toid)
        {
            this.fromlineid = fromlineid;
            this.toid = toid;
        }
        public void On()
        {
            lv1body = command.CreateElement("ext");       
            lv1body.SetAttribute("lineid", this.fromlineid);

            lv2bodyelement = command.CreateElement("Fwd_Type");
            lv2bodyelement.InnerText = "1";
            lv2body.Add(lv2bodyelement);

            lv2bodyelement = command.CreateElement("Fwd_Number");
            lv2bodyelement.InnerText = this.toid;
            lv2body.Add(lv2bodyelement);
            BuildXML();
        }
        public void Off()
        {
            lv1body = command.CreateElement("ext");
            lv1body.SetAttribute("lineid", this.fromlineid);

            lv2bodyelement = command.CreateElement("Fwd_Type");
            lv2bodyelement.InnerText = "0";
            lv2body.Add(lv2bodyelement);

            lv2bodyelement = command.CreateElement("Fwd_Number");
            lv2bodyelement.InnerText = "";
            lv2body.Add(lv2bodyelement);

            BuildXML();
        }
    }
    public class AcceptCommand : Command
    {
        XmlElement element;
        public AcceptCommand(string visitorid)
        {
            commandhead = command.CreateElement("Notify");
            commandhead.SetAttribute("attribute", "Accept");

            element = command.CreateElement("visitor");
            element.SetAttribute("id", visitorid);
            commandbody.Add(element);
            BuildXML();
        }
    }
}
