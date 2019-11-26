using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using System.Linq.Expressions;
using System.Reflection;
using Formula.DynConditionObject;
using Config;
using System.Data;
using Formula.Helper;
using Base.Logic.BusinessFacade;

namespace Base.Areas.Auth.Controllers
{
    public class ResRoleController : BaseController<S_A_Res, S_A__RoleRes, S_A_Role>
    {
        public override JsonResult GetTree()
        {
            string userID = Formula.FormulaHelper.UserID;
            string sql = string.Format("select * from S_A_Res where FullID like '{0}%'", Request["RootFullID"]);
            if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
            {
                sql += string.Format(" and FullID not like '{0}%'", Config.Constant.SystemMenuFullID);
            }

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
                sql += string.Format("and ({0})", str);
            }

            sql += " order by SortIndex";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            return Json(dt);
        }

        #region 列表查询

        public override JsonResult GetRelationList(QueryBuilder qb)
        {
            if (string.IsNullOrEmpty(Request["NodeFullID"]))
                return Json("");
            string resID = Request["NodeFullID"].Split('.').Last();

            string sql = "";
            if (Config.Constant.IsOracleDb)
            {
                sql = @"
select * from (
select ID, Name, to_char(FullID) as FullID,Description,'组织' as Type,'Org' as TypeCode from S_A__OrgRes join S_A_Org on OrgID=ID where ResID='{0}'
union
select ID, Name,'' as FullID,Description,'角色' as Type,'Role' as TypeCode from S_A__RoleRes join S_A_Role on RoleID=ID where ResID='{0}' 
union
select ID, Name,'' as FullID,Description,'用户' as Type,'User' as TypeCode from S_A__UserRes join S_A_User on UserID=ID where ResID='{0}'
) table1 order by Type,FullID asc";
            }
            else
            {
                sql = @"
select * from (
select ID, Name, FullID,Description,'组织' as Type,'Org' as TypeCode from S_A__OrgRes join S_A_Org on OrgID=ID where ResID='{0}'
union
select ID, Name,'' as FullID,Description,'角色' as Type,'Role' as TypeCode from S_A__RoleRes join S_A_Role on RoleID=ID where ResID='{0}' 
union
select ID, Name,'' as FullID,Description,'用户' as Type,'User' as TypeCode from S_A__UserRes join S_A_User on UserID=ID where ResID='{0}'
) table1 order by Type,FullID asc";
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            return Json(sqlHelper.ExecuteDataTable(string.Format(sql, resID)));

        }

        public JsonResult GetUserRelationList(QueryBuilder qb)
        {
            if (string.IsNullOrEmpty(Request["NodeFullID"]))
                return Json("");
            string resID = Request["NodeFullID"].Split('.').Last();
            string gridRowID = Request["GridRowID"];

            string sql = @"
--继承自组织的角色
select S_A_User.* from S_A__RoleRes
join S_A__OrgRole on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID
join S_A_User on UserID=ID
join S_A_Org on S_A_Org.ID=S_A__OrgRole.OrgID
where (S_A_Org.FullID like '%'+ S_A_User.DeptID +'%' or S_A_Org.FullID like '%'+ S_A_User.PrjID +'%') and ResID='{0}' {1}
union
--组织角色
select S_A_User.* from S_A__RoleRes
join S_A__OrgRoleUser on S_A__OrgRoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
where (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%' + S_A_User.PrjID+'%') and S_A__RoleRes.ResID='{0}' {1}
union
--系统角色
select S_A_User.* from S_A__RoleRes 
join S_A__RoleUser on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_User on UserID = ID
where ResID='{0}' {2}
union
--组织
select S_A_User.* from S_A__OrgRes 
join S_A__OrgUser on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID
join S_A_User on UserID=ID
where ResID='{0}' {3}
union
--用户
select S_A_User.* from S_A__UserRes
join S_A_User on UserID=ID
where ResID='{0}' {4}
";


            if (string.IsNullOrEmpty(gridRowID))
                sql = string.Format(sql, resID, "", "", "", "");
            else
                sql = string.Format(sql
                    , resID
                    , string.Format("and S_A__RoleRes.RoleID='{0}'", gridRowID)
                    , string.Format("and S_A__RoleRes.RoleID='{0}'", gridRowID)
                    , string.Format("and S_A__OrgRes.OrgID='{0}'", gridRowID)
                    , string.Format("and S_A__UserRes.UserID='{0}'", gridRowID)
                    );


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        #endregion

        #region 授权

        public JsonResult GetRoleRelationAll()
        {
            return base.JsonGetRelationAll<S_A_Res, S_A__RoleRes, S_A_Role>(Request["NodeFullID"]);
        }

        public JsonResult GetOrgRelationAll()
        {
            return base.JsonGetRelationAll<S_A_Res, S_A__OrgRes, S_A_Org>(Request["NodeFullID"]);
        }

        public JsonResult SetRoleRelation()
        {
            //记录安全审计日志
            string resFullID = Request["NodeFullID"];
            string[] roleIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
            string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
            string RoleNames = string.Join(",", entities.Set<S_A_Role>().Where(c => roleIDs.Contains(c.ID)).Select(c => c.Name));
            string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "菜单授权（角色）" : "对象授权（角色）";
            AuthFO.Log(opName, menuName, RoleNames);

            return base.JsonAppendRelation<S_A_Res, S_A__RoleRes, S_A_Role>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public JsonResult SetOrgRelation()
        {
            //记录安全审计日志
            string resFullID = Request["NodeFullID"];
            string[] orgIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
            string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
            string orgNames = string.Join(",", entities.Set<S_A_Org>().Where(c => orgIDs.Contains(c.ID)).Select(c => c.Name));
            string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "菜单授权（组织）" : "对象授权（组织）";
            AuthFO.Log(opName, menuName, orgNames);

            return base.JsonAppendRelation<S_A_Res, S_A__OrgRes, S_A_Org>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public JsonResult SetUserRelation()
        {
            //记录安全审计日志
            string resFullID = Request["NodeFullID"];
            string[] orgIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
            string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
            string userNames = string.Join(",", entities.Set<S_A_User>().Where(c => orgIDs.Contains(c.ID)).Select(c => c.Name));
            string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "菜单授权（用户）" : "对象授权（用户）";
            AuthFO.Log(opName, menuName, userNames);

            return base.JsonAppendRelation<S_A_Res, S_A__UserRes, S_A_User>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public override JsonResult DeleteRelation()
        {
            string relationData = Request["RelationData"];

            List<Dictionary<string, object>> list = JsonHelper.ToObject<List<Dictionary<string, object>>>(relationData);

            //list中只有一条记录
            if (list[0]["TypeCode"].ToString() == "Org")
            {
                //记录安全审计日志
                string resFullID = Request["NodeFullID"];
                string[] orgIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
                string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
                string orgNames = string.Join(",", entities.Set<S_A_Org>().Where(c => orgIDs.Contains(c.ID)).Select(c => c.Name));
                string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "删除菜单授权（组织）" : "删除对象授权（组织）";
                AuthFO.Log(opName, menuName, orgNames);

                return base.JsonDeleteRelation<S_A_Res, S_A__OrgRes, S_A_Org>(Request["NodeFullID"], relationData, "True");
            }
            else if (list[0]["TypeCode"].ToString() == "Role")
            {
                //记录安全审计日志
                string resFullID = Request["NodeFullID"];
                string[] roleIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
                string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
                string RoleNames = string.Join(",", entities.Set<S_A_Role>().Where(c => roleIDs.Contains(c.ID)).Select(c => c.Name));
                string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "删除菜单授权（角色）" : "删除对象授权（角色）";
                AuthFO.Log(opName, menuName, RoleNames);

                return base.JsonDeleteRelation<S_A_Res, S_A__RoleRes, S_A_Role>(Request["NodeFullID"], relationData, "True");
            }
            else
            {
                //记录安全审计日志
                string resFullID = Request["NodeFullID"];
                string[] userIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
                string menuName = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID == resFullID).Name;
                string userNames = string.Join(",", entities.Set<S_A_User>().Where(c => userIDs.Contains(c.ID)).Select(c => c.Name));
                string opName = resFullID.StartsWith(Config.Constant.MenuRooID) ? "删除菜单授权（用户）" : "删除对象授权（用户）";
                AuthFO.Log(opName, menuName, userNames);

                return base.JsonDeleteRelation<S_A_Res, S_A__UserRes, S_A_User>(Request["NodeFullID"], relationData, "True");
            }
        }

        #endregion

    }
}
