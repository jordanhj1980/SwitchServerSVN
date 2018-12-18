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
        /// <summary>
        /// 插入软交换设备，如果存在则失败。
        /// </summary>
        public bool InsertSwitch(ref AddEditSW addsw, out string reason)
        {
            reason = "";
            StringBuilder sqlstr = new StringBuilder();
            if (!IPCheck(addsw.ip))
            {
                reason = "IP地址不合法！";
                return false;
            }
            if (!IsUnsign(addsw.port)||addsw.port=="")
            {
                reason = "Port非法！";
                return false;
            }

            sqlstr.AppendFormat(
                "INSERT INTO switch (devname,ip,port,type,username,password) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')",
                addsw.name, addsw.ip, addsw.port, addsw.type, addsw.username, addsw.password);
            try
            {
                int tempindex = -1;
                //根据IP进行查找
                tempindex = QuerySwitchIP(addsw.ip);
                if (tempindex > 1)
                {
                    reason = "相同IP的软交换已存在！";
                    return false;
                    //DeleteSwitch(tempindex);//软交换设备已存在则删除
                }
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                sqlcommand.ExecuteNonQuery();
                tempindex = QuerySwitchIP(addsw.ip);
                addsw.index = tempindex.ToString();
                return true;//返回新插入软交换设备的序号
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                reason = "异常错误！！！";
                return false;
            }
        }
        /// <summary>
        /// 删除对应序号的软交换设备
        /// </summary>
        public bool DeleteSwitch(string index, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("delete from switch where index = {0}", index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int indexresult = sqlcommand.ExecuteNonQuery();

                if (indexresult > 0)
                {
                    reason = "";
                    return true;
                }
                else
                {
                    reason = "删除失败！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "数据库删除异常！";
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 修改软交换
        /// </summary>
        /// <param name="structdata"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool EditSwitch(AddEditSW structdata, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            if ((structdata.index == null) || (structdata.index == ""))
            {
                reason = "index为空";
                return false;
            }
            if (!IPCheck(structdata.ip))
            {
                reason = "IP地址不合法！";
                return false;
            }
            if (!IsUnsign(structdata.port) || structdata.port == "")
            {
                reason = "Port非法！";
                return false;
            }
            //判断哪些值需要更新
            string value = "";
            if (!((structdata.ip == null) || (structdata.ip == "")))
            {
                value += "ip ='" + structdata.ip + "',";
            }
            if (!((structdata.port == null) || (structdata.port == "")))
            {
                value += "port ='" + structdata.port + "',";
            }
            if (!((structdata.name == null) || (structdata.name == "")))
            {
                value += "devname ='" + structdata.name + "',";
            }
            if (!((structdata.password == null) || (structdata.password == "")))
            {
                value += "password ='" + structdata.password + "',";
            }
            if (!((structdata.type == null) || (structdata.type == "")))
            {
                value += "type ='" + structdata.type + "',";
            }
            if (!((structdata.username == null) || (structdata.username == "")))
            {
                value += "username ='" + structdata.username + "',";
            }
            value = value.Substring(0, value.Length - 1);
            sqlstr.AppendFormat("update switch set {0} where index = {1}", value, Convert.ToInt32(structdata.index));
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int indexresult = sqlcommand.ExecuteNonQuery();

                if (indexresult > 0)
                {
                    reason = "";
                    return true;
                }
                else
                {
                    reason = "EditSwitch失败！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "EditSwitch数据库异常！";
                Console.WriteLine(e.Message);
                return false;
            }

        }
        /// <summary>
        /// 获取所有软交换信息
        /// </summary>
        /// <param name="respondstruct"></param>
        /// <returns></returns>
        public bool QuerySW(out QuerySWsp respondstruct)
        {
            respondstruct = new QuerySWsp();
            respondstruct.switchlist = new List<SwitchStruct>();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("SELECT index,devname,ip,port,type,username,password from switch");

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    SwitchStruct member = new SwitchStruct();


                    member.index = row["index"].ToString();
                    member.name = row["devname"].ToString();
                    member.ip = row["ip"].ToString();
                    member.port = row["port"].ToString();
                    member.type = row["type"].ToString();
                    member.username = row["username"].ToString();
                    member.password = row["password"].ToString();

                    respondstruct.switchlist.Add(member);
                    //extinfo.grade = Convert.ToInt32(row["class"]);                    
                }
                return true;
            }
        }
        /// <summary>
        /// 向对应软交换查询下属设备列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="respondstruct"></param>
        /// <returns></returns>
        public bool QueryAllDev(string index, out QueryGetAllDevsp respondstruct)
        {
            respondstruct = new QueryGetAllDevsp();
            respondstruct.devlist = new List<DevStruct>();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("SELECT callno,type,name,level,description from member where switch_index = '{0}'", index);

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    DevStruct member = new DevStruct();


                    member.callno = row["callno"].ToString();
                    member.name = row["name"].ToString();
                    member.type = row["type"].ToString();
                    member.level = row["level"].ToString();
                    member.description = row["description"].ToString();

                    respondstruct.devlist.Add(member);
                    //extinfo.grade = Convert.ToInt32(row["class"]);                    
                }
                return true;
            }
        }
        /// <summary>
        /// 获取对应软交换的下属设备列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="respondstruct"></param>
        /// <returns></returns>
        public bool GetAllDev(string index, out QueryGetAllDevsp respondstruct)
        {
            respondstruct = new QueryGetAllDevsp();
            respondstruct.devlist = new List<DevStruct>();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("SELECT callno,type,name,level,description from member where switch_index = '{0}'", index);

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    DevStruct member = new DevStruct();


                    member.callno = row["callno"].ToString();
                    member.name = row["name"].ToString();
                    member.type = row["type"].ToString();
                    member.level = row["level"].ToString();
                    member.description = row["description"].ToString();

                    respondstruct.devlist.Add(member);
                    //extinfo.grade = Convert.ToInt32(row["class"]);                    
                }
                return true;
            }
        }
        /// <summary>
        /// 修改对应软交换的下属设备
        /// </summary>
        /// <param name="switchindex"></param>
        /// <param name="memberlist"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool EditAllDev(string switchindex, List<DevStruct> memberlist, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            if ((switchindex == null) || (switchindex == ""))
            {
                reason = "index为空";
                return false;
            }

            foreach (DevStruct member in memberlist)
            {
                if ((member.callno == null) || (switchindex == ""))
                {
                    reason = "callno为空";
                    return false;
                }
                //判断哪些值需要更新
                string value = "";
                if (!((member.type == null) || (member.type == "")))
                {
                    value += "type ='" + member.type + "',";
                }
                if (!((member.name == null) || (member.name == "")))
                {
                    value += "name ='" + member.name + "',";
                }
                if (!((member.level == null) || (member.level == "")))
                {
                    value += "level ='" + member.level + "',";
                }
                if (!((member.description == null) || (member.description == "")))
                {
                    value += "description ='" + member.description + "',";
                }

                value = value.Substring(0, value.Length - 1);
                sqlstr.Clear();
                sqlstr.AppendFormat("update member set {0} where switch_index = {1} and callno = '{2}'", value, switchindex, member.callno);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int indexresult = sqlcommand.ExecuteNonQuery();

                    if (indexresult > 0)
                    {
                    }
                    else
                    {
                        reason = "EditAllDev失败！";
                        return false;
                    }

                }
                catch (Exception e)
                {
                    reason = "EditAllDev数据库异常！" + e.Message;
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            reason = "";
            return true;
        }
        /// <summary>
        /// 获取所有的电话等设备资源列表
        /// </summary>
        /// <param name="respondstruct"></param>
        /// <returns></returns>
        public bool GetAllRegisterDev(out GetAllRegisterDevsp respondstruct)
        {
            respondstruct = new GetAllRegisterDevsp();
            respondstruct.devlist = new List<DevStruct>();

            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select callno,type,name,level,description from member");

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    DevStruct member = new DevStruct();


                    member.callno = row["callno"].ToString();
                    member.name = row["name"].ToString();
                    member.type = row["type"].ToString();
                    member.level = row["level"].ToString();
                    member.description = row["description"].ToString();

                    respondstruct.devlist.Add(member);                  
                }
                return true;
            }
        }
    }
}
