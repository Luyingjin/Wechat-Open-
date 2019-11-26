using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class RoleController : BaseController
    {
        public ActionResult Selector()
        {
            return View();
        }

        public JsonResult GetOrgRoleList(MvcAdapter.QueryBuilder qb)
        {
            string sql = string.Format("select ID,Code,Name,Type,Description from S_A_Role where GroupID='{0}' and Type='OrgRole'", Request["GroupID"]);
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = "select ID,Code,Name,Type,Description from S_A_Role where Type='OrgRole'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb));
        }

        public JsonResult GetSysRoleList(MvcAdapter.QueryBuilder qb)
        {
            string sql = string.Format("select ID,Code,Name,Type,Description from S_A_Role where GroupID='{0}' and Type='SysRole'", Request["GroupID"]);
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = "select ID,Code,Name,Type,Description from S_A_Role where Type='SysRole'";


           
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, qb));
        }
    }
}
