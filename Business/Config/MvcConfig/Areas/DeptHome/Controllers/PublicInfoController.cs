using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using MvcAdapter;
using Config;
using Formula.Helper;

namespace MvcConfig.Areas.DeptHome.Controllers
{
    public class PublicInfoController : Controller
    {
        //
        // GET: /DeptHome/PublicInfo/

        public ActionResult Tabs()
        {
            return View();
        }

        public ActionResult List()
        {
            ViewBag.UserInfo = JsonHelper.ToJson(Formula.FormulaHelper.GetUserInfo());
            ViewBag.CatalogEnum = JsonHelper.ToJson(SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable("select ID as value,CatalogName as text from S_I_PublicInformCatalog"));
            ViewBag.CatalogVisible = string.IsNullOrEmpty(this.Request["CatalogId"]) ? "true" : "false";
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            IQueryable<S_I_PublicInformation> query = entities.Set<S_I_PublicInformation>().Where(c => !string.IsNullOrEmpty(c.DeptDoorId)).AsQueryable();
            string catalogID = this.Request["CatalogId"];
            if (!string.IsNullOrEmpty(catalogID))
            {
                query = query.Where(c => c.CatalogId == catalogID);
            }
            GridData gridData = query.WhereToGridData(qb);
            return Json(gridData);
        }

    }
}
