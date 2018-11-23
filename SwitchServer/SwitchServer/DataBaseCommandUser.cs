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
        /// 添加用户，name,password,privilege不能为空
        /// </summary>
        /// <param name="userdata"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool AddUser(AddEditUser userdata, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            string title = "";
            string value = "";
            if ((userdata.name == null) || (userdata.name == ""))
            {
                reason = "name为空";
                return false;
            }
            else
            {
                title += "name,";
                value += "'" + userdata.name + "',";
            }
            if ((userdata.password == null) || (userdata.password == ""))
            {
                reason = "password为空";
                return false;
            }
            else
            {
                title += "password,";
                value += "'" + userdata.password + "',";
            }

            if ((userdata.privilege == null) || (userdata.privilege == ""))
            {
                reason = "privilege为空";
                return false;
            }
            else
            {
                title += "privilege,";
                value += "'" + userdata.privilege + "',";
            }
            if ((userdata.description == null) || (userdata.description == ""))
            {
            }
            else
            {
                title += "description,";
                value += "'" + userdata.description + "',";
            }
            if ((userdata.status == null) || (userdata.status == ""))
            {
            }
            else
            {
                title += "status,";
                value += "'" + userdata.status + "',";
            }
            if ((userdata.role == null) || (userdata.role == ""))
            {
            }
            else
            {
                title += "role,";
                value += "'" + userdata.role + "',";
            }
            if ((userdata.desk == null) || (userdata.desk == ""))
            {
            }
            else
            {
                title += "deskassign,";
                value += "'" + userdata.desk + "',";
            }

            title = title.Substring(0, title.Length - 1);
            value = value.Substring(0, value.Length - 1);

            sqlstr.AppendFormat("insert into governer ({0}) select {1} where not exists (select * from governer where name = '{2}') limit 1",
                                title, value, userdata.name);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int resultindex = sqlcommand.ExecuteNonQuery();
                if (resultindex > 0)
                {
                    reason = "";
                    return true;
                }
                else
                {
                    reason = "AddUser存在重复用户！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "AddUser数据库异常！" + e.Message;
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <param name="respondstruct"></param>
        /// <returns></returns>
        public bool GetUser(out GetUsersp respondstruct)
        {
            respondstruct = new GetUsersp();
            respondstruct.userlist = new List<User>();

            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select name,password,privilege,description,status,role,deskassign from governer");

            DataSet ds = new DataSet();

            using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
            {
                sqldap.Fill(ds);
                DataTable tbl = ds.Tables[0];//获取第一张表

                foreach (DataRow row in tbl.Rows)
                {
                    User member = new User();


                    member.name = row["name"].ToString();
                    member.password = row["password"].ToString();
                    member.privilege = row["privilege"].ToString();
                    member.description = row["description"].ToString();
                    member.status = row["status"].ToString();
                    member.role = row["role"].ToString();
                    member.desk = row["deskassign"].ToString();

                    respondstruct.userlist.Add(member);
                    //extinfo.grade = Convert.ToInt32(row["class"]);                    
                }
                return true;
            }
        }
        /// <summary>
        /// 删除指定用户名的用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool DelUser(string name, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("delete from governer where name = '{0}'", name);
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
                    reason = "DelUser失败，不存在该用户！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "DelUser数据库删除异常！";
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="structdata"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool EditUser(AddEditUser structdata, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            if ((structdata.name == null) || (structdata.name == ""))
            {
                reason = "name为空";
                return false;
            }
            //判断哪些值需要更新
            string value = "";
            if (!((structdata.password == null) || (structdata.password == "")))
            {
                value += "password ='" + structdata.password + "',";
            }
            if (!((structdata.privilege == null) || (structdata.privilege == "")))
            {
                value += "privilege ='" + structdata.privilege + "',";
            }
            if (!((structdata.description == null) || (structdata.description == "")))
            {
                value += "description ='" + structdata.description + "',";
            }
            if (!((structdata.status == null) || (structdata.status == "")))
            {
                value += "status ='" + structdata.status + "',";
            }
            if (!((structdata.role == null) || (structdata.role == "")))
            {
                value += "role ='" + structdata.role + "',";
            }
            if (!((structdata.desk == null) || (structdata.desk == "")))
            {
                value += "deskassign ='" + structdata.desk + "',";
            }
            value = value.Substring(0, value.Length - 1);
            sqlstr.AppendFormat("update governer set {0} where name = '{1}'", value, structdata.name);
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
                    reason = "DelUser失败，未找到对应的用户！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "DelUser数据库异常！";
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}