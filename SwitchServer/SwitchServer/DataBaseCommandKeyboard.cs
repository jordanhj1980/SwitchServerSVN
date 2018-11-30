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
        /// 获取对应组编号的成员信息
        /// </summary>
        /// <param name="groupindex"></param>
        /// <param name="memberlist"></param>
        /// <returns></returns>
        private bool GetGroupMember(string groupindex, out List<DevStruct> memberlist)
        {
            memberlist = new List<DevStruct>();

            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();

            Group igroup = new Group();
            sqlstr.AppendFormat("select callno,name,type,level,description from deskgrp_mem where desk_grp_index = {0}", groupindex);//获取所有的控制台
            try
            {
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

                        memberlist.Add(member);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetGroupMember数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 添加新的调度键盘
        /// </summary>
        /// <param name="structdata"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool AddKeyboard(AddKeyBoard structdata, out string reason, out string index)
        {
            StringBuilder sqlstr = new StringBuilder();
            string title = "";
            string value = "";
            if ((structdata.name == null) || (structdata.name == ""))
            {
                reason = "name不能为空！";
                index = "";
                return false;
            }
            title = "name,ip,mac";
            value = "'" + structdata.name + "','" + structdata.ip + "','" + structdata.mac + "'";

            sqlstr.AppendFormat("insert into desk ({0}) select {1} where not exists (select * from desk where name = '{2}') limit 1 RETURNING index",
                    title, value, structdata.name);

            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                //int resultindex = sqlcommand.ExecuteNonQuery();
                int deskindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                if (deskindex > 0)
                {
                    //添加键权电话
                    InsertHotlineList(structdata.hotlinelist, deskindex.ToString());
                    //添加分组
                    InsertDeskGroup(structdata.grouplist, deskindex.ToString());
                    //添加广播按钮
                    InsertDeskBroadcast(structdata.broadcastlist, deskindex.ToString());
                    //添加中继列表
                    InsertTrunkList(structdata.trunklist, deskindex.ToString());
                    reason = "";
                    index = deskindex.ToString();
                    return true;
                }
                else
                {
                    
                    //reason = "AddKeyboard存在重复！";
                    index = structdata.index;
                    return (EditKeyboard(structdata, out reason));
                }

            }
            catch (Exception e)
            {
                reason = "AddKeyboard数据库异常！" + e.Message;
                Console.WriteLine(e.Message);
                index = "";
                return false;
            }
        }
        /// <summary>
        /// 修改调度键盘信息
        /// </summary>
        /// <param name="structdata"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool EditKeyboard(AddKeyBoard structdata, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();

            if ((structdata.index == null) || (structdata.index == ""))
            {
                reason = "index不能为空！";
                return false;
            }
            //sqlstr.AppendFormat("insert into desk ({0}) select {1} from desk where not exists (select * from desk where name = '{2}') limit 1 RETURNING index",
            //        title, value, structdata.name);
            sqlstr.AppendFormat(@"  update desk
                                    set name='{0}', ip='{1}', mac='{2}'
                                    where index = {3}",
                                    structdata.name, structdata.ip, structdata.mac, structdata.index);

            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                int resultindex = sqlcommand.ExecuteNonQuery();
                //int deskindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                if (resultindex > 0)
                {
                    //修改键权电话
                    EditHotlineList(structdata.hotlinelist, structdata.index);
                    //修改分组
                    EditDeskGroup(structdata.grouplist, structdata.index);
                    //修改广播按钮
                    EditBroadcast(structdata.broadcastlist, structdata.index);
                    //修改中继列表
                    EditTrunkList(structdata.trunklist, structdata.index);
                    reason = "";
                    return true;
                }
                else
                {
                    reason = "EditKeyboard失败！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "AddKeyboard数据库异常！" + e.Message;
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool EditHotlineList(List<DevStruct> hotlinelist, string index)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table test(callno varchar(10))");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (DevStruct member in hotlinelist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@" insert into test (callno) values('{0}')", member.callno);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            //删除desk_totalmem中不在test中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from desk_totalmem 
                                    where desk_index = '{0}' and
                                    not exists(select * from test where desk_totalmem.callno = test.callno);
                                    drop table test;",
                                    index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditHotlineList"+e.Message);
                return false;
            }
            //在desk_totalmem中修改对应成员，没有就增加
            foreach (DevStruct member in hotlinelist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@"  update desk_totalmem set type='{1}',name='{2}',level='{3}',description='{4}'
                                        where desk_index='{5}' and callno = '{0}'",
                                        member.callno, member.type, member.name, member.level, member.description, index);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                    if (result > 0)
                    {

                    }
                    else
                    {
                        sqlstr.Clear();
                        sqlstr.AppendFormat(@"  insert into desk_totalmem (callno,desk_index,key,name,type,level,description)
                                                values('{0}','{1}',{2},'{3}','{4}','{5}','{6}')",
                                                member.callno, index, "true", member.name, member.type, member.level, member.description);
                        try
                        {
                            NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                            result = sqlcommand1.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("EditHotlineList"+e.Message);
                            return false;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        private bool EditTrunkList(List<trunkdev> trunklist, string desk_index)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table test(trunkid varchar(10))");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditTrunkList"+e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (trunkdev member in trunklist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@" insert into test (trunkid) values('{0}')", member.trunkid);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditTrunkList"+e.Message);
                    return false;
                }
            }
            //删除desk_trunk中不在test中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from desk_trunk 
                                    where desk_index = '{0}' and
                                    not exists(select * from test where desk_trunk.trunkid = test.trunkid);
                                    drop table test;",
                                    desk_index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditTrunkList"+e.Message);
                return false;
            }
            //在desk_trunk中修改对应成员，没有就增加
            foreach (trunkdev member in trunklist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@"  update desk_trunk set name='{0}',bindingnumber='{1}'
                                        where desk_index='{2}' and trunkid = '{3}'",
                                        member.name, member.bindingnumber,desk_index,member.trunkid);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                    if (result > 0)
                    {

                    }
                    else
                    {
                        sqlstr.Clear();
                        sqlstr.AppendFormat(@"  insert into desk_trunk (trunkid,name,bindingnumber,desk_index)
                                                values('{0}','{1}','{2}','{3}')",
                                                member.trunkid,member.name,member.bindingnumber,desk_index);
                        try
                        {
                            NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                            result = sqlcommand1.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("EditTrunkList"+e.Message);
                            return false;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditTrunkList"+e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 修改分组信息
        /// </summary>
        /// <param name="grouplist"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool EditDeskGroup(List<Group> grouplist, string index)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table testgroup(index int)");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditDeskGroup"+e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (Group member in grouplist)
            {
                if (member.index != null)
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@" insert into testgroup (index) values('{0}')", member.index);
                    try
                    {
                        NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                        result = sqlcommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditDeskGroup"+e.Message);
                        //return false;
                    }
                }
            }
            //删除deskgrp中不在test中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from deskgrp 
                                    where desk_index = '{0}' and
                                    not exists(select * from testgroup where deskgrp.index = testgroup.index);
                                    drop table testgroup;",
                                    index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditDeskGroup"+e.Message);
                return false;
            }
            //在deskgrp中修改对应成员，没有就增加
            foreach (Group member in grouplist)
            {
                if (member.index != null)
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@"  update deskgrp set col='{1}',description='{2}',name = '{3}'
                                        where desk_index='{4}' and index = '{0}'",
                                            member.index, member.column, member.description, member.groupname, index);
                    try
                    {
                        NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                        result = sqlcommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            EditGroupMemberList(member.memberlist, member.index);
                        }
                        else
                        {
                            sqlstr.Clear();
                            sqlstr.AppendFormat(@"  insert into deskgrp (name,desk_index,col,description)
                                                values('{0}','{1}','{2}','{3}') returning index",
                                                    member.groupname, index, member.column, member.description);
                            try
                            {
                                NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                                int groupindex = Convert.ToInt32(sqlcommand1.ExecuteScalar());
                                EditGroupMemberList(member.memberlist, groupindex.ToString());
                                //result = sqlcommand1.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("EditDeskGroup"+e.Message);
                                return false;
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditDeskGroup"+e.Message);
                        return false;
                    }
                }
                else
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@"  insert into deskgrp (name,desk_index,col,description)
                                                values('{0}','{1}','{2}','{3}') returning index",
                                            member.groupname, index, member.column, member.description);
                    try
                    {
                        NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                        int groupindex = Convert.ToInt32(sqlcommand1.ExecuteScalar());
                        EditGroupMemberList(member.memberlist, groupindex.ToString());
                        //result = sqlcommand1.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditDeskGroup"+e.Message);
                        return false;
                    }
                }

            }
            return true;
        }
        /// <summary>
        /// 修改调度键盘分组成员信息
        /// </summary>
        /// <param name="memberlist"></param>
        /// <param name="groupindex"></param>
        /// <returns></returns>
        private bool EditGroupMemberList(List<DevStruct> memberlist, string groupindex)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table testgroupmember(callno varchar(10))");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditGroupMemberList"+e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (DevStruct member in memberlist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@" insert into testgroupmember (callno) values('{0}')", member.callno);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditGroupMemberList"+e.Message);
                    return false;
                }
            }
            //删除deskgrp中不在test中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from deskgrp_mem 
                                    where desk_grp_index = '{0}' and
                                    not exists(select * from testgroupmember where deskgrp_mem.callno = testgroupmember.callno);
                                    drop table testgroupmember;",
                                    groupindex);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditGroupMemberList"+e.Message);
                return false;
            }
            //在deskgrp_mem中修改对应成员，没有就增加
            foreach (DevStruct member in memberlist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@"  update deskgrp_mem set name='{1}',type='{2}',level='{3}',description = '{4}'
                                        where desk_grp_index='{5}' and callno = '{0}'",
                                        member.callno, member.name, member.type, member.level, member.description, groupindex);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                    if (result > 0)
                    {

                    }
                    else
                    {
                        sqlstr.Clear();
                        sqlstr.AppendFormat(@"  insert into deskgrp_mem (callno,desk_grp_index,name,type,level,description)
                                                values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                member.callno, groupindex, member.name, member.type, member.level, member.description);
                        try
                        {
                            NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                            result = sqlcommand1.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("EditGroupMemberList"+e.Message);
                            return false;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditGroupMemberList"+e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 修改广播按钮列表
        /// </summary>
        /// <param name="broadcastlist"></param>
        /// <param name="desk_index"></param>
        /// <returns></returns>
        private bool EditBroadcast(List<broadcast> broadcastlist, string desk_index)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table testbroadcast(index int)");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditBroadcast"+e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (broadcast member in broadcastlist)
            {
                if (member.index != null)
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@" insert into testbroadcast (index) values('{0}')", member.index);
                    try
                    {
                        NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                        result = sqlcommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditBroadcast"+e.Message);
                        //return false;
                    }
                }
            }
            //删除broadcast中不在testbroadcast中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from broadcast 
                                    where desk_index = '{0}' and
                                    not exists(select * from testbroadcast where broadcast.index = testbroadcast.index);
                                    drop table testbroadcast;",
                                    desk_index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditBroadcast"+e.Message);
                return false;
            }
            //在deskgrp中修改对应成员，没有就增加
            foreach (broadcast member in broadcastlist)
            {
                if (member.index != null)
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@"  update broadcast set name='{0}'
                                        where desk_index='{1}' and index = '{2}'",
                                             member.name, desk_index, member.index);
                    try
                    {
                        NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                        result = sqlcommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            EditBroadcastMemberList(member.bmemberlist, member.index);
                        }
                        else
                        {
                            sqlstr.Clear();
                            sqlstr.AppendFormat(@"  insert into broadcast (name,desk_index)
                                                values('{0}','{1}') returning index",
                                                    member.name, desk_index);
                            try
                            {
                                NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                                int broadcast_index = Convert.ToInt32(sqlcommand1.ExecuteScalar());
                                EditBroadcastMemberList(member.bmemberlist, broadcast_index.ToString());
                                //result = sqlcommand1.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("EditBroadcast"+e.Message);
                                return false;
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditBroadcast"+e.Message);
                        return false;
                    }
                }
                else
                {
                    sqlstr.Clear();
                    sqlstr.AppendFormat(@"  insert into broadcast (name,desk_index)
                                                values('{0}','{1}') returning index",
                                            member.name, desk_index);
                    try
                    {
                        NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                        int broadcast_index = Convert.ToInt32(sqlcommand1.ExecuteScalar());
                        EditBroadcastMemberList(member.bmemberlist, broadcast_index.ToString());
                        //result = sqlcommand1.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EditBroadcast"+e.Message);
                        return false;
                    }
                }

            }
            return true;
        }
        /// <summary>
        /// 修改调度键盘广播按钮成员信息
        /// </summary>
        /// <param name="memberlist"></param>
        /// <param name="broadcast_index"></param>
        /// <returns></returns>
        private bool EditBroadcastMemberList(List<broadcastmember> memberlist, string broadcast_index)
        {
            int result;
            StringBuilder sqlstr = new StringBuilder();
            //建临时表test
            sqlstr.AppendFormat(@"  create temp table testbroadcastmember(callno varchar(10))");
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditBroadcastMemberList"+e.Message);
                return false;
            }
            //向临时表添加成员
            foreach (broadcastmember member in memberlist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@" insert into testbroadcastmember (callno) values('{0}')", member.callno);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditBroadcastMemberList"+e.Message);
                    return false;
                }
            }
            //删除deskgrp中不在test中的元素
            sqlstr.Clear();
            sqlstr.AppendFormat(@"  delete from broadcast_member 
                                    where broadcast_index = '{0}' and
                                    not exists(select * from testbroadcastmember where broadcast_member.callno = testbroadcastmember.callno);
                                    drop table testbroadcastmember;",
                                    broadcast_index);
            try
            {
                NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                result = sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("EditBroadcastMemberList"+e.Message);
                return false;
            }
            //在deskgrp_mem中修改对应成员，没有就增加
            foreach (broadcastmember member in memberlist)
            {
                sqlstr.Clear();
                sqlstr.AppendFormat(@"  update broadcast_member set name='{0}'
                                        where broadcast_index='{1}' and callno = '{2}'",
                                         member.name, broadcast_index, member.callno);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    result = sqlcommand.ExecuteNonQuery();
                    if (result > 0)
                    {

                    }
                    else
                    {
                        sqlstr.Clear();
                        sqlstr.AppendFormat(@"  insert into broadcast_member (callno,name,broadcast_index)
                                                values('{0}','{1}','{2}')",
                                                member.callno,member.name,broadcast_index);
                        try
                        {
                            NpgsqlCommand sqlcommand1 = new NpgsqlCommand(sqlstr.ToString(), conn);
                            result = sqlcommand1.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("EditBroadcastMemberList"+e.Message);
                            return false;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EditBroadcastMemberList"+e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 插入键权电话列表
        /// </summary>
        /// <param name="hotlinelist"></param>
        /// <param name="deskindex"></param>
        private bool InsertHotlineList(List<DevStruct> hotlinelist, string deskindex)
        {

            if ((hotlinelist == null) || (hotlinelist.Count == 0))
            {
                Console.WriteLine("插入的键权电话列表不能为空！");
                return false;
            }
            foreach (DevStruct member in hotlinelist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat("insert into desk_totalmem (callno,type,name,level,description,key,desk_index) values ('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                member.callno, member.type, member.name, member.level, member.description, "true", deskindex);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex <= 0)
                    {
                        Console.WriteLine("insert hotlinelist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert Hotlinelist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;

                }
            }
            return true;
        }
        /// <summary>
        /// 插入中继列表
        /// </summary>
        /// <param name="trunklist"></param>
        /// <param name="deskindex"></param>
        /// <returns></returns>
        private bool InsertTrunkList(List<trunkdev> trunklist, string deskindex)
        {

            if ((trunklist == null) || (trunklist.Count == 0))
            {
                Console.WriteLine("插入的中继列表为空！");
                return true;
            }
            foreach (trunkdev member in trunklist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat("insert into desk_trunk (trunkid,name,bindingnumber,desk_index) values ('{0}','{1}','{2}','{3}')",
                member.trunkid, member.name, member.bindingnumber, deskindex);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex <= 0)
                    {
                        Console.WriteLine("insert hotlinelist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert Hotlinelist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;

                }
            }
            return true;
        }
        /// <summary>
        /// 插入调度键盘分组
        /// </summary>
        /// <param name="grouplist"></param>
        /// <param name="deskindex"></param>
        /// <returns></returns>
        private bool InsertDeskGroup(List<Group> grouplist, string deskindex)
        {
            if ((grouplist == null) || (grouplist.Count == 0))
            {
                Console.WriteLine("插入的调度键盘分组不能为空！");
                return false;
            }
            foreach (Group member in grouplist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat("insert into deskgrp (name,col,description,desk_index) values ('{0}','{1}','{2}','{3}') RETURNING index",
                member.groupname, member.column, member.description, deskindex);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    //int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex > 0)
                    {
                        //添加组成员
                        InsertGroupMember(member.memberlist, resultindex.ToString());
                        //Console.WriteLine("resultindex:{0}", resultindex);
                    }
                    else
                    {
                        Console.WriteLine("insert grouplist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert grouplist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 插入分组成员
        /// </summary>
        /// <param name="memberlist"></param>
        /// <param name="groupindex"></param>
        private bool InsertGroupMember(List<DevStruct> memberlist, string groupindex)
        {
            if ((memberlist == null) || (memberlist.Count == 0))
            {
                Console.WriteLine("插入的调度键盘分组不能为空！");
                return false;
            }
            foreach (DevStruct member in memberlist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat("insert into deskgrp_mem (callno,name,type,level,description,desk_grp_index) values ('{0}','{1}','{2}','{3}','{4}','{5}') RETURNING index",
                member.callno, member.name, member.type, member.level, member.description, groupindex);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    //int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex <= 0)
                    {
                        Console.WriteLine("insert grouplist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert grouplist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 插入调度键盘广播按钮
        /// </summary>
        /// <param name="grouplist"></param>
        /// <param name="deskindex"></param>
        /// <returns></returns>
        private bool InsertDeskBroadcast(List<broadcast> broadcastlist, string deskindex)
        {
            if ((broadcastlist == null) || (broadcastlist.Count == 0))
            {
                Console.WriteLine("插入的调度键盘广播按钮为空！");
                return true;
            }
            foreach (broadcast member in broadcastlist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat(@"insert into broadcast (name,desk_index) 
                                values ('{0}','{1}') RETURNING index",
                member.name,deskindex);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    //int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex > 0)
                    {
                        //添加组成员
                        InsertBroadcastMember(member.bmemberlist, resultindex.ToString());
                        //Console.WriteLine("resultindex:{0}", resultindex);
                    }
                    else
                    {
                        Console.WriteLine("insert broadcastlist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert broadcastlist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 插入广播按钮成员
        /// </summary>
        /// <param name="memberlist"></param>
        /// <param name="broadcast_index"></param>
        /// <returns></returns>
        private bool InsertBroadcastMember(List<broadcastmember> memberlist, string broadcast_index)
        {
            if ((memberlist == null) || (memberlist.Count == 0))
            {
                Console.WriteLine("插入的调度键盘分组不能为空！");
                return false;
            }
            foreach (broadcastmember member in memberlist)
            {
                StringBuilder sqlstr = new StringBuilder();
                sqlstr.AppendFormat(@"insert into broadcast_member (callno,name,broadcast_index) values
                                        ('{0}','{1}','{2}') RETURNING index",
                member.callno, member.name, broadcast_index);
                //Console.WriteLine(sqlstr);
                try
                {
                    NpgsqlCommand sqlcommand = new NpgsqlCommand(sqlstr.ToString(), conn);
                    int resultindex = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    //int resultindex = sqlcommand.ExecuteNonQuery();
                    if (resultindex <= 0)
                    {
                        Console.WriteLine("insert grouplist error!!");
                        return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert grouplist数据库异常！" + e.Message);
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取对应控制台的键权电话
        /// </summary>
        /// <param name="deskindex"></param>
        /// <param name="hotlinelist"></param>
        /// <returns></returns>
        public bool GetHotlineList(string deskindex, out List<DevStruct> hotlinelist)
        {
            hotlinelist = new List<DevStruct>();

            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select callno,name,type,level,description from desk_totalmem where desk_index = {0}", deskindex);//获取所有的控制台
            try
            {
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

                        hotlinelist.Add(member);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetHotlineList数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取对应控制台的中继列表
        /// </summary>
        /// <param name="deskindex"></param>
        /// <param name="trunklist"></param>
        /// <returns></returns>
        public bool GetTrunkList(string deskindex, out List<trunkdev> trunklist)
        {
            trunklist = new List<trunkdev>();

            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();

            sqlstr.AppendFormat("select trunkid,name,bindingnumber from desk_trunk where desk_index = {0}", deskindex);
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        trunkdev member = new trunkdev();
                        member.trunkid = row["trunkid"].ToString();
                        member.name = row["name"].ToString();
                        member.bindingnumber = row["bindingnumber"].ToString();

                        trunklist.Add(member);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTrunkList数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取对应键盘的组信息
        /// </summary>
        /// <param name="deskindex"></param>
        /// <param name="grouplist"></param>
        /// <returns></returns>
        public bool GetAllGroup(string deskindex, out List<Group> grouplist)
        {
            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();
            grouplist = new List<Group>();

            sqlstr.AppendFormat("select index,name,col,description from deskgrp where desk_index = {0}", deskindex);//获取所有的控制台
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        Group igroup = new Group();
                        igroup.index = row["index"].ToString();
                        igroup.groupname = row["name"].ToString();
                        igroup.column = row["col"].ToString();
                        igroup.description = row["description"].ToString();

                        GetGroupMember(igroup.index, out igroup.memberlist);

                        grouplist.Add(igroup);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAllKeyboard数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取对应组广播按钮的成员信息
        /// </summary>
        /// <param name="broadcast_index"></param>
        /// <param name="memberlist"></param>
        /// <returns></returns>
        private bool GetBroadcastMember(string broadcast_index, out List<broadcastmember> memberlist)
        {
            memberlist = new List<broadcastmember>();

            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();

            Group igroup = new Group();
            sqlstr.AppendFormat("select callno,name from broadcast_member where broadcast_index = {0}", broadcast_index);//获取所有的控制台
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        broadcastmember member = new broadcastmember();
                        member.callno = row["callno"].ToString();
                        member.name = row["name"].ToString();
                        memberlist.Add(member);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetBroadcastMember数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取对应键盘的组信息
        /// </summary>
        /// <param name="deskindex"></param>
        /// <param name="grouplist"></param>
        /// <returns></returns>
        public bool GetAllBroadcast(string deskindex, out List<broadcast> broadcastlist)
        {
            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();
            broadcastlist = new List<broadcast>();

            sqlstr.AppendFormat("select index,name from broadcast where desk_index = {0}", deskindex);//获取所有的控制台
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        broadcast igroup = new broadcast();
                        igroup.index = row["index"].ToString();
                        igroup.name = row["name"].ToString();
                        
                        GetBroadcastMember(igroup.index, out igroup.bmemberlist);

                        broadcastlist.Add(igroup);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAllKeyboard数据库异常！" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取所有键盘信息
        /// </summary>
        /// <param name="respond"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool GetAllKeyboard(out GetAllKeyboardsp respond)
        {
            DataSet ds = new DataSet();
            StringBuilder sqlstr = new StringBuilder();
            respond = new GetAllKeyboardsp();
            respond.keyboardlist = new List<Keyboard>();

            sqlstr.AppendFormat("select index,name,mac,ip from desk");//获取所有的控制台
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr.ToString(), this.conn))
                {
                    sqldap.Fill(ds);
                    DataTable tbl = ds.Tables[0];//获取第一张表

                    foreach (DataRow row in tbl.Rows)
                    {
                        Keyboard ikeyboard = new Keyboard();
                        ikeyboard.index = row["index"].ToString();
                        ikeyboard.name = row["name"].ToString();
                        ikeyboard.mac = row["mac"].ToString();
                        ikeyboard.ip = row["ip"].ToString();
                        ///添加组
                        List<Group> grouplist;// = new List<Group>();
                        if (!GetAllGroup(ikeyboard.index, out grouplist))
                        {
                            return false;
                        }
                        ikeyboard.grouplist = new List<Group>();
                        ikeyboard.grouplist.AddRange(grouplist);
                        respond.keyboardlist.Add(ikeyboard);
                        //添加键权电话列表
                        List<DevStruct> hotlinelist;
                        if (!GetHotlineList(ikeyboard.index, out hotlinelist))
                        {
                            return false;
                        }
                        ikeyboard.hotlinelist = new List<DevStruct>();
                        ikeyboard.hotlinelist.AddRange(hotlinelist);
                        //添加广播按钮列表
                        List<broadcast> broadcastlist;
                        if(!GetAllBroadcast(ikeyboard.index,out broadcastlist))
                        {
                            return false;
                        }
                        ikeyboard.broadcastlist = new List<broadcast>();
                        ikeyboard.broadcastlist.AddRange(broadcastlist);

                        //添加中继按键列表
                        List<trunkdev> trunklist;
                        trunklist = new List<trunkdev>();
                        if (!GetTrunkList(ikeyboard.index,out trunklist))
                        {
                            return false;
                        }
                        ikeyboard.trunklist = new List<trunkdev>();
                        ikeyboard.trunklist.AddRange(trunklist);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAllKeyboard数据库异常！" + e.Message);
                return false;
            }

        }
        public bool DelKeyboard(string index, out string reason)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat("delete from desk where index = {0}", index);
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
                    reason = "DelKeyboard失败！";
                    return false;
                }

            }
            catch (Exception e)
            {
                reason = "DelKeyboard数据库删除异常！";
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}