using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace SwitchServer
{
    /// <summary>
    /// 用于管理向软交换发送命令的事件
    /// </summary>
    /// <param name="extid"></param>
    /// <param name="command"></param>
    public delegate void CommandSendHandler(List<string> extid,TypeData command);
    /// <summary>
    /// 软交换管理器，用于将客户端的命令下发到各个软交换的对象
    /// </summary>
    class SwitchManage
    {
        //软交换列表
        public static List<SwitchDev> switchlist = new List<SwitchDev>();
        public event CommandSendHandler CommandSendEvent;
        public SwitchManage(NpgsqlConnection conn)
        {
            DataBaseCommand sqlcom = new DataBaseCommand(conn);
            switchlist = sqlcom.GetAllSwitch(this);
        }
        public void CommandSend(List<string> extid,TypeData message)
        {
            if (CommandSendEvent != null)
            {
                CommandSendEvent(extid,message);
            }
        }
        /// <summary>
        /// 根据号码查找软交换
        /// </summary>
        /// <param name="extid"></param>
        /// <returns></returns>
        public SwitchDev GetSwitchFromExtid(List<string> extid,out string type)
        {
            bool result;
            foreach (SwitchDev d in switchlist)
            {
                result = d.IsMember(extid, out type);
                if (result)
                {
                    return d;
                }
                else
                {

                }
                    
            }
            type = "";
            return null;
        }
    }
    
}
