using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Formula.Helper;
using Base.Logic.Domain;
using MvcAdapter;
using Config;
using Formula;
using System.Text;
using Base.Logic.BusinessFacade;

namespace Base.Areas.Meta.Controllers
{
    public class DataController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        #region 树

        public JsonResult GetTree()
        {
            return base.JsonGetTree<S_M_Category>("0");
        }

        #endregion

        #region Table

        public JsonResult GetTableList(QueryBuilder qb)
        {
            if (qb.SortField == "ID")
            {
                qb.SortField = "Code";
                qb.SortOrder = "asc";
            }
            return base.JsonGetList<S_M_Table>(qb);
        }

        public JsonResult SaveTable()
        {
            return base.JsonSaveList<S_M_Table>();
        }

        #endregion

        #region Field

        public JsonResult GetFieldList(QueryBuilder qb)
        {
            return base.JsonGetList<S_M_Field>(qb);
        }

        public JsonResult SaveField()
        {
            return base.JsonSaveList<S_M_Field>();
        }

        #endregion

        #region 表和字段的导入

        public JsonResult ImportTable(string connName)
        {
            MetaFO metaFO = FormulaHelper.CreateFO<MetaFO>();
            metaFO.ImportTable(connName);
            return Json("");
        }

        public JsonResult ImportField(string tableID)
        {
            MetaFO metaFO = FormulaHelper.CreateFO<MetaFO>();
            metaFO.ImportField(tableID);
            return Json("");
        }

        #endregion

    }
}
