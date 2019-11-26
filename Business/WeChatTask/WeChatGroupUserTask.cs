using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Quartz;
using WeChatTask.Common;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.AdvancedAPIs.Groups;
using Senparc.Weixin.MP.AdvancedAPIs.User;

namespace WeChatTask
{
    public class WeChatGroupUserTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log4NetConfig.Configure();
            try
            {
                var mpaccounts = StaticObjects.wechatdh.ExecuteTable("select ID,Name from MpAccount where IsDelete=0 and AutoSyncUser=1").AsEnumerable().Select(c => new 
                {
                    ID = DbTool.ToString(c["ID"]),
                    Name = DbTool.ToString(c["Name"]),
                });
                foreach (var mp in mpaccounts)
                {
                    try
                    {
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的分组开始", mp.Name, mp.ID));
                        RefreshGroup(mp.ID);
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的分组结束", mp.Name, mp.ID));
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的用户开始", mp.Name, mp.ID));
                        ReFreshFans(mp.ID);
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的用户结束", mp.Name, mp.ID));
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error(ex, string.Format("同步公众号{0}[{1}]的分组和用户出现错误", mp.Name, mp.ID));
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, "同步微信分组和用户出现错误");
                throw ex;
            }
        }

        public void test()
        {
            Log4NetConfig.Configure();
            try
            {
                var mpaccounts = StaticObjects.wechatdh.ExecuteTable("select ID,Name from MpAccount where IsDelete=0 and isnull(AutoSyncUser,0)=1").AsEnumerable().Select(c => new
                {
                    ID = DbTool.ToString(c["ID"]),
                    Name = DbTool.ToString(c["Name"]),
                });
                foreach (var mp in mpaccounts)
                {
                    try
                    {
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的分组开始", mp.Name, mp.ID));
                        RefreshGroup(mp.ID);
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的分组结束", mp.Name, mp.ID));
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的用户开始", mp.Name, mp.ID));
                        ReFreshFans(mp.ID);
                        LogWriter.Info(string.Format("同步公众号{0}[{1}]的用户结束", mp.Name, mp.ID));
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error(ex, string.Format("同步公众号{0}[{1}]的分组和用户出现错误", mp.Name, mp.ID));
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, "同步微信分组和用户出现错误");
                throw ex;
            }
        }

        /// <summary>
        /// 更新微信分组
        /// </summary>
        /// <param name="MpID"></param>
        private void RefreshGroup(string MpID)
        {
            #region 初始化
            StringBuilder sb = new StringBuilder();
            string groupinsertstr = @"
insert into MpGroup (ID,MpID,WxGroupID,Name,FansCount,ParentID,FullPath,Length,ChildCount) values('{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},{8})";
            string groupupdatestr = @"
update MpGroup set Name='{0}',FansCount={1} where ID='{2}'";
            var dt = StaticObjects.wechatdh.ExecuteTable(string.Format("select ID,WxGroupID,Length from MpGroup where MpID='{0}'", MpID)).AsEnumerable()
                .Select(c => new
                {
                    ID = DbTool.ToString(c["ID"]),
                    WxGroupID = DbTool.ToNullInt(c["WxGroupID"]),
                    Length = DbTool.ToInt(c["Length"]),
                });
            GroupsJson result = null;
            try
            {
                result = GroupsApi.Get(WxApi.GetAccessToken(MpID));
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("获取MpID为{0}的分组报错", MpID));
                result = GroupsApi.Get(WxApi.GetAccessToken(MpID, true));
            }
            #endregion

            #region 处理根节点
            var rootgroup = dt.Where(c => c.Length == 1).FirstOrDefault();
            string rootid;
            if (rootgroup == null)
            {
                rootid = DbTool.CreateGuid();
                sb.AppendFormat(groupinsertstr, rootid, MpID, "null", "全部", result.groups.Sum(c => c.count), -1, rootid, 1, result.groups.Count());
            }
            else
            {
                rootid = rootgroup.ID;
            }
            #endregion

            #region 保存最新的节点
            var oldgroups = dt.Where(c => c.Length == 2).ToList();
            var notgroupid = "";
            foreach (var wxgroup in result.groups)
            {
                var g = oldgroups.Where(c => c.WxGroupID == wxgroup.id).FirstOrDefault();
                if (g == null)
                {
                    var id = DbTool.CreateGuid();
                    sb.AppendFormat(groupinsertstr, id, MpID, wxgroup.id, wxgroup.name, wxgroup.count, rootid, string.Format("{0}.{1}", rootid, id), 2, 0);
                    if (wxgroup.name == "未分组")
                        notgroupid = g.ID;
                }
                else
                {
                    sb.AppendFormat(groupupdatestr, wxgroup.name, wxgroup.count, g.ID);
                    if (wxgroup.name == "未分组")
                        notgroupid = g.ID;
                }
            }
            #endregion

            #region 剔除老的节点
            var deletegroups = oldgroups.Where(c => !result.groups.Select(d => d.id).Contains(c.WxGroupID.Value)).ToList();
            if (notgroupid == "")
                throw new Exception("找不到未分组节点");
            sb.AppendFormat(@"
update MpFans set groupid='{0}' from MpFans a inner join MpGroup b on a.GroupID=b.ID where b.MpID='{1}' and a.MpID='{1}' and b.ID in ('{2}')
delete MpGroup where MpID='{1}' and ID in ('{2}')"
                , notgroupid, MpID, string.Join("','", deletegroups.Select(c => c.ID)));
            #endregion

            StaticObjects.wechatdh.ExecuteNonQuery(sb.ToString(), 3600);
        }

        /// <summary>
        /// 更新微信用户
        /// </summary>
        /// <param name="MpID"></param>
        private void ReFreshFans(string MpID)
        {
            #region 初始化
            var sb = new StringBuilder();
            int stoptick = 0, stopcount = 100; ;
            var groupdt = StaticObjects.wechatdh.ExecuteTable(string.Format("select ID,WxGroupID,Length from MpGroup where MpID='{0}'", MpID)).AsEnumerable()
                .Select(c => new
                {
                    ID = DbTool.ToString(c["ID"]),
                    WxGroupID = DbTool.ToNullInt(c["WxGroupID"]),
                    Length = DbTool.ToInt(c["Length"]),
                });
            var fanssavestr = @"
if not exists(select * from MpFans where MpID='{1}' and OpenID='{2}')
insert into MpFans ([ID],[MpID],[OpenID],[NickName],[Sex],[Language],[City],[Province],[Country],[HeadImgUrl],[SubscribeTime],[UniionID],[Remark],[WxGroupID],[GroupID],[IsFans],[UpdateTime]) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}',getdate())
else
update MpFans SET [MpID]='{1}',[OpenID]='{2}',[NickName]='{3}',[Sex]='{4}',[Language]='{5}',[City]='{6}',[Province]='{7}',[Country]='{8}',[HeadImgUrl]='{9}',[SubscribeTime]='{10}',[UniionID]='{11}',[Remark]='{12}',[WxGroupID]='{13}',[GroupID]='{14}',[IsFans]='{15}',[UpdateTime]=getdate() WHERE MpID='{1}' and OpenID='{2}'";
            #endregion

            #region 更新用户
            #region 更新前10000个用户
            OpenIdResultJson fansopenids = null;
            try
            {
                fansopenids = UserApi.Get(WxApi.GetAccessToken(MpID), "");
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("获取MpID为{0}的openid报错", MpID));
                fansopenids = UserApi.Get(WxApi.GetAccessToken(MpID, true), "");
            }
            DateTime now = DateTime.Now;
            if (fansopenids.count > 0)
            {
                foreach (var id in fansopenids.data.openid)
                {
                    UserInfoJson wxinfo = null;
                    try
                    {
                        wxinfo = UserApi.Info(WxApi.GetAccessToken(MpID), id);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error(ex, string.Format("获取MpID为{0}，openid为{1}的用户信息报错", MpID, id));
                        wxinfo = UserApi.Info(WxApi.GetAccessToken(MpID, true), id);
                    }
                    var group = groupdt.Where(c => c.WxGroupID == wxinfo.groupid).FirstOrDefault();
                    if (group == null)
                        continue;
                    sb.AppendFormat(fanssavestr, DbTool.CreateGuid(), MpID, id,
                        DbTool.ToSqlParamString(wxinfo.nickname), wxinfo.sex, DbTool.ToSqlParamString(wxinfo.language),
                        DbTool.ToSqlParamString(wxinfo.city), DbTool.ToSqlParamString(wxinfo.province),
                        DbTool.ToSqlParamString(wxinfo.country), DbTool.ToSqlParamString(wxinfo.headimgurl),
                        DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time), DbTool.ToSqlParamString(wxinfo.unionid),
                        DbTool.ToSqlParamString(wxinfo.remark), wxinfo.groupid, group.ID, 1);
                    stoptick++;
                    if (stoptick % stopcount == 0)
                    {
                        StaticObjects.wechatdh.ExecuteNonQuery(sb.ToString(), 36000);
                        sb.Clear();
                    }
                }
            }
            #endregion

            #region while循环更新后续所有用户
            while (!string.IsNullOrEmpty(fansopenids.next_openid))
            {
                try
                {
                    fansopenids = UserApi.Get(WxApi.GetAccessToken(MpID), fansopenids.next_openid);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("获取MpID为{0}的openid报错，nextopenid为{1}", MpID, fansopenids.next_openid));
                    fansopenids = UserApi.Get(WxApi.GetAccessToken(MpID, true), fansopenids.next_openid);
                }
                if (fansopenids.count > 0)
                {
                    foreach (var id in fansopenids.data.openid)
                    {
                        UserInfoJson wxinfo = null;
                        try
                        {
                            wxinfo = UserApi.Info(WxApi.GetAccessToken(MpID), id);
                        }
                        catch (Exception ex)
                        {
                            LogWriter.Error(ex, string.Format("获取MpID为{0}，openid为{1}的用户信息报错", MpID, id));
                            wxinfo = UserApi.Info(WxApi.GetAccessToken(MpID, true), id);
                        }
                        var group = groupdt.Where(c => c.WxGroupID == wxinfo.groupid).FirstOrDefault();
                        if (group == null)
                            continue;
                        sb.AppendFormat(fanssavestr, DbTool.CreateGuid(), MpID, id,
                            DbTool.ToSqlParamString(wxinfo.nickname), wxinfo.sex, DbTool.ToSqlParamString(wxinfo.language),
                            DbTool.ToSqlParamString(wxinfo.city), DbTool.ToSqlParamString(wxinfo.province),
                            DbTool.ToSqlParamString(wxinfo.country), DbTool.ToSqlParamString(wxinfo.headimgurl),
                            DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time), DbTool.ToSqlParamString(wxinfo.unionid),
                            DbTool.ToSqlParamString(wxinfo.remark), wxinfo.groupid, group.ID, 1);
                        stoptick++;
                        if (stoptick % stopcount == 0)
                        {
                            StaticObjects.wechatdh.ExecuteNonQuery(sb.ToString(), 36000);
                            sb.Clear();
                        }
                    }
                }
            }
            #endregion

            //没更新到的用户设为取消关注
            sb.AppendFormat("update MpFans set IsFans='0' where MpID='{0}' and UpdateTime<'{1}'", MpID, now.AddDays(-1));
            #endregion

            StaticObjects.wechatdh.ExecuteNonQuery(sb.ToString(), 36000);
            LogWriter.Info(string.Format("更新数量{0},更新完毕", stoptick));
        }
    }
}
