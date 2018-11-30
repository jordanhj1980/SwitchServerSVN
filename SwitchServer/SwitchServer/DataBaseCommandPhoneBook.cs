using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Diagnostics;
namespace SwitchServer
{
    public class DepartmentData
    {
        public string department;
        public string callno;
        public string name;
    }
    partial class DataBaseCommand
    {
        public bool GetPhoneBook(out GetPhoneBooksp respondstruct)
        {
            respondstruct = new GetPhoneBooksp();
            List<DepartmentData> departmentlist = new List<DepartmentData>();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("select callno,name,department from phonebook");

            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表
                    //从数据库获取所有数据
                    foreach (DataRow row in tbl.Rows)
                    {
                        DepartmentData member = new DepartmentData();


                        member.callno = row["callno"].ToString();
                        member.name = row["name"].ToString();
                        member.department = row["department"].ToString();
                        departmentlist.Add(member);                  
                    }
                    //获取所有部门
                    var queryResults = departmentlist.Select(c => c.department).Distinct();
                    foreach(var item in queryResults)
                    {
                        departmentstruct member = new departmentstruct();
                        member.department = item;
                        //获取对应部门的电话
                        foreach(DepartmentData element in departmentlist)
                        {
                            if(element.department.Equals(item))
                            {
                                contact contactmember = new contact();
                                contactmember.callno = element.callno;
                                contactmember.name = element.callno;
                                member.memberlist.Add(contactmember);
                            }
                        }
                        respondstruct.departmentlist.Add(member);
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("GetPhoneBook wrong:" + ex.Message);
                return false;
            }            
        }
        public bool EditPhoneBook(EditPhoneBookCmd structdata,out string reason)
        {
            List<DepartmentData> departmentlist = new List<DepartmentData>();

            List<departmentstruct> datalist = new List<departmentstruct> ();
            foreach(departmentstruct element in structdata.departmentlist)
            {
                foreach(contact member in element.memberlist)
                {
                    DepartmentData dataelement = new DepartmentData ();
                    dataelement.department = element.department;
                    dataelement.callno = member.callno;
                    dataelement.name = member.name;
                    departmentlist.Add(dataelement);
                }
            }

            int result;
            StringBuilder sqlstr = new StringBuilder();
            //删除phonebook表
            sqlstr.AppendFormat(@"truncate table phonebook");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reason = ex.Message;
                return false;
            }
            //向空表添加成员
            foreach (DepartmentData member in departmentlist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@" insert into phonebook (callno,name,department) values('{0}','{1}','{2}')", 
                    member.callno,member.name,member.department);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    reason = ex.Message;
                    return false;
                }
            }
            reason = "";
            return true;

        }
    }
}
