using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class OrgController : BaseController
    {
        #region Selector

        public ActionResult Selector()
        {
            return View();
        }

        public JsonResult GetTree()
        {
            string fullID = Request["RootFullID"];
            if (fullID == null)
                fullID = "";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string sql = string.Format("select ID,Code,Name,ParentID,Type from S_A_Org where  FullID like '{0}%' and IsDeleted='0'", fullID);

            if (!string.IsNullOrEmpty(Request["OrgType"]))
                sql += string.Format(" and Type in ('{0}')", Request["OrgType"].Replace(",", "','"));

            sql += " order by ParentID,SortIndex";
            return Json(sqlHelper.ExecuteDataTable(sql), JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}
