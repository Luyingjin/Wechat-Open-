using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula;

namespace MvcConfig.Areas.Project.Controllers
{
    public class DesignTaskController : BaseController
    {
        public JsonResult GetDesignTaskProductsList(QueryBuilder qb)
        {
            string wbsFullID = GetQueryString("WBSFULLID");
            string sql = "Select * from S_E_Product Where WBSFullID like '{0}%' ";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Project");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult GetDesignTaskList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(GetQueryString("ProjectInfoID")))
                qb.Add("ProjectInfoID", QueryMethod.Equal, GetQueryString("ProjectInfoID"));
            string sql = @"select DisplayName,CreateDate,LinkUrl,ID,WBSID,BusniessID,ProjectInfoID from S_W_Activity Where ActivityKey='DesignTask' and (OwnerUserID  like '%{0}%') And (State='Create') ORDER BY ID DESC";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Project");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
    }
}
