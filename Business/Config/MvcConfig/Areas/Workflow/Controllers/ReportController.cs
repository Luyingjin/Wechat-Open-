using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using System.Data;
using Formula.Helper;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class ReportController : BaseController
    {
        #region TaskTimeConsuming
        public ViewResult TaskTimeConsuming()
        {
            string sql = "select value=ID,text=Name from S_M_Category";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            ViewBag.Category = JsonHelper.ToJson(dt);

            return View();
        }

        public JsonResult GetFlowTimeConsuming(QueryBuilder qb)
        {
            string sql = @"
select 
CategoryID
,FlowCode=Code
,FlowName=S_WF_InsDefFlow.Name
,FlowCount=COUNT(1)
,TimeConsuming=avg(DateDiff (Minute, CreateTime,CompleteTime))/60.0
from S_WF_InsFlow
join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=InsDefFlowID
--where CompleteTime is not null
group by S_WF_InsDefFlow.Name,CategoryID,Code
";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            var dt = sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb);

            return Json(dt);
        }

        public JsonResult GetTaskTimeConsuming(string flowName, QueryBuilder qb)
        {
            string sql = @"
select 
FlowName=S_WF_InsDefFlow.Name
,StepName=S_WF_InsDefStep.Name
,SortIndex
,TimeConsuming=avg(DateDiff (Second, S_WF_InsTask.CreateTime,S_WF_InsTask.CompleteTime))/3600.0
,NoticeCount= COUNT(case when TimeConsuming>TimeoutNotice then 1 else null end)
,AlarmCount= COUNT(case when TimeConsuming>TimeoutAlarm then 1 else null end)
,DeadlineCount= COUNT(case when TimeConsuming>TimeoutDeadline then 1 else null end)
,TaskCount=COUNT(1)
from S_WF_InsTask 
join S_WF_InsDefStep on S_WF_InsDefStep.ID=InsDefStepID
join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=InsDefFlowID
where S_WF_InsDefStep.Type='Normal' and S_WF_InsDefFlow.Name='{0}' --and S_WF_InsTask.CompleteTime is not null
group by S_WF_InsDefFlow.Name,S_WF_InsDefStep.Name,S_WF_InsDefStep.SortIndex
order by FlowName,SortIndex
";
            sql = string.Format(sql, flowName);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            var dt = sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb);

            dt.Columns.Add("NoticePer");
            dt.Columns.Add("AlarmPer");
            dt.Columns.Add("DeadlinePer");
            dt.Columns.Add("NormalPer");

            foreach (DataRow row in dt.Rows)
            {
                if (row["TaskCount"].ToString() != "0")
                {
                    float NoticePer = int.Parse(row["NoticeCount"].ToString()) / int.Parse(row["TaskCount"].ToString());
                    float AlarmPer = int.Parse(row["AlarmCount"].ToString()) / int.Parse(row["TaskCount"].ToString());
                    float DeadlinePer = int.Parse(row["DeadlineCount"].ToString()) / int.Parse(row["TaskCount"].ToString());
                    float NormalPer = 1 - NoticePer - AlarmPer - DeadlinePer;

                    row["NoticePer"] = (NoticePer * 100).ToString() + "%";
                    row["AlarmPer"] = (AlarmPer * 100).ToString() + "%";
                    row["DeadlinePer"] = (DeadlinePer * 100).ToString() + "%";
                    row["NormalPer"] = (NormalPer * 100).ToString() + "%";
                }
            }

            return Json(dt);
        }
        #endregion

        #region TaskConsuming

        public ViewResult TaskConsuming()
        {

            #region 曲线

            string[] categories = new string[13];
            int[] data1 = new int[13];
            int[] data2 = new int[13];

            int m = DateTime.Now.Month;
            for (int i = 0; i < 13; i++)
            {
                categories[i] = string.Format("{0}月", m++ % 12);
                if (categories[i] == "0月")
                    categories[i] = "12月";
            }

            for (int i = 0; i < 13; i++)
            {
                data1[i] = 0;
                data2[i] = 0;
            }


            string sql = @"select CreateTime,CreateTimeCount=COUNT(CreateTime) from(
select CreateTime= month(CreateTime) from S_WF_InsFlow where CreateTime between '{0}' and '{1}'
) dt group by CreateTime";
            sql = string.Format(sql, new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, 1), DateTime.Now.Date.AddHours(23));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                data1[int.Parse(row["CreateTime"].ToString())] = int.Parse(row["CreateTimeCount"].ToString());
            }

            sql = @"select CompleteTime,CompleteTimeCount=COUNT(CompleteTime) from(
select CompleteTime= month(CompleteTime) from S_WF_InsFlow where CompleteTime between '{0}' and '{1}'
) dt group by CompleteTime";
            sql = string.Format(sql, new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, 1), DateTime.Now.Date.AddHours(23));
            sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            dt = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                data2[int.Parse(row["CompleteTime"].ToString())] = int.Parse(row["CompleteTimeCount"].ToString());
            }

            ViewBag.categories = JsonHelper.ToJson(categories);
            ViewBag.data1 = JsonHelper.ToJson(data1);
            ViewBag.data2 = JsonHelper.ToJson(data2);

            #endregion

            //活动流程
            sql = "select COUNT(1) from S_WF_InsFlow where CompleteTime is null and [Status]='Processing'";
            ViewBag.processingFlowCount = sqlHelper.ExecuteScalar(sql).ToString();

            //流程平均耗时和流程实例数量
            sql = "select s= SUM( datediff( SECOND, CreateTime, CompleteTime )),c= COUNT(1) from S_WF_InsFlow where CompleteTime is not null and [Status]='Complete'";
            dt = sqlHelper.ExecuteDataTable(sql);
            ViewBag.flowCount = int.Parse(dt.Rows[0]["c"].ToString());
            ViewBag.flowConsuming = int.Parse(dt.Rows[0]["s"].ToString());

            if (ViewBag.flowConsuming != 0)
                ViewBag.flowConsuming = string.Format("{0:F2}", (float)ViewBag.flowConsuming / ViewBag.flowCount / 3600);

            //任务平均耗时
            sql = "select s= SUM( datediff( SECOND, CreateTime, CompleteTime )),c= COUNT(1) from S_WF_InsTask where CompleteTime is not null and [Status]='Complete'";
            dt = sqlHelper.ExecuteDataTable(sql);
            ViewBag.taskCount = int.Parse(dt.Rows[0]["c"].ToString());
            ViewBag.taskConsuming = int.Parse(dt.Rows[0]["s"].ToString());
            if (ViewBag.taskConsuming != 0)
                ViewBag.taskConsuming = string.Format("{0:F2}", (float)ViewBag.taskConsuming / ViewBag.taskCount / 3600);

            //流程定义数量
            sql = "select count(1) from S_WF_DefFlow";
            ViewBag.flowDefCount = int.Parse(sqlHelper.ExecuteScalar(sql).ToString());

            //使用人数
            sql = "select COUNT( distinct TaskUserID) from S_WF_InsTaskExec";
            ViewBag.actUserCount = int.Parse(sqlHelper.ExecuteScalar(sql).ToString());

            sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sql = "select count(1) from S_A_User";
            ViewBag.userCount = int.Parse(sqlHelper.ExecuteScalar(sql).ToString());

            ViewBag.userPer = 0;
            if (ViewBag.userCount > 0)
                ViewBag.userPer = (int)((float)ViewBag.actUserCount / ViewBag.userCount * 100);

            return View();
        }

        #endregion
    }
}
