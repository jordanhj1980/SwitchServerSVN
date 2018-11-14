using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
namespace SwitchServer
{
    partial class DataBaseCommand
    {
        NpgsqlConnection conn;
        public DataBaseCommand(NpgsqlConnection iconn)
        {
            this.conn = iconn;
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }    
            }
            catch(Exception ex)
            {
                Console.WriteLine("DataBaseCommand wrong:"+ex.Message);
            }
        }
        /// <summary>
        /// 更新数据库对应软交换的设备列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="extlist"></param>
        /// <returns></returns>
        public bool UpdateSwitchDev(string index ,List<ExtDevice> extlist)
        {
            foreach(ExtDevice member in extlist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat("update member set lineid = '{0}', type = '{1}' where callno = '{2}' and switch_index = '{3}'",
                                    member.lineid,member.type,member.extid, index);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int result = sqlcommand.ExecuteNonQuery();
                    //update失败说明是新信息，需要插入
                    if(result<=0)
                    {
                        InsertSwitchDev(index, member);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("UpdateSwitchDev wrong:"+e.Message);
                }
            }

            return true;
        }
        /// <summary>
        /// 插入对应软交换的一个设备
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        private bool InsertSwitchDev(string index,ExtDevice ext)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("insert into member (callno,name,lineid,type,switch_index) values('{0}','{1}','{2}','{3}','{4}')",
                                ext.extid,ext.extid, ext.lineid,ext.type, index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int result = sqlcommand.ExecuteNonQuery();
                if (result <= 0)
                {
                    Console.WriteLine("insert dev wrong!!!");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("InsertSwitchDev wrong:"+ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 从数据库获取所有的软交换设备
        /// </summary>
        public List<SwitchDev> GetAllSwitch(SwitchManage switchmanage)
        {
            List<SwitchDev> switchlist = new List<SwitchDev>();
            SwitchDev groupdata =null;
            SwitchInfo switchinfo;
            //string index;
            //string type;
            //string ip;
            //string port;
            StringBuilder sqlstr = new StringBuilder();
            //System.Net.IPAddress ipaddress;
            sqlstr.AppendFormat("SELECT devname,index,type,ip,port from switch");

            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        switchinfo.name = row["devname"].ToString();
                        switchinfo.index = row["index"].ToString();
                        switchinfo.type = row["type"].ToString();
                        //switchinfo.ip = new System.Net.IPAddress(Convert.ToInt64(row["ip"])).ToString();
                        switchinfo.ip = row["ip"].ToString();
                        switchinfo.port = row["port"].ToString();
                        switch (switchinfo.type)
                        {
                            case "1":
                                groupdata = new NewRockTech(switchmanage, switchinfo, conn);
                                //groupdata.GetExtFromDB(Program.conn);
                                break;
                            default:
                                groupdata = new NewRockTech(switchmanage, switchinfo, conn);
                                break;
                        }
                        switchlist.Add(groupdata);
                        //extinfo.grade = Convert.ToInt32(row["class"]);                    
                    }
                    return switchlist;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("GetAllSwitch wrong:"+ex.Message);
                return switchlist;
            }
            
        }
        
        
        /// <summary>
        /// 插入对应软交换下的话机
        /// </summary>
        public void InsertMember(string extid ,string lineid,int switch_index)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("INSERT INTO member (id,callno,switch_index) VALUES ('{0}','{1}',{2})", extid, lineid, switch_index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                sqlcommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("InsertMember wrong:"+ex.Message);
            }
        }
        /// <summary>
        /// 查询与输入IP相同的软交换设备的序号
        /// </summary>
        public int QuerySwitchIP(string ip)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("SELECT index from switch where ip='{0}'",ip);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int temp = Convert.ToInt32(sqlcommand.ExecuteScalar());
                return temp;
            }
            catch (Exception e)
            {
                Console.WriteLine("QuerySwitchIP wrong:"+e.Message);
                return -1;
            }
        }
        /// <summary>
        /// 清空对应名字的表
        /// </summary>
        public void ClearTable(string tablename)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("delete from {0}", tablename);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("ClearTable wrong:"+e.Message);
            }
        }

        /// <summary>
        /// 根据index查找并生成对应的软交换对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SwitchDev GetSW(string index)
        {

            SwitchInfo switchinfo = new SwitchInfo();
            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("SELECT devname,index,type,ip,port from switch where index = {0}", index);

            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        switchinfo.name = row["devname"].ToString();
                        switchinfo.index = row["index"].ToString();
                        switchinfo.type = row["type"].ToString();
                        switchinfo.ip = row["ip"].ToString();
                        switchinfo.port = row["port"].ToString();

                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("GetSW wrong:"+ex.Message);
            }
            
            SwitchDev switchdev;
            switch (switchinfo.type)
            {
                case "1":
                    switchdev = new NewRockTech(Program.switchmanage, switchinfo, conn);
                    //groupdata.GetExtFromDB(Program.conn);
                    break;
                default:
                    switchdev = new NewRockTech(Program.switchmanage, switchinfo, conn);
                    break;
            }
            return switchdev;
        }
        /// <summary>
        /// 查询对应序号软交换设备下的话机
        /// </summary>
        public List<ExtDevice> GetExtList(int switchindex)
        {
            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select * from member where switch_index = {0}", switchindex);
            List<ExtDevice> listext = new List<ExtDevice>();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表
                    
                    foreach (DataRow row in tbl.Rows)
                    {
                        ExtDevice extinfo = new ExtDevice();
                        extinfo.extid = row["callno"].ToString();
                        extinfo.lineid = row["lineid"].ToString();
                        extinfo.type = row["type"].ToString();
                        //extinfo.grade = Convert.ToInt32(row["class"]);
                        listext.Add(extinfo);
                    }
                    return listext;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return listext;
            }
        }
        /// <summary>
        /// 查询用户名密码是否正确
        /// </summary>
        public bool LogInfoCheck(ref LogInfo loginfo)
        {
            DataSet ds = new DataSet();
            int temp = -1;
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select index,privilege from governer where name = '{0}' and password = '{1}'", loginfo.name, loginfo.pwd);
            try
            {
                NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn);
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    temp = Convert.ToInt32(row["index"]);
                    loginfo.type = row["privilege"].ToString();
                }                          

                if (temp > 0)
                {
                    return true;
                }                   
                else
                {
                    return false;
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取对应用户的控制台ID
        /// </summary>
        /// <param name="登录用户名"></param>
        /// <param name="登录密码"></param>
        /// <returns></returns>
        public string GetDeskIndex(string name, string pwd)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select deskassign from governer where name = '{0}' and password = '{1}'", name,pwd);
            NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
            return sqlcommand.ExecuteScalar().ToString();
        }
        /// <summary>
        /// 获取对应用户的可管理设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public List<GroupData> GetGroupExt(string name,string pwd)
        {
            List<GroupData> groupdatalist = new List<GroupData>();
            DataSet ds = new DataSet();
            string index = GetDeskIndex(name,pwd);
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select desk_grp_index,callno from deskgrp,deskgrp_mem where deskgrp.desk_index = '{0}' and deskgrp.index = deskgrp_mem.desk_grp_index", index);
            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表
                    
                    foreach (DataRow row in tbl.Rows)
                    {
                        GroupData groupdata = new GroupData();
                        groupdata.groupid = row["desk_grp_index"].ToString();
                        groupdata.extid = row["callno"].ToString();
                        //extinfo.grade = Convert.ToInt32(row["class"]);
                        groupdatalist.Add(groupdata);
                    }
                    return groupdatalist;
            }
                
        }
        /// <summary>
        /// 获取广播组成员，暂定所有分机
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public List<GroupData> GetBroadCast(string name, string pwd)
        {
            List<GroupData> groupdatalist = new List<GroupData>();
            DataSet ds = new DataSet();
            string index = GetDeskIndex(name, pwd);
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select distinct callno from deskgrp,deskgrp_mem where deskgrp.desk_index = '{0}' and deskgrp.index = deskgrp_mem.desk_grp_index", index);
            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    GroupData groupdata = new GroupData();
                    groupdata.groupid = "B";
                    groupdata.extid = row["callno"].ToString();
                    //extinfo.grade = Convert.ToInt32(row["class"]);
                    groupdatalist.Add(groupdata);
                }
                return groupdatalist;
            }

        }
        /// <summary>
        /// 获取对应用户的键权电话
        /// </summary>
        /// <returns></returns>
        public List<GroupData> GetKeyExt(string name, string pwd)
        {
            List<GroupData> keylist = new List<GroupData>();
            DataSet ds = new DataSet();
            string index = GetDeskIndex(name, pwd);
            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select callno from desk_totalmem where desk_index = '{0}' and key = true", index);
            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    GroupData groupdata = new GroupData();
                    groupdata.groupid = "0";//表示键权电话
                    groupdata.extid = row["callno"].ToString();
                    //extinfo.grade = Convert.ToInt32(row["class"]);
                    keylist.Add(groupdata);
                }
                return keylist;
            }
        }
        /// <summary>
        /// 获取中继设备列表
        /// </summary>
        /// <returns></returns>
        public List<GroupData> GetTrunk()
        {
            List<GroupData> keylist = new List<GroupData>();
            DataSet ds = new DataSet();
            //string index = GetDeskIndex(name, pwd);
            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select callno from member where type = 'trunk'");
            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    GroupData groupdata = new GroupData();
                    groupdata.groupid = "T";//表示中继
                    groupdata.extid = row["callno"].ToString();
                    //extinfo.grade = Convert.ToInt32(row["class"]);
                    keylist.Add(groupdata);
                }
                return keylist;
            }
        }
        
        /// <summary>
        /// 插入通话记录
        /// </summary>
        /// <param name="cdrdata"></param>
        /// <returns></returns>
        public bool InsertCDR(CDR cdrdata)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat(@"INSERT INTO cdr (cdrid,callid,type,timestart,timeend,cpn,cdpn,duration) 
                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                cdrdata.Cdrid, cdrdata.callid, cdrdata.type, cdrdata.TimeStart, cdrdata.TimeEnd, cdrdata.CPN, cdrdata.CDPN, cdrdata.Duration);

            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int result = sqlcommand.ExecuteNonQuery();
                if(result>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取通话记录
        /// </summary>
        /// <returns></returns>
        public List<CDR> GetCDR()
        {
            List<CDR>cdrlist = new List<CDR>();

            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select cdrid,callid,type,timestart,timeend,cpn,cdpn,duration from cdr order by index desc limit 50");

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    CDR member = new CDR();
                    member.Cdrid = row["cdrid"].ToString();
                    member.callid = row["callid"].ToString();
                    member.type = row["type"].ToString();
                    member.TimeStart = row["timestart"].ToString();
                    member.TimeEnd = row["timeend"].ToString();
                    member.CPN = row["cpn"].ToString();
                    member.CDPN = row["cdpn"].ToString();
                    member.Duration = row["duration"].ToString();
                    cdrlist.Add(member);
                }
            }
            return cdrlist;
        }
        /// <summary>
        /// 插入用户操作记录
        /// </summary>
        /// <param name="cdrdata"></param>
        /// <returns></returns>
        public bool InsertUserLog(string name,string actiontype)
        {
            DateTime currentTime = new DateTime ();
            currentTime = System.DateTime.Now;
            string timestr = currentTime.ToString();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat(@"insert into userlog (username,actiontype,time) 
                                VALUES ('{0}','{1}','{2}')",
                                name, actiontype, timestr);

            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int result = sqlcommand.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取用户操作记录
        /// </summary>
        /// <returns></returns>
        public List<UserLog> GetUserLog()
        {
            List<UserLog> userloglist = new List<UserLog>();

            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select name,actiontype,time,from userlog");

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    UserLog member = new UserLog();
                    member.name = row["name"].ToString();
                    member.actiontype = row["actiontype"].ToString();
                    member.time = row["time"].ToString();

                    userloglist.Add(member);
                }
            }
            return userloglist;
        }
    }
}
