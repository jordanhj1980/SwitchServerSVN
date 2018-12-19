using SuperWebSocket;
using System.Collections.Generic;
using System;
namespace SwitchServer
{
    public struct CMDRespond
    {
        public string result;
        public string reason;
    }
    public struct FailCalldata
    {
        public string fromid;
        public string toid;
        public string reason;
    }
    public struct CallData
    {
        public string fromid;
        public string toid;
        public string visitorid;
        public string outerid;
        public string callid;
    }		
    public struct CallOut
    {
        public string fromid;
        public string toid;
        public string trunkid;
    }
    public struct SetMenu
    {
        public string menuid;
        public string voicefile;
    }
    public class AssignGroupCMD
    {
        public string distribution;
        public List<string> devlist;
    }
    /// <summary>
    /// 通话记录结构体
    /// </summary>
    public struct CDR
    {
        public string Cdrid;//话单id
        public string callid;//通话的相对唯一标识
        public string type;//通话类型 IN(打入)/OU(打出)/FI(呼叫转移入)/FW(呼叫转 移出)/LO(内部通话)/CB(双向外呼)
        public string TimeStart;//呼叫起始时间，即发送或收到呼叫请求的时间
        public string TimeEnd;//呼叫结束时间，即通话的一方挂断的时间
        public string CPN;//主叫号码
        public string CDPN;//被叫号码
        public string Duration;//通话时长，值为 0 说明未接通。
    }

    public struct TypeData
    {
        public string type;
        public string data;
        public WebSocketSession clientsession;
    };
    public struct IpTypeData
    {
        public string ip;
        public TypeData typedata;
    }
    public struct SessionTypeData
    {
        public WebSocketSession clientsession;
        public string type;
        public string data;
    }
    /// <summary>
    /// 用于向客户端上报消息
    /// </summary>
    public class ReportMessage
    {
        public List<string> extid;
        public string message;
        public ReportMessage()
        {
            this.extid = new List<string>();
        }
    }
    //public struct SessionData
    //{
    //    public string sessionid;
    //    public string type;
    //    public string data;
    //};
    //通话记录
    public struct callsession
    {
        public string trunkid;
        public string visitorid;
        public string fromnumber;
        public string tonumber;
        public string callid;
    };
    //话机设备属性
    public class ExtDevice
    {
        public string extid;//分机号
        public string lineid;//线路编号，是分机的唯一固定标识
        public string type;//是分机还是中继线路
        //public List<string> groupid;
        public string staffid;//工号
        public string Call_pickup;//代接权限
        public string Fwd_Type;//呼叫转移方式
        public string Fwd_Number;//呼叫转移号码
        public string Call_Restriction;//呼叫权限
        //public string Off_Line_Num;
        public string mobile;//分机绑定的手机号
        public string fork;//同振号码
        //public string email;
        public string record;//实时录音功能开关
        public string api;//API功能开关
        //public string voicefile;
        public string AutoAnswer;//自动应答功能，只支持IP电话
        public string state;//状态
    };
    //中继设备属性
    public struct TrunkDevice
    {
        public string trunkid;//中继（外线）号
        public string lineid;//中继的线路编号，是中继的唯一固定标识
        public string state;//状态
    };
    //软交换设备属性
    public struct SwitchInfo
    {
        public string index;
        public string name;
        public string ip;
        public string port;
        public string type;
    };
    
    //用户信息
    public struct LogInfo
    {
        public string name;//用户名
        public string pwd;//密码
        public string type;//用户类型
        //public string sessionid;
        public string ip;//用户IP
        public WebSocketSession clientsession;
    }

    public struct GroupData
    {
        public string groupid;
        public string extid;
        public string name;
    }
    public class Broadcast
    {
        public string name;
        public List<broadcastmember> bmemberlist;
        public Broadcast()
        {
            bmemberlist = new List<broadcastmember>();
        }
    }
    public struct UserLog
    {
//        public string name;
        public string time;
        public string actiontype;    
    }
}
