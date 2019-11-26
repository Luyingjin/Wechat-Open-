using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Base.Logic.BusinessFacade;
using System.Web.Security;
using Config;
using System.Data;

namespace Base.Areas.Auth.Controllers
{
    public class OrgUserController : BaseController<S_A_Org, S_A__OrgUser, S_A_User>
    {
        #region 重载 保存关联数据（用户）

        public override JsonResult SaveRelationData()
        {
            var user = UpdateEntity<S_A_User>();
            string nodeFullID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(user.DeptID) && !string.IsNullOrEmpty(nodeFullID))
            {
                string[] orgIDs = nodeFullID.Split('.');
                var orgs = entities.Set<S_A_Org>().Where(c => orgIDs.Contains(c.ID));
                foreach (var org in orgs)
                {
                    if (org.Type == "Dept")
                    {
                        user.DeptID = org.ID;
                        user.DeptFullID = org.FullID;
                        user.DeptName = org.Name;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Code.ToLower(), "SHA1");
            }

            return base.JsonSaveRelationData<S_A_Org, S_A__OrgUser, S_A_User>(user);
        }

        #endregion

        #region 重载 获取关联数据列表（用户）

        public override JsonResult GetRelationList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string nodeFullID = Request["NodeFullID"];
            string nodeID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(nodeID))
                return Json("");
            nodeID = nodeID.Split('.').Last();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_A_User join S_A__OrgUser on ID=UserID where OrgID='{0}'", nodeID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);

            string userIDs = string.Join("','", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());

            //部门名称
            sql = "select UserID,Name as OrgName from S_A__OrgUser join S_A_Org on S_A_Org.ID=OrgID where UserID in('{0}') and S_A_Org.Type like '%Dept'";
            sql = string.Format(sql, userIDs);
            var dtOrgName = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            //系统角色名称
            sql = "select UserID,Name as RoleName from S_A__RoleUser join S_A_Role on ID=RoleID where UserID in('{0}')";
            sql = string.Format(sql, userIDs);
            var dtRoleName = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            //组织角色名称
            sql = @"
select  a.UserID
,case when sum(Src)>2 then '★' when sum(Src)=2 then '☆'  else '' end +a.RoleName+'('+MAX(OrgName)+')' as RoleName 
from
(
select UserID,S_A_Role.Name as RoleName,2 as Src,S_A_Org.Name as OrgName from S_A__OrgRole
join S_A__OrgUser on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A_Org on S_A_Org.ID=S_A__OrgRole.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
where UserID in('{0}')
union
select UserID, S_A_Role.Name as RoleName,1 as Src,S_A_Org.Name as OrgName from S_A__OrgRoleUser
join S_A_Org on S_A_Org.ID=OrgID
join S_A_Role on S_A_Role.ID=RoleID
where UserID in('{0}')
) a 
group by a.UserID, a.RoleName,a.OrgName
";

            sql = string.Format(sql, userIDs);
            var dtOrgRole = sqlHelper.ExecuteDataTable(sql).AsEnumerable();
            
            dt.Columns.Add("DeptNames");
            dt.Columns.Add("UserRoleName");
            dt.Columns.Add("OrgRoleName");

            foreach (DataRow row in dt.Rows)
            {
                string userID = row["ID"].ToString();
                row["DeptNames"] = string.Join(",", dtOrgName.Where(c => c["UserID"].ToString() == userID).Select(c => c["OrgName"]).ToArray());
                row["UserRoleName"] = string.Join(",", dtRoleName.Where(c => c["UserID"].ToString() == userID).Select(c => c["RoleName"]).ToArray());
                row["OrgRoleName"] = string.Join(",", dtOrgRole.Where(c => c["UserID"].ToString() == userID).Select(c => c["RoleName"]).ToArray());
            }

            GridData gridData = new GridData(dt);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        #endregion

        #region 重载 删除关联（用户）

        public override JsonResult DeleteRelation()
        {
            #region 设置当前部门

            string nodeFullID = Request["NodeFullID"];
            string relationData = Request["RelationData"];
            var userids = GetValues(relationData, "ID");
            var users = entities.Set<S_A_User>().Where(c => userids.Contains(c.ID)).ToArray();
            foreach (var user in users)
            {
                if (user.DeptFullID != null && user.DeptFullID.StartsWith(nodeFullID))
                {
                    user.DeptID = "";
                    user.DeptName = "";
                    user.DeptFullID = "";
                }
            }
            #endregion

            return base.DeleteRelation();
        }

        #endregion

        #region 作废部门

        public ActionResult AbortOrg(string fullID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.AbortOrg(fullID);
            return Json("");
        }
        #endregion

        #region 恢复部门
        public ActionResult RecoverOrg(string nodeID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.RecoverOrg(nodeID);
            return Json("");
        }

        #endregion

        #region 删除部门

        public override JsonResult DeleteNode()
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.DeleteOrg(Request["FullID"]);
            return Json("");
        }

        #endregion

        #region 组织的角色

        public JsonResult GetOrgRole(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_Org, S_A__OrgRole, S_A_Role>(nodeFullID);
        }

        public JsonResult SetOrgRole(string nodeFullID, string relationData, string fullRelation)
        {
            return base.JsonSetRelation<S_A_Org, S_A__OrgRole, S_A_Role>(nodeFullID, relationData, fullRelation);
        }

        #endregion

        #region 组织的权限

        public JsonResult GetOrgList(string nodeFullID)
        {
            return Json(entities.Set<S_A_Org>().Where(c => c.FullID.StartsWith(nodeFullID)));
        }

        public JsonResult GetOrgRes(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID);
        }

        public JsonResult SetOrgRes(string nodeFullID, string relationData, string fullRelation)
        {
            string nodeID = nodeFullID.Split('.').Last();
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == nodeID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            entities.SaveChanges();
            return base.JsonAppendRelation<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID, relationData, "False");
        }

        public JsonResult SetOrgRule(string nodeFullID, string relationData, string fullRelation)
        {
            string nodeID = nodeFullID.Split('.').Last();
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == nodeID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            entities.SaveChanges();
            return base.JsonAppendRelation<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID, relationData, "False");
        }

        #endregion

        #region 人员的角色

        public JsonResult GetUserRole(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_User, S_A__RoleUser, S_A_Role>(nodeFullID);
        }

        public JsonResult SetUserRole(string nodeFullID, string relationData, string fullRelation)
        {
            return base.JsonSetRelation<S_A_User, S_A__RoleUser, S_A_Role>(nodeFullID, relationData, fullRelation);
        }

        #endregion

        #region 人员的权限

        public JsonResult GetUserRes(string nodeFullID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();

            return Json(authBF.GetResByUserID(nodeFullID));
        }

        #endregion

        #region 作废的组织查看

        public ActionResult AbortTree()
        {
            return View();
        }

        public JsonResult GetAbortTree(string rootFullID)
        {
            if (rootFullID == null)
                rootFullID = "";

            var orgs = entities.Set<S_A_Org>().Where(c => c.FullID.StartsWith(rootFullID)).OrderBy(c => c.SortIndex);

            foreach (var org in orgs)
            {
                if (org.IsDeleted == "1")
                {
                    var ss = (org.DeleteTime == null) ? "" : org.DeleteTime.Value.ToShortDateString();
                    org.Name = "<b title ='作废日期：" + ss + "'>" + org.Name + "<font color='red' >(已作废)</font></b>";
                }
            }
            return Json(orgs, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 设置人员的组织角色

        public JsonResult SetOrgRoleUser(string OrgID, string RoleIDs, string UserIDs)
        {
            foreach (var userID in UserIDs.Split(','))
            {
                entities.Set<S_A__OrgRoleUser>().Delete(c => c.OrgID == OrgID && c.UserID == userID);

                foreach (var roleID in RoleIDs.Split(','))
                {
                    if (roleID == "") continue;
                    S_A__OrgRoleUser orgRoleUser = new S_A__OrgRoleUser();
                    orgRoleUser.UserID = userID;
                    orgRoleUser.OrgID = OrgID;
                    orgRoleUser.RoleID = roleID;
                    entities.Set<S_A__OrgRoleUser>().Add(orgRoleUser);

                }
            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 设置当前部门

        public JsonResult SetCurrentOrg(string userIDs, string deptID)
        {
            var org = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == deptID);

            foreach (string userID in userIDs.Split(','))
            {
                var user = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID);
                if (org != null)
                {
                    user.DeptID = org.ID;
                    user.DeptName = org.Name;
                    user.DeptFullID = org.FullID;
                }
                else
                {
                    user.DeptID = "";
                    user.DeptName = "";
                    user.DeptFullID = "";
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion

    }
}
