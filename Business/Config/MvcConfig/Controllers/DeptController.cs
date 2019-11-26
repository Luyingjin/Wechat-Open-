using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Config;

namespace MvcConfig.Controllers
{
    public class DeptController : BaseController
    {
        //
        // GET: /Dept/

        /// <summary>
        /// 获取二级部门
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDept()
        {
            string sql = "select * from S_A_Org where ParentID = 'a1b10168-61a9-44b5-92ca-c5659456deb5' order by SortIndex";
            DataTable dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

    }
}
