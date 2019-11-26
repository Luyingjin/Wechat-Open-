using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Data;
using System.Web;

namespace Config.Logic
{
    public class ResService
    {
        public static List<Res> GetRes(string url, string types)
        {
            string path = GetUrlPath(url);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = string.Format(@"select * from S_A_Res where Type in('{1}') and Url like '{0}%'", path, types.Replace(",", "','"));

            var list = sqlHelper.ExecuteList<Res>(sql);
            list = list.Where(c => url.StartsWith(c.Url, StringComparison.CurrentCultureIgnoreCase)).ToList();
            return list;
        }

        public static List<Res> GetRes(string url, string types, string userID, string orgID, string prjID)
        {
            if (string.IsNullOrEmpty(orgID))
                orgID = "NULL";
            if (string.IsNullOrEmpty(prjID))
                prjID = "NULL";

            string path = GetUrlPath(url);



            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = @"
select * from (
--组织权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A__OrgUser.OrgID=S_A_Org.ID
where url like'{0}%' and UserID='{1}' and S_A_Res.Type in('{2}') 
union
--系统角色权限
select S_A_Res.* from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
where url like'{0}%' and UserID='{1}' and S_A_Res.Type in('{2}') 
union
--继承组织的角色权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRole on  S_A__OrgUser.OrgID=S_A__OrgRole.OrgID 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A__OrgUser.OrgID=S_A_Org.ID
where url like'{0}%' and UserID='{1}' and S_A_Res.Type in('{2}') --and(S_A_Org.FullID like '%{3}%' or S_A_Org.FullID like '%{4}%')
union
--组织角色权限
select S_A_Res.* from S_A__OrgRoleUser
join S_A_Org on  S_A_Org.ID=OrgID
join S_A__RoleRes on S_A__OrgRoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_Res on ResID=S_A_Res.ID
where url like'{0}%' and UserID='{1}' and S_A_Res.Type in('{2}')  --and (S_A_Org.FullID like '%{3}%' or S_A_Org.FullID like '%{4}%') --当前部门条件
union
--用户权限
select S_A_Res.* from S_A__UserRes
join S_A_Res on ResID=ID
where S_A_Res.url like'{0}%' and UserID='{1}' and S_A_Res.Type in('{2}') 
--管理员权限
{6} {8}
) tb  {5} {7}
";

            int isAdmin = IsAdmin(userID);
            int isSepAdmin = IsSepAdmin(userID); //0非分级管理员，1是分级管理员

            sql = string.Format(sql
                , path, userID, types.Replace(",", "','"), orgID, prjID
                //非管理员排除系统管理菜单                    
                , isAdmin == 0 ? string.Format(" where FullID not like '%{0}%'", Config.Constant.SystemMenuFullID) : ""
                //管理员增加系统管理菜单
                , isAdmin == 1 ? string.Format(" union select * from S_A_Res where S_A_Res.url like'{0}%' and (FullID like '{1}%' or '{1}' like FullID + '%')", path, Config.Constant.SystemMenuFullID) : ""
                //分级授权管理员
                , isSepAdmin == 1 && isAdmin == 0 ? string.Format(" or FullID like '{0}%' or '{0}' like FullID +'%'", Config.Constant.AuthrizeMenuFullID) : ""
                //分级授权管理员
                , isSepAdmin == 1 ? string.Format(" union select * from S_A_Res where S_A_Res.url like'{0}%' and FullID like '{1}%' or '{1}' like FullID + '%'", path, Config.Constant.AuthrizeMenuFullID) : ""
                );

            var list = sqlHelper.ExecuteList<Res>(sql);
            list = list.Where(c => url.StartsWith(c.Url, StringComparison.CurrentCultureIgnoreCase)).ToList();

            string denyAuthSql = @"select S_A_Res.* from S_A__UserRes join S_A_Res on S_A_Res.ID=ResID where DenyAuth = '1' and UserID = '{0}'";
            denyAuthSql = string.Format(denyAuthSql, userID);

            var denyAuthList = sqlHelper.ExecuteDataTable(denyAuthSql);

            var rows = denyAuthList.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    if (list[j].ID.ToString() == rows[i]["ID"].ToString())
                    {
                        list.Remove(list[j]);
                        break;
                    }
                }
            }


            return list;
        }

        public static DataTable GetResTree(string rootID, string userID, string orgID, string prjID)
        {
            if (string.IsNullOrEmpty(orgID))
                orgID = "NULL";
            if (string.IsNullOrEmpty(prjID))
                prjID = "NULL";

            int isAdmin = IsAdmin(userID);//是否管理员,-1表示没有开启三权分立，0表示不是管理员，1表示是管理员
            int isSepAdmin = IsSepAdmin(userID); //0非分级管理员，1是分级管理员

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string sql = string.Format(@"
select * from (
--组织权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
where S_A_Res.FullID like '{0}%' and UserID='{1}'
union
--系统角色权限
select S_A_Res.* from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
where S_A_Res.FullID like '{0}%' and UserID='{1}'
union
--继承自组织的角色权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A_Res.FullID like '{0}%' and UserID='{1}'
union
--组织角色权限
select S_A_Res.* from S_A__OrgRoleUser
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRoleUser.RoleID
join S_A_Res on S_A_Res.ID=S_A__RoleRes.ResID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
where S_A_Res.FullID like '{0}%' and UserID='{1}' --and (S_A_Org.FullID like '%{2}%' or S_A_Org.FullID like '%{3}%') --当前部门权限
union
--用户权限
select S_A_Res.* from S_A__UserRes
join S_A_Res on S_A_Res.ID=ResID
where S_A_Res.FullID like '{0}%' and UserID='{1}'
--管理员权限
{5} {7}
) dt1
{4} {6}
order by ParentID, SortIndex "
                , rootID, userID, orgID, prjID
                //非管理员排除系统管理菜单                    
                , isAdmin == 0 ? string.Format(" where FullID not like '{0}%'", Config.Constant.SystemMenuFullID) : ""
                //管理员增加系统管理菜单
                , isAdmin == 1 ? string.Format(" union select * from S_A_Res where FullID like '{0}%' or '{0}' like FullID+'%'", Config.Constant.SystemMenuFullID) : ""
                //分级授权管理员
                , isSepAdmin == 1 && isAdmin == 0 ? string.Format(" or FullID like '{0}%' or '{0}' like FullID +'%'", Config.Constant.AuthrizeMenuFullID) : ""
                //分级授权管理员
                , isSepAdmin == 1 ? string.Format(" union select * from S_A_Res where FullID like '{0}%' or '{0}' like FullID + '%'", Config.Constant.AuthrizeMenuFullID) : ""
                );



            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            string denyAuthSql = @"select S_A_Res.* from S_A__UserRes join S_A_Res on S_A_Res.ID=ResID where DenyAuth = '1' and UserID = '{0}'";
            denyAuthSql = string.Format(denyAuthSql, userID);
            DataTable denyAuthDT = sqlHelper.ExecuteDataTable(denyAuthSql);
            var denyAuthDTRows = denyAuthDT.Rows;
            var dtRows = dt.Rows;
            for (int i = 0; i < denyAuthDTRows.Count; i++)
            {
                for (int j = dtRows.Count - 1; j >= 0; j--)
                {
                    if (dtRows[j]["FullID"].ToString().StartsWith(denyAuthDTRows[i]["FullID"].ToString()))
                    {
                        dtRows.Remove(dt.Rows[j]);
                    }
                }
            }

            return dt;
        }

        #region 私有方法

        private static string GetUrlPath(string url)
        {
            var arr = url.Split('?');
            string path = arr[0];
            if (arr.Length == 2)
            {
                if (arr[1].StartsWith("TmplCode="))   //TmplCode为自定义列表参数
                    path = path + "?" + arr[1].Split('&')[0];
                else if(arr[1].StartsWith("ReportCode="))
                    path = path + "?" + arr[1].Split('&')[0];
            }

            return path;
        }

        /// <summary>
        /// -1表示没有启用三权分立，0表示不是管理员，1表示是管理员
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int IsAdmin(string userID)
        {
            //如果启用三权分立
            int result = -1;

            string key = "IsAdmin_" + userID;
            object obj = HttpRuntime.Cache.Get(key);
            if (obj == null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
                {
                    result = 0;
                    DataTable dtSecurity = sqlHelper.ExecuteDataTable("select * from S_A_Security");
                    if (dtSecurity.Rows.Count == 1)
                    {
                        if (dtSecurity.Rows[0]["AdminIDs"].ToString().Contains(userID))
                            result = 1;
                    }
                }
                obj = result;
                HttpRuntime.Cache[key] = obj;
            }

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 0表示非分级授权管理员1，表示是分级授权管理员
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int IsSepAdmin(string userID)
        {
            int result = 0;

            string key = "IsSepAdmin_" + userID;

            object obj = HttpRuntime.Cache.Get(key);
            if (obj == null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_A_AuthLevel where UserID like '%{0}%'", userID));
                if (dt.Rows.Count > 0)
                    result = 1;
                obj = result;
                HttpRuntime.Cache[key] = obj;
            }

            return Convert.ToInt32(obj);
        }

        #endregion

    }


}
