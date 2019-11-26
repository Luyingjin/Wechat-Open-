using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using Config;
using System.Data;
using MvcAdapter;

namespace Base.Areas.PortalBlock.Controllers
{
    public class CalendarController : BaseController
    {

        public JsonResult GetModel()
        {
            return Json("");
        }

        //
        // GET: /PortalBlock/Calendar/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit(string start, string end, string allDay)
        {
            return View();
        }

        public JsonResult Save()
        {
            S_H_Calendar model = UpdateEntity<S_H_Calendar>();
            entities.SaveChanges();
            return Json(model);
        }

        public JsonResult Delete(string id)
        {
            return base.JsonDelete<S_H_Calendar>(id);
        }

        public JsonResult GetPersonalSchedules(string start, string end)
        {
            string userID = FormulaHelper.GetUserInfo().UserID;
            IQueryable<S_H_Calendar> query = entities.Set<S_H_Calendar>().Where(c => c.CreateUserID == userID);
            if (!string.IsNullOrEmpty(start))
            {
                DateTime dateS = Convert.ToDateTime(start);
                query = query.Where(c => c.EndTime == null || c.EndTime >= dateS);
            }
            if (!string.IsNullOrEmpty(end))
            {
                DateTime dateE = Convert.ToDateTime(end);
                query = query.Where(c => c.StartTime < dateE);
            }
            List<S_H_Calendar> listSchedule = query.ToList();
            return Json(SchedulesToEvents(listSchedule), JsonRequestBehavior.AllowGet);
        }

        private List<Event> GetPersonalSchedules(MvcAdapter.QueryBuilder qb)
        {
            string userID = FormulaHelper.GetUserInfo().UserID;
            IQueryable<S_H_Calendar> query = entities.Set<S_H_Calendar>().Where(c => c.CreateUserID == userID);
            List<S_H_Calendar> list = query.Where(qb).ToList();
            return SchedulesToEvents(list);
        }

        private List<Event> SchedulesToEvents(List<S_H_Calendar> list)
        {
            List<Event> listEvent = new List<Event>();
            foreach (S_H_Calendar item in list)
            {
                Event ev = new Event();
                ev.id = item.ID;
                ev.title = item.Title;
                ev.description = item.Description;
                ev.start = Convert.ToDateTime(item.StartTime);
                ev.end = Convert.ToDateTime(item.EndTime);
                ev.allDay = Convert.ToDateTime(ev.start).ToString("T") == "0:00:00" && Convert.ToDateTime(ev.end).ToString("T") == "0:00:00" ? true : false;
                ev.editable = true;
                ev.startEditable = true;
                ev.endEditable = true;
                ev.type = EventType.Schedule;
                listEvent.Add(ev);
            }
            return listEvent;
        }

        public JsonResult GetTasks(string start, string end)
        {
            string sql = @"
select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTask.ID as TaskID
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,FormUrl
,{5}(FormWidth,0) as FormWidth
,{5}(FormHeight,0) as FormHeight
,S_WF_InsTaskExec.ExecTime
from S_WF_InsTaskExec
join S_WF_InsTask on ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (S_WF_InsTaskExec.CreateTime between {2}'{1}' and {4}'{3}') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID

";
            string isOracle = Config.Constant.IsOracleDb ? "date" : "";
            string isOracleNull = Config.Constant.IsOracleDb ? "nvl" : "isnull";
            if (string.IsNullOrEmpty(start))
                start = DateTime.MinValue.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(end))
                end = DateTime.MaxValue.ToString("yyyy-MM-dd");
            sql = string.Format(sql, FormulaHelper.GetUserInfo().UserID, start.Replace(" 00:00:00", ""), isOracle, end.Replace(" 00:00:00", ""), isOracle, isOracleNull);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sqlBase = @"
select ID as id,StartTime as start,[EndTime] as endtime,Title as title,Description as [description]
	,'' as url,'' as width,'' as height,'Schedule' as [type],'' as FormInstanceID
from S_H_Calendar
where CreateUserID = '{0}'

";
            string userID = FormulaHelper.GetUserInfo().UserID;
            sqlBase = string.Format(sqlBase, userID);
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            GridData gridData = baseSqlHelper.ExecuteGridData(sqlBase, qb);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
    }

    public class Event
    {
        public string id;
        public DateTime start;
        public DateTime end;
        public string title;
        public string description;
        public bool allDay;
        public string url;
        public string width;
        public string height;
        public bool editable;
        public bool startEditable;
        public bool endEditable;
        public EventType type;
    }

    public enum EventType
    {
        Schedule,
        Task,
        News,
    }
}
