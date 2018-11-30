using System.Collections.Generic;
using System;

namespace SwitchServer
{
    /// <summary>
    /// 操作软交换命令数据结构，用于AddSW,EditSW
    /// </summary>
    public class AddEditSW
    {
        public string sequence;
        public string index;
        public string name;
        public string ip;
        public string port;
        public string type;
        public string username;
        public string password;
    }
    /// <summary>
    /// 通用命令应答数据结构，用于AddSW, DelSW, EditSW, EditAllDev
    /// </summary>
    public class SWRespond
    {
        public string sequence;
        public string index;
        public string result;
        public string reason;
    }
    /// <summary>
    /// 通用命令数据结构，用于DelSW,QuerySW，QueryAllDev，GetAllDev
    /// </summary>
    public class CommonCommand
    {
        public string sequence;
        public string index;
    }
    public class SwitchStruct
    {
        public string index;
        public string name;
        public string ip;
        public string port;
        public string type;
        public string username;
        public string password;
    }
    /// <summary>
    /// 查询软交换设备列表应答
    /// </summary>
    public class QuerySWsp
    {
        public string sequence;
        public List<SwitchStruct> switchlist;
    }
    /// <summary>
    /// 软交换下的设备
    /// </summary>
    public class DevStruct
    {
        public string callno;
        public string type;
        public string name;
        public string level;
        public string description;
    }
    /// <summary>
    /// 查询软交换下属设备列表应答,用于QueryAllDev，GetAllDev的应答
    /// </summary>
    public class QueryGetAllDevsp
    {
        public string sequence;
        public List<DevStruct> devlist;
    }
    /// <summary>
    /// 修改软交换下属设备列表
    /// </summary>
    public class EditAllDev
    {
        public string sequence;
        public string index;
        public List<DevStruct> devlist;
    }
    public class GetAllRegisterDev
    {
        public string sequence;
    }
    public class GetAllRegisterDevsp
    {
        public string sequence;
        public List<DevStruct> devlist;
    }
    /// <summary>
    /// 添加/修改调度员用于AddUser，EditUser
    /// </summary>
    public class AddEditUser
    {
        public string sequence;
        public string name;
        public string password;
        public string privilege;
        public string description;
        public string status;
        public string role;
        public string desk;
    }
    public class User
    {
        public string name;
        public string password;
        public string privilege;
        public string description;
        public string status;
        public string role;
        public string desk;
    }
    /// <summary>
    /// 查询所有调度员GetUser应答
    /// </summary>
    public class GetUsersp
    {
        public string sequence;
        public List<User> userlist;
    }
    /// <summary>
    /// 查询所有调度员GetUser
    /// </summary>
    public class GetUser
    {
        public string sequence;
    }
    /// <summary>
    /// 操作调度员通用应答 用于AddUser, DelUser, EditUser, AddKeyBoard,DelkeyBoard
    /// </summary>
    public class UserRespond
    {
        public string sequence;
        public string result;
        public string reason;
    }
    /// <summary>
    /// AddKeyBoard应答
    /// </summary>
    public class AddKeyBoardRespond
    {
        public string sequence;
        public string index;
        public string result;
        public string reason;
    }
    /// <summary>
    /// 删除调度员
    /// </summary>
    public class DelUser
    {
        public string sequence;
        public string name;
    }
    /// <summary>
    /// 调度键盘分组
    /// </summary>
    public class Group
    {
        public string index;
        public string groupname;
        public string column;
        public string description;
        public List<DevStruct> memberlist;
    }
    public class AddKeyBoard
    {
        public string sequence;
        public string index;
        public string name;
        public string mac;
        public string ip;
        public List<Group> grouplist;
        public List<DevStruct> hotlinelist;
        public List<broadcast> broadcastlist;
        public List<trunkdev> trunklist;
        public AddKeyBoard()
        {
            grouplist = new List<Group>();
            hotlinelist = new List<DevStruct>();
            broadcastlist = new List<broadcast>();
            trunklist = new List<trunkdev>();
        }
    }
    public class DelKeyBoard
    {
        public string sequence;
        public string index;
    }
    public class GetAllKeyboard
    {
        public string sequence;
    }
    public class broadcastmember
    {
        public string callno;
        public string name;
        //public string broadcast_index;
    }
    public class broadcast
    {
        public string index;
        public string name;
        public List<broadcastmember> bmemberlist;
        public broadcast()
        {
            bmemberlist = new List<broadcastmember>();
        }
    }
    public class trunkdev
    {
        public string trunkid;
        public string name;
        public string bindingnumber;
    }
    /// <summary>
    /// 调度键盘
    /// </summary>
    public class Keyboard
    {
        public string index;
        public string name;
        public string mac;
        public string ip;
        public List<Group> grouplist;
        public List<DevStruct> hotlinelist;
        public List<broadcast> broadcastlist;
        public List<trunkdev> trunklist;
        public Keyboard()
        {
            grouplist = new List<Group>();
            hotlinelist = new List<DevStruct>();
            broadcastlist = new List<broadcast>();
            trunklist = new List<trunkdev>();
        }
    }
    public class GetAllKeyboardsp
    {
        public string sequence;
        public List<Keyboard> keyboardlist;
    }
    public class GetPhoneBookCmd
    {
        public string sequence;
    }
    public class contact
    {
        public string callno;
        public string name;
    }
    public class departmentstruct
    {
        public string department;
        public List<contact>memberlist;
        public departmentstruct()
        {
            memberlist = new List<contact>();
        }
    }
    public class GetPhoneBooksp
    {
        public string sequence;
        public List<departmentstruct> departmentlist;
        public GetPhoneBooksp()
        {
            departmentlist = new List<departmentstruct>();
        }
    }
    public class EditPhoneBookCmd
    {
        public string sequence;
        public List<departmentstruct> departmentlist;
        public EditPhoneBookCmd()
        {
            departmentlist = new List<departmentstruct>();
        }
    }
    public class EditPhoneBooksp
    {
        public string sequence;
        public string result;
        public string reason;
    }
}