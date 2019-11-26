using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using Config;
using System.Data;
using MvcAdapter;
using Base.Logic;
using Base.Logic.BusinessFacade;

namespace Base.Areas.Auth.Controllers
{
    public class AuthorizeController : BaseController
    {
        #region 展示

        public JsonResult GetOrgTree()
        {
            var result = entities.Set<S_A_Org>().Where(c => c.FullID.StartsWith(Config.Constant.OrgRootID) && c.IsDeleted != "1").OrderBy("SortIndex", true);
            return Json(result);
        }

        public JsonResult GetRoleList(QueryBuilder qb)
        {
            var result = entities.Set<S_A_Role>().WhereToGridData(qb);
            return Json(result);
        }

        public JsonResult GetUserList(QueryBuilder qb)
        {
            string nodeFullID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(nodeFullID))
            {
                var result = entities.Set<S_A_User>().Where(c => c.IsDeleted != "1").WhereToGridData(qb);
                return Json(result);
            }

            string getUserListSQL = "select b.* from(select * from S_A__OrgUser where S_A__OrgUser.OrgID='{0}') a left join S_A_User b on a.UserID=b.ID And b.IsDeleted<>'1' order by SortIndex";
            var nodeID = nodeFullID.Split('.').Last();
            string sql = string.Format(getUserListSQL, nodeID);
            SQLHelper sqlHelp = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = sqlHelp.ExecuteGridData(sql, qb);
            return Json(data);
        }

        #region 组织授权展示

        public JsonResult GetResTreeByOrgID(string orgID)
        {
            DataTable dt = GetOrgAuthTree(Config.Constant.MenuRooID, orgID);
            return Json(dt);
        }

        public JsonResult GetRuleTreeByOrgID(string orgID)
        {
            DataTable dt = GetOrgAuthTree(Config.Constant.RuleRootID, orgID);
            return Json(dt);
        }

        private DataTable GetOrgAuthTree(string rootID, string orgID)
        {
            string sql = @"
select a.*,Checked=case when ResID is null then 'false' else 'true' end
from (select * from S_A_Res where FullID like '{0}%' {2}) a
left join 
(select ResID from S_A_Res join S_A__OrgRes on FullID like '{0}%' and ResID=ID and OrgID='{1}') b
on a.ID=b.ResID order by SortIndex
";
            sql = string.Format(sql, rootID, orgID, Auth_PowerDiscrete);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return dt;
        }

        #endregion

        #region 角色授权展示

        public JsonResult GetResTreeByRoleID(string roleID)
        {
            DataTable dt = GetRuleAuthTree(Config.Constant.MenuRooID, roleID);
            return Json(dt);
        }

        public JsonResult GetRuleTreeByRoleID(string roleID)
        {
            DataTable dt = GetRuleAuthTree(Config.Constant.RuleRootID, roleID);
            return Json(dt);
        }

        private DataTable GetRuleAuthTree(string rootID, string roleID)
        {
            string sql = @"
select a.*,Checked=case when ResID is null then 'false' else 'true' end
from (select * from S_A_Res where FullID like '{0}%' {2}) a
left join 
(select ResID from S_A_Res join S_A__RoleRes on FullID like '{0}%' and ResID=ID and RoleID='{1}') b
on a.ID=b.ResID order by SortIndex
";
            sql = string.Format(sql, rootID, roleID, Auth_PowerDiscrete);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return dt;
        }

        #endregion

        #region 用户授权展示

        public JsonResult GetResTreeByUserID(string userID)
        {
            DataTable dt = GetUserAuthTree(Config.Constant.MenuRooID, userID);


            return Json(dt);
        }

        public JsonResult GetRuleTreeByUserID(string userID)
        {
            DataTable dt = GetUserAuthTree(Config.Constant.RuleRootID, userID);
            return Json(dt);
        }

        private DataTable GetUserAuthTree(string rootID, string userID)
        {
            string getUserResTreeSql = @"
select a.*
,b.Src
,DenyAuth=case when c.DenyAuth is null then 'NULL' else c.DenyAuth end
 from( select * from S_A_Res where  S_A_Res.FullID like '{0}%' {2}) a
left join (
select ID,Src=MAX(Src) from (
--组织权限
select S_A_Res.ID,Src=S_A_Org.Name+'(组织)' from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A_Res.FullID like '{0}%' and UserID='{1}' 
union
--系统角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
join S_A_Role on S_A_Role.ID=S_A__RoleUser.RoleID
where S_A_Res.FullID like '{0}%' and UserID='{1}'
union
--继承自组织的角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where S_A_Res.FullID like '{0}%' and UserID='{1}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--组织角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__OrgRoleUser
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRoleUser.RoleID
join S_A_Res on S_A_Res.ID=S_A__RoleRes.ResID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRoleUser.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
where S_A_Res.FullID like '{0}%' and UserID='{1}' and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--用户权限
select S_A_Res.ID,Src=S_A_User.Name+'(用户)' from S_A__UserRes
join S_A_Res on S_A_Res.ID=ResID
join S_A_User on S_A_User.ID=S_A__UserRes.UserID
where S_A_Res.FullID like '{0}%' and UserID='{1}' 

) dt1 group by ID

) b on a.ID=b.ID

left join (select ResID,DenyAuth from S_A__UserRes join S_A_Res on S_A_Res.FullID like '{0}%' and S_A_Res.ID=ResID where UserID='{1}' ) c on c.ResID=a.ID
order by a.SortIndex
";
            string sql = string.Format(getUserResTreeSql
                , rootID
                , userID
                , Auth_PowerDiscrete
                );

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                if (row["Src"].ToString() != "")
                    row["Name"] = string.Format("<span>{0}<a href='#' style='text-decoration: underline;color:blue;' onclick='ViewDetail(\"{2}\",\"{3}\");'>√</a></span>", row["Name"], row["Src"], userID, row["ID"]);

                string denyAuth = row["DenyAuth"].ToString();
                if (denyAuth == "NULL")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/unchecked.gif' id = " + "'" + row["ID"] + "'" + " />" + row["Name"].ToString();
                else if (denyAuth == "0")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/checked.gif' id = " + row["ID"] + " />" + row["Name"].ToString();
                else if (denyAuth == "1")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/DenyAuth.gif' id = " + row["ID"] + " />" + row["Name"].ToString();


            }

            return dt;
        }

        public JsonResult GetUserAuthDetail(string userID, string resID)
        {
            string sql = @"
--组织权限
select S_A_Res.ID,Src=S_A_Org.Name+'(组织)' from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A_Res.ID = '{0}' and UserID='{1}' 
union
--系统角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
join S_A_Role on S_A_Role.ID=S_A__RoleUser.RoleID
where S_A_Res.ID = '{0}' and UserID='{1}'
union
--继承自组织的角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--组织角色权限
select S_A_Res.ID,Src=S_A_Role.Name+'(角色)' from S_A__OrgRoleUser
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRoleUser.RoleID
join S_A_Res on S_A_Res.ID=S_A__RoleRes.ResID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRoleUser.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--用户权限
select S_A_Res.ID,Src=S_A_User.Name+'(用户)' from S_A__UserRes
join S_A_Res on S_A_Res.ID=ResID
join S_A_User on S_A_User.ID=S_A__UserRes.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' 
";
            sql = string.Format(sql, resID, userID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }

        #endregion

        #endregion

        #region 授权

        public JsonResult SaveOrgRes(string orgID, string resIDs)
        {
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));

            foreach (string item in resIDs.Split(','))
            {
                if (item == "") continue;
                S_A__OrgRes orgRes = new S_A__OrgRes();
                orgRes.OrgID = orgID;
                orgRes.ResID = item;
                entities.Set<S_A__OrgRes>().Add(orgRes);
            }
            //记录安全审计日志
            string orgName=entities.Set<S_A_Org>().SingleOrDefault(c=>c.ID==orgID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => resIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("组织授权（菜单）", orgName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveOrgRule(string orgID, string ruleIDs)
        {
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));

            foreach (string item in ruleIDs.Split(','))
            {
                if (item == "") continue;
                S_A__OrgRes orgRes = new S_A__OrgRes();
                orgRes.OrgID = orgID;
                orgRes.ResID = item;
                entities.Set<S_A__OrgRes>().Add(orgRes);
            }
            //记录安全审计日志
            string orgName = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == orgID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => ruleIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("组织授权（对象）", orgName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRoleRes(string roleID, string resIDs)
        {
            entities.Set<S_A__RoleRes>().Delete(c => c.RoleID == roleID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));

            foreach (string item in resIDs.Split(','))
            {
                if (item == "") continue;
                S_A__RoleRes roleRes = new S_A__RoleRes();
                roleRes.RoleID = roleID;
                roleRes.ResID = item;
                entities.Set<S_A__RoleRes>().Add(roleRes);
            }
            //记录安全审计日志
            string roleName = entities.Set<S_A_Role>().SingleOrDefault(c => c.ID == roleID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => resIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("角色授权（菜单）", roleName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRoleRule(string roleID, string ruleIDs)
        {
            entities.Set<S_A__RoleRes>().Delete(c => c.RoleID == roleID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));

            foreach (string item in ruleIDs.Split(','))
            {
                if (item == "") continue;
                S_A__RoleRes roleRes = new S_A__RoleRes();
                roleRes.RoleID = roleID;
                roleRes.ResID = item;
                entities.Set<S_A__RoleRes>().Add(roleRes);
            }

            //记录安全审计日志
            string roleName = entities.Set<S_A_Role>().SingleOrDefault(c => c.ID == roleID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => ruleIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("角色授权（对象）", roleName, resNames);

            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveUserRes(string userID, string checkedIDs, string denyAuthIDs)
        {
            entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));

            checkedIDs = checkedIDs.Trim('"');
            denyAuthIDs = denyAuthIDs.Trim('"');

            foreach (string item in checkedIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "0";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            foreach (string item in denyAuthIDs.Split(','))
            {
                if ("" == item) continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "1";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            //记录安全审计日志
            string UserName = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => checkedIDs.Contains(c.ID)).Select(c => c.Name));
            string denyNames = string.Join(",", entities.Set<S_A_Res>().Where(c => denyAuthIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("用户授权（菜单）", UserName, resNames);
            AuthFO.Log("用户授权（菜单-否定）", UserName, denyNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveUserRule(string userID, string checkedIDs, string denyAuthIDs)
        {
            entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));

            checkedIDs = checkedIDs.Trim('"');
            denyAuthIDs = denyAuthIDs.Trim('"');

            foreach (string item in checkedIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "0";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            foreach (string item in denyAuthIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "1";
                entities.Set<S_A__UserRes>().Add(userRes);
            }

            //记录安全审计日志
            string UserName = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => checkedIDs.Contains(c.ID)).Select(c => c.Name));
            string denyNames = string.Join(",", entities.Set<S_A_Res>().Where(c => denyAuthIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("用户授权（对象）", UserName, resNames);
            AuthFO.Log("用户授权（对象-否定）", UserName, denyNames);

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 查询

        #region 按人员

        public JsonResult GetResTreeView(string userID)
        {
            return GetResTreeByUserID(userID);
        }

        public JsonResult GetRuleTreeView(string userID)
        {
            return GetRuleTreeByUserID(userID);
        }

        #endregion

        #region 按菜单入口

        public JsonResult GetResTree()
        {
            var result = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(Config.Constant.MenuRooID));
            return Json(result);
        }

        public JsonResult GetRuleTree()
        {
            var result = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(Config.Constant.RuleRootID));
            return Json(result);
        }


        public JsonResult GetResUserList(string resID, QueryBuilder qb)
        {
            string sql = @"
select * from (
--直接授权给用户的
select S_A_User.* from S_A__UserRes
join S_A_User on S_A_User.ID=UserID
where ResID='{0}'
--系统角色的
union
select S_A_User.* from S_A__RoleRes
join S_A__RoleUser on S_A__RoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_User on S_A_User.ID=S_A__RoleUser.UserID
where S_A__RoleRes.ResID='{0}'
--组织的
union
select S_A_User.* from S_A__OrgRes
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRes.OrgID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where S_A__OrgRes.ResID='{0}'
--继承自组织的组织角色的
union
select S_A_User.* from S_A__RoleRes
join S_A__OrgRole on S_A__OrgRole.RoleID=S_A__RoleRes.RoleID
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A__RoleRes.ResID='{0}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%' + S_A_User.PrjID+'%')
--组织角色权限
union
select S_A_User.* from S_A__RoleRes
join S_A__OrgRoleUser on S_A__OrgRoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
where S_A__RoleRes.ResID='{0}' and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%' + S_A_User.PrjID+'%')
) table1

left join S_A__UserRes on table1.ID=S_A__UserRes.UserID and S_A__UserRes.ResID='{0}'
where S_A__UserRes.DenyAuth is null or S_A__UserRes.DenyAuth='0'
";

            sql = string.Format(sql, resID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var result = sqlHelper.ExecuteGridData(sql, qb);
            return Json(result);
        }


        #endregion

        #endregion


        private string _Auth_PowerDiscrete = "";
        private string Auth_PowerDiscrete
        {
            get
            {
                if (_Auth_PowerDiscrete == "")
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
                    {
                        _Auth_PowerDiscrete = string.Format(" and S_A_Res.FullID not like '{0}%'", Config.Constant.SystemMenuFullID);
                    }
                    else
                    {
                        _Auth_PowerDiscrete = " ";
                    }
                    string userID = FormulaHelper.UserID;
                    var entity = entities.Set<S_A_AuthLevel>().SingleOrDefault(c => c.UserID == userID);
                    if (entity != null) //通过分级授权获得权限            
                    {
                        string str = "";
                        if (!string.IsNullOrEmpty(entity.MenuRootFullID))
                            foreach (var item in entity.MenuRootFullID.Split(','))
                                str += string.Format(" or FullID like '{0}%' or '{0}' like FullID +'%'", item);
                        if (!string.IsNullOrEmpty(entity.RuleRootFullID))
                            foreach (var item in entity.RuleRootFullID.Split(','))
                                str += string.Format(" or FullID like '{0}%' or '{0}' like FullID +'%'", item);
                        if (str.Length > 3)
                            str = str.Substring(3);
                        else
                            str = "1=2";
                        _Auth_PowerDiscrete += string.Format(" and ({0})", str);
                    }
                }
                return _Auth_PowerDiscrete;
            }
        }
    }
}
