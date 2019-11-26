using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using MvcAdapter;
using Formula;
using Workflow.Logic.Domain;
using Workflow.Logic;
using System.Text;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class MonitorController : BaseController
    {
        public JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (Config.Constant.IsOracleDb)
            {
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID, NAME as \"Name\", PARENTID as \"ParentID\", FULLID as \"FullID\" FROM S_M_CATEGORY WHERE FULLID like '{0}%'", "0"));
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_M_Category where FullID like '{0}%'", "0"));
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string sql = string.Format(@"select S_WF_InsFlow.FormInstanceID as ID, CreateTime as CREATETIME, Name as NAME,FlowName as INSTANCENAME from S_WF_InsFlow
join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=InsDefFlowID
 where Status='{0}'", Request["Status"]);

            string categoryID = Request["NodeFullID"].Split('.').Last();
            if (categoryID != "0")
            {
                sql += string.Format(" and CategoryID='{0}'", categoryID);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult GetTaskExecList()
        {
            string taskID = Request["TaskID"];
            string sql = string.Format("select * from S_WF_InsTaskExec where InsTaskID='{0}'", taskID);
            SQLHelper sqlHelpe = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dt = sqlHelpe.ExecuteDataTable(sql);
            dt.Columns.Add("Status");
            foreach (DataRow row in dt.Rows)
            {
                if (row["ExecTime"].ToString() == "")
                    row["Status"] = "未执行";
                else
                    row["Status"] = "已执行";
            }
            return Json(dt);
        }

        public JsonResult AddTaskExec(string taskID, string UserID, string userName)
        {
            //创建TaskExec
            S_WF_InsTaskExec taskExec = new S_WF_InsTaskExec();
            taskExec.ID = FormulaHelper.CreateGuid();
            taskExec.InsTaskID = taskID;
            taskExec.CreateTime = DateTime.Now;
            taskExec.TaskUserID = UserID;
            taskExec.TaskUserName = userName;

            //执行人
            taskExec.ExecUserID = taskExec.TaskUserID;
            taskExec.ExecUserName = taskExec.TaskUserName;
            taskExec.Type = TaskExecType.Normal.ToString();

            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().Where(c => c.ID == taskID).SingleOrDefault();
            taskExec.InsFlowID = task.InsFlowID;
            entities.Set<S_WF_InsTaskExec>().Add(taskExec);
            entities.SaveChanges();

            return Json("");
        }

        public JsonResult DelTaskExec(string taskID, string listIDs)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskID);
            string[] ids = listIDs.Split(',');

            if (task.S_WF_InsTaskExec.Count() == ids.Count())
                throw new Exception("不能全部删除！");
            foreach (var item in task.S_WF_InsTaskExec.ToArray())
            {
                if (item.ExecTime != null)
                    throw new Exception("已完成的任务不能删除");

                if (ids.Contains(item.ID))
                    entities.Set<S_WF_InsTaskExec>().Remove(item);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ReplaceExecUser(string taskExecID, string userID, string userName)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var exec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
            exec.ExecUserID = userID;
            exec.ExecUserName = userName;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Delete(string listIDs)
        {
            SQLHelper sqlWorkflowHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string sql = "select FormInstanceID,ConnName,TableName from S_WF_InsFlow join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID where FormInstanceID in('{0}') order by ConnName";
            sql = string.Format(sql, listIDs.Replace(",", "','"));
            DataTable dt = sqlWorkflowHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());
                sql = string.Format("delete from {0} where ID='{1}'", row["TableName"], row["FormInstanceID"]);
                sqlHelper.ExecuteNonQuery(sql);
            }

            sql = "delete from S_WF_InsFlow where FormInstanceID in ('{0}')";
            sql = string.Format(sql, listIDs.Replace(",", "','"));
            sqlWorkflowHelper.ExecuteDataTable(sql);

            return Json("");
        }


    }
}
