using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using Formula;

namespace Base.Areas.ShortMsg.Controllers
{
    public class AlarmController : BaseController<S_S_Alarm>
    {
        public ActionResult AlarmList()
        {
            return View();
        }
        public ActionResult AlarmDetail(string id)
        {
            var model = entities.Set<S_S_Alarm>().FirstOrDefault(p => p.ID == id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qb"></param>
        /// <param name="important">重要度</param>
        /// <param name="urgency">紧急性</param>
        /// <returns></returns>
        public JsonResult GetAlarmList(MvcAdapter.QueryBuilder qb, string important, string urgency, string type)
        {
            /*不支持默认多个排序
            IQueryable<S_S_Alarm> data = entities.Set<S_S_Alarm>().Where(p => p.OwnerID == FormulaHelper.UserID && (string.IsNullOrEmpty(p.IsDelete) == true || p.IsDelete != "1"));
            if (!string.IsNullOrEmpty(important))
                data = data.Where(p => p.Important == important);
            if (!string.IsNullOrEmpty(urgency))
                data = data.Where(p => p.Urgency == urgency);

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "last")//最新
                    data = data.Where(p => p.DeadlineTime >= DateTime.Now);
                else if (type.ToLower() == "history")//历史
                    data = data.Where(p => p.DeadlineTime < DateTime.Now);
            }
            return Json(data.WhereToGridData(qb));
            */

            string sql = string.Format(@"SELECT * FROM S_S_Alarm WHERE OwnerID='{0}' AND (IsDelete is null OR IsDelete='0')", FormulaHelper.UserID);
            if (!string.IsNullOrEmpty(important))
                sql += string.Format(" AND (Important ='{0}') ", important);
            if (!string.IsNullOrEmpty(urgency))
                sql += string.Format(" AND (Urgency ='{0}') ", urgency);

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "last")//最新
                    sql += string.Format(" AND (DeadlineTime >='{0}') ", DateTime.Now);
                else if (type.ToLower() == "history")//历史
                    sql += string.Format(" AND (DeadlineTime <'{0}') ", DateTime.Now);
            }
            var data = SQLHelper.CreateSqlHelper("Base").ExecuteGridData(sql, qb);
            return Json(data);

        }
        public JsonResult GetLastAlarmList()
        {
            var user = FormulaHelper.GetUserInfo();
            List<S_S_Alarm> data = entities.Set<S_S_Alarm>().Where(p => p.OwnerID == user.UserID && (string.IsNullOrEmpty(p.IsDelete) == true || p.IsDelete != "T") && p.DeadlineTime >= DateTime.Now).OrderBy(p => p.DeadlineTime).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 标记为假删除.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DeleteAlarm(string ids)
        {
            List<S_S_Alarm> list = entities.Set<S_S_Alarm>().Where(p => ids.IndexOf(p.ID) >= 0).ToList();
            foreach (var item in list)
                item.IsDelete = "1";
            entities.SaveChanges();
            return Json("");
        }
    }
}
