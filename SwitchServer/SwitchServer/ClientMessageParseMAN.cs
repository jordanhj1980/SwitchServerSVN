﻿using System;
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
        /// 解析MAN管理消息
        /// </summary>
        public bool ParseMANMessage(string datastr)
        {
            //Command command;
            TypeData message = ParseType(datastr);
            TypeData commanddata = ParseType(message.data);
            if (message.type.Equals("MAN"))
            {
                switch (commanddata.type)
                {
                    case "ADDSW"://解析添加软交换设备
                        AddSW(commanddata.data);
                        break;
                    case "DELSW"://解析删除软交换设备
                        DelSW(commanddata.data);
                        break;
                    case "EDITSW"://解析修改软交换设备
                        EditSW(commanddata.data);
                        break;
                    case "QUERYSW"://解析查询软交换设备列表
                        QuerySW(commanddata.data);
                        break;
                    case "QUERYALLDEV"://解析查询软交换下属设备列表
                        QueryAllDev(commanddata.data);
                        break;
                    case "GETALLDEV"://解析获取软交换下属设备列表
                        GetAllDev(commanddata.data);
                        break;
                    case "EDITALLDEV"://解析修改软交换下属设备列表
                        EditAllDev(commanddata.data);
                        break;
                    case "GETALLREGISTERDEV":
                        GetAllRegisterDev(commanddata.data);
                        break;
                    case "ADDUSER"://解析添加调度员
                        AddUser(commanddata.data);
                        break;
                    case "GETUSER":
                        GetUser(commanddata.data);
                        break;
                    case "DELUSER"://解析删除调度员
                        DelUser(commanddata.data);
                        break;
                    case "EDITUSER"://解析修改调度员
                        EditUser(commanddata.data);
                        break;
                    case "ADDKEYBOARD"://解析添加/修改调度键盘
                        AddKeyboard(commanddata.data);
                        break;
                    case "EDITKEYBOARD":
                        EditKeyboard(commanddata.data);
                        break;
                    case "GETALLKEYBOARD"://解析查询调度键盘
                        GetAllKeyboard(commanddata.data);
                        break;
                    case "DELKEYBOARD"://解析删除调度键盘
                        DelKeyboard(commanddata.data);
                        break;
                    case "":
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
        /// <summary>
        /// 解析添加软交换命令，发送对应应答
        /// </summary>
        /// <param name="data"></param>
        public bool AddSW(string data)
        {
            AddEditSW structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddEditSW>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！" ;
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            SWRespond responddata = new SWRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            
            string reason;
            if (sqlcom.InsertSwitch(ref structdata, out reason))
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Success";
                responddata.reason = "";
            }
            else
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Fail";
                responddata.reason = reason;
            }
            respondstr = "MAN#ADDSW#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        /// <summary>
        /// 解析删除软交换命令
        /// </summary>
        /// <param name="data"></param>
        private bool DelSW(string data)
        {
            CommonCommand structdata;
            string respondstr;
            try
            {
               structdata = JsonConvert.DeserializeObject<CommonCommand>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            SWRespond responddata = new SWRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            
            string reason;
            if (sqlcom.DeleteSwitch(structdata.index, out reason))
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Success";
                responddata.reason = "";
            }
            else
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Fail";
                responddata.reason = reason;
            }
            respondstr = "MAN#DELSW#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        /// <summary>
        /// 解析修改软交换命令
        /// </summary>
        /// <param name="data"></param>
        public bool EditSW(string data)
        {
            AddEditSW structdata;
            string respondstr;
            string reason;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddEditSW>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            SWRespond responddata = new SWRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            
            
            if (sqlcom.EditSwitch(structdata, out reason))
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Success";
                responddata.reason = "";
            }
            else
            {
                responddata.sequence = structdata.sequence;
                responddata.index = structdata.index;
                responddata.result = "Fail";
                responddata.reason = reason;
            }
            respondstr = "MAN#EDITSW#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        /// <summary>
        /// 解析查询所有软交换设备命令
        /// </summary>
        /// <param name="data"></param>
        public bool QuerySW(string data)
        {
            CommonCommand structdata;
            string respondstr;
            try
            {
                structdata =  JsonConvert.DeserializeObject<CommonCommand>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            QuerySWsp responddata;
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            if (sqlcom.QuerySW(out responddata))
            {
                responddata.sequence = structdata.sequence;
            }
            respondstr = "MAN#QUERYSW#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        /// <summary>
        /// 解析向对应软交换查询下属设备命令
        /// </summary>
        /// <param name="data"></param>
        public bool QueryAllDev(string data)
        {
            CommonCommand structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<CommonCommand>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            SwitchDev switchdev = sqlcom.GetSW(structdata.index);
            switchdev.index = structdata.index;
            if (switchdev != null)
            {
                Command command = new QueryDeviceInfo();
                string commandstr = command.XmlCommandString;
                TypeData com;
                com.type = "QueryDeviceInfo";
                com.data = commandstr;
                switchdev.ThreadPostRequest(com);
            }


            respondstr = "MAN#QueryAllDev#TEST";//协议暂定
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        
        /// <summary>
        /// 解析从数据库获取对应软交换下属设备列表
        /// </summary>
        /// <param name="data"></param>
        private bool GetAllDev(string data)
        {
            CommonCommand structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<CommonCommand>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }

            QueryGetAllDevsp responddata;
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            if (sqlcom.GetAllDev(structdata.index, out responddata))
            {
                responddata.sequence = structdata.sequence;
            }
            respondstr = "MAN#QueryAllDev#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool EditAllDev(string data)
        {
            EditAllDev structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<EditAllDev>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            SWRespond responddata = new SWRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            string reason;
            responddata.sequence = structdata.sequence;
            responddata.index = structdata.index;
            if (sqlcom.EditAllDev(structdata.index, structdata.devlist, out reason))
            {
                responddata.result = "Success";
                responddata.reason = reason;
            }
            {
                responddata.result = "Fail";
                responddata.reason = reason;
            }

            respondstr = "MAN#EDITALLDEV#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool GetAllRegisterDev(string data)
        {
            GetAllRegisterDev structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<GetAllRegisterDev>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            GetAllRegisterDevsp responddata;// = new GetAllRegisterDevsp();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            //string reason;
            
            
            if (sqlcom.GetAllRegisterDev(out responddata))
            {
                responddata.sequence = structdata.sequence;
            }
            {

            }

            respondstr = "MAN#GETALLREGISTERDEV#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool AddUser(string data)
        {
            AddEditUser structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddEditUser>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            string reason;
            responddata.sequence = structdata.sequence;
            if (sqlcom.AddUser(structdata, out reason))
            {
                responddata.result = "Success";
            }
            else
            {
                responddata.result = "Fail";
                responddata.reason = reason;
            }

            respondstr = "MAN#ADDUSER#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool GetUser(string data)
        {
            GetUser structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<GetUser>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            GetUsersp responddata = new GetUsersp();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            //string reason;

            if (sqlcom.GetUser(out responddata))
            {
                responddata.sequence = structdata.sequence;
            }
            else
            {

            }

            respondstr = "MAN#GETUSER#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool DelUser(string data)
        {
            DelUser structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<DelUser>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            string reason;
            responddata.sequence = structdata.sequence;
            if (sqlcom.DelUser(structdata.name, out reason))
            {
                responddata.result = "Success";
                responddata.reason = "";
            }
            else
            {
                responddata.result = "Fail";
                responddata.reason = reason;
            }

            respondstr = "MAN#DELUSER#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool EditUser(string data)
        {
            AddEditUser structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddEditUser>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);
            string reason;
            responddata.sequence = structdata.sequence;
            if (sqlcom.EditUser(structdata, out reason))
            {
                responddata.result = "Success";
            }
            else
            {
                responddata.result = "Fail";
                responddata.reason = reason;
            }

            respondstr = "MAN#EDITUSER#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool AddKeyboard(string data)
        {
            AddKeyBoard structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddKeyBoard>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            string reason;
            if (sqlcom.AddKeyboard(structdata, out reason))
            {
                responddata.reason = "";
                responddata.result = "Success";
            }
            else
            {
                responddata.reason = reason;
                responddata.result = "Fail";
            }
            respondstr = "MAN#ADDKEYBOARD#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        public bool EditKeyboard(string data)
        {
            AddKeyBoard structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<AddKeyBoard>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            string reason;
            if (sqlcom.EditKeyboard(structdata, out reason))
            {
                responddata.reason = "";
                responddata.result = "Success";
            }
            else
            {
                responddata.reason = reason;
                responddata.result = "Fail";
            }
            respondstr = "MAN#EDITKEYBOARD#" + JsonConvert.SerializeObject(responddata);
            Console.WriteLine(respondstr);
            clientsession.Send(respondstr);
            return true;
        }
        private bool GetAllKeyboard(string data)
        {
            GetAllKeyboard structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<GetAllKeyboard>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            GetAllKeyboardsp responddata;
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            if (sqlcom.GetAllKeyboard(out responddata))
            {
                responddata.sequence = structdata.sequence;
                respondstr = "MAN#GETALLKEYBOARD#" + JsonConvert.SerializeObject(responddata);
                clientsession.Send(respondstr);
                Console.WriteLine(respondstr);
                return true;
            }
            else
            {
                Console.WriteLine("GetAllKeyboard Wrong!!!");
                return false;
            }
        }
        public bool DelKeyboard(string data)
        {
            DelKeyBoard structdata;
            string respondstr;
            try
            {
                structdata = JsonConvert.DeserializeObject<DelKeyBoard>(data);
            }
            catch
            {
                respondstr = "Jason格式错误！！！";
                Console.WriteLine(respondstr);
                clientsession.Send(respondstr);
                return false;
            }
            UserRespond responddata = new UserRespond();
            DataBaseCommand sqlcom = new DataBaseCommand(Program.conn);

            string reason;
            responddata.sequence = structdata.sequence;
            if (sqlcom.DelKeyboard(structdata.index, out reason))
            {
                responddata.reason = "";
                responddata.result = "Success";
                respondstr = "MAN#DELKEYBOARD#" + JsonConvert.SerializeObject(responddata);
                clientsession.Send(respondstr);
            }
            else
            {
                responddata.reason = reason;
                responddata.result = "Fail";
                respondstr = "MAN#DELKEYBOARD#" + JsonConvert.SerializeObject(responddata);
                clientsession.Send(respondstr);
                Console.WriteLine("DelKeyboard Wrong!!!");
            }
            return true;
        }
    }
}