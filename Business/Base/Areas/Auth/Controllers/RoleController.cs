using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Base.Logic;
using Formula.Exceptions;
using Config;
using MvcAdapter;

namespace Base.Areas.Auth.Controllers
{
    public class RoleController : BaseController<S_A_Role>
    {
        public JsonResult GetRoleUserList(string roleID, QueryBuilder qb)
        {
            string sql = @"select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName from S_A__OrgRole 
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID and RoleID='{0}'
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
union
select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName  from S_A_User join S_A__RoleUser on ID=UserID and RoleID='{0}'
union
select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName  from S_A_User join S_A__OrgRoleUser on ID=UserID and RoleID='{0}'
";
            sql = string.Format(sql, roleID);
            SQLHelper sqlHelper =  SQLHelper.CreateSqlHelper(ConnEnum.Base);
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public override JsonResult Save()
        {
            var entity = base.UpdateEntity<S_A_Role>();

            if (entities.Set<S_A_Role>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception("角色编号不能重复！");

            if (entity.Type == Base.Logic.RoleType.OrgRole.ToString())
            {
                entity.S_A__RoleUser.Clear();
            }

            entities.SaveChanges();
            return Json("");
        }


        public JsonResult GetRoleRes(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_Role, S_A__RoleRes, S_A_Res>(nodeID);
        }

        public JsonResult SetRoleRes(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__RoleRes>().Where(c => c.RoleID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            return base.JsonSetRelation<S_A_Role, S_A__RoleRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult SetRoleRule(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__RoleRes>().Where(c => c.RoleID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            return base.JsonSetRelation<S_A_Role, S_A__RoleRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult GetRoleUser(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_Role, S_A__RoleUser, S_A_User>(nodeID);
        }

        public JsonResult SetRoleUser(string nodeFullID, string relationData, string fullRelation)
        {
            return base.JsonSetRelation<S_A_Role, S_A__RoleUser, S_A_User>(nodeFullID, relationData, "False");
        }
        
    }
}
