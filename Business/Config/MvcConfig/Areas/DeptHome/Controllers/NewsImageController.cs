using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using System.Data;
using Base.Logic.Domain;
using Formula;

namespace MvcConfig.Areas.DeptHome.Controllers
{
    public class NewsImageController : BaseController
    {
        private const string NewsImagePrefix = "NewsImage_";
        //
        // GET: /DeptHome/NewsImage/

        public ActionResult List()
        {
            ViewBag.UserInfo = Formula.Helper.JsonHelper.ToJson(Formula.FormulaHelper.GetUserInfo());
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string userFullOrgID = Formula.FormulaHelper.GetUserInfo().UserFullOrgID;
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            IQueryable<S_I_NewsImageGroup> list = entities.Set<S_I_NewsImageGroup>().Where(c => !string.IsNullOrEmpty(c.DeptDoorId)).AsQueryable();
            GridData gridData = list.WhereToGridData(qb);
            return Json(gridData);
        }

        public JsonResult GetDeptHomeNewsImageGroup(string deptHomeID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,c.Remark,b.ID as NewsImageID,b.CreateTime from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage group by GroupID
                            ) a join S_I_NewsImage b on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c on c.ID=b.GroupID
                            where DeptDoorId = '{0}'
                            order by c.CreateTime desc";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, deptHomeID));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }


    }
}
