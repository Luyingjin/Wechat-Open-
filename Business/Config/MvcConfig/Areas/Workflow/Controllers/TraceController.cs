using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using System.Text;
using Formula;
using Workflow.Logic.Domain;


namespace MvcConfig.Areas.Workflow.ControllersGetFlowExecList
{
    public class TraceController : BaseController
    {

        string sqlFlowExecList = @"
select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.CreateTime as CreateTime
,TaskUserID
,TaskUserName
,ExecUserID
,ExecUserName
,ExecTime
,ExecComment
,S_WF_InsTaskExec.Type as Type
,S_WF_InsTask.ID as TaskID
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,FlowName
,FlowCategory
,FlowSubCategory
,S_WF_InsDefStep.Name as StepName
,S_WF_InsDefStep.ID as StepID
,ExecRoutingIDs
,ExecRoutingName
,S_WF_InsFlow.InsDefFlowID
,S_WF_InsTask.DoBackRoutingID
,S_WF_InsTask.OnlyDoBack
from S_WF_InsTaskExec
right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID
where FormInstanceID='{0}' and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='')
order by isnull(S_WF_InsTaskExec.CreateTime,S_WF_InsTask.CreateTime)
";

        #region GetFlowExecList

        public JsonResult GetFlowExecList(string id)
        {

            string sql = string.Format(sqlFlowExecList, id);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            if (dt.Rows.Count == 0)
                return Json(dt);

            string insDefFlowID = dt.Rows[0]["InsDefFlowID"].ToString();
            sql = string.Format("select ID,Name from S_WF_InsDefRouting where InsDefFlowID='{0}'", insDefFlowID);
            DataTable dtRouting = sqlHelper.ExecuteDataTable(sql);

            dt.Columns.Add("UseTime");
            dt.Columns.Add("TaskUserDept");

            var userService = FormulaHelper.GetService<IUserService>();

            foreach (DataRow row in dt.Rows)
            {
                string ExecRoutingIDs = row["ExecRoutingIDs"].ToString().Trim(',');
                if (!string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "")
                {
                    row["ExecRoutingName"] = dtRouting.AsEnumerable().SingleOrDefault(c => c["ID"].ToString() == ExecRoutingIDs.Split(',').LastOrDefault())["Name"];
                }
                //处理打回和直送操作的名称
                if (string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "" && row["ExecTime"].ToString() != "")
                {
                    if (row["DoBackRoutingID"].ToString() != "")
                        row["ExecRoutingName"] = "驳回";
                    if (row["OnlyDoBack"].ToString() == "1")
                        row["ExecRoutingName"] = "送驳回人";
                }

                string CreateTime = row["CreateTime"].ToString();
                string ExecTime = row["ExecTime"].ToString();
                if (!string.IsNullOrEmpty(ExecTime))
                {
                    var span = DateTime.Parse(ExecTime) - DateTime.Parse(CreateTime);
                    row["UseTime"] = string.Format("{0}小时{1}分", span.Days * 24 + span.Hours, span.Minutes == 0 ? 1 : span.Minutes);
                }
                if (row["TaskUserID"].ToString() != "")
                {
                    row["TaskUserDept"] = userService.GetUserInfoByID(row["TaskUserID"].ToString()).UserOrgName;
                }
                else
                {
                    row["TaskUserName"] = "";
                    row["ExecUserName"] = "";
                }

            }

            return Json(dt);
        }

        #endregion


        public ViewResult Diagram()
        {
            string id = Request["ID"];
            var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();

            var flow = flowEntities.S_WF_InsFlow.SingleOrDefault(c => c.FormInstanceID == id);
            if (flow == null)
                ViewBag.ExistFlow = "false";
            else
            {
                ViewBag.ExistFlow = "true";
                if (flow.S_WF_InsDefFlow.IsFreeFlow == "1")
                    Response.Redirect("/MvcConfig/Workflow/Trace/FreeFlowExecDetail?ID=" + id);
            }
            return View();
        }

        public ViewResult Sequence()
        {
            ViewBag.TableHtml = getSequenceHtml(Request["ID"]);
            return View();
        }


        private string getSequenceHtml(string formInstanceID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            string sqlStep = string.Format(@"select S_WF_InsDefStep.Name from S_WF_InsDefStep
join S_WF_InsFlow on S_WF_InsFlow.FormInstanceID='{0}' and S_WF_InsFlow.InsDefFlowID=S_WF_InsDefStep.InsDefFlowID where S_WF_InsDefStep.Type<>'Completion'
order by S_WF_InsDefStep.SortIndex", formInstanceID);

            DataTable dtStepName = sqlHelper.ExecuteDataTable(sqlStep);

            string[] stepArr = dtStepName.AsEnumerable().Select(c => c["Name"].ToString()).Distinct().ToArray();

            string sql = string.Format(sqlFlowExecList, formInstanceID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table width='{0}px' class='CTable' cellpadding='0' cellspacing='0'>", dtStepName.Rows.Count * 200 + 130);
            sb.Append("<tr style='background:#E9EBE7;height:50px'><td align='center' width='130px'><strong>执行时间\\环节内容</strong></td>");
            foreach (string stepName in stepArr)
            {
                sb.AppendFormat("<td width='200px' align='center'><strong>{0}</strong></td>", stepName);
            }
            sb.Append("</tr>");
            int i = 0;
            string _stepName = "";
            foreach (DataRow row in dt.Rows)
            {
                if (_stepName != row["StepName"].ToString())
                    i++;
                _stepName = row["StepName"].ToString();
                sb.Append(getSequenceRowHtml(row, stepArr, i));


            }

            sb.Append("</table>");
            return sb.ToString();
        }

        private string getSequenceRowHtml(DataRow row, string[] stepArr, int idx)
        {
            string cellHtml = @"
<table class='ITable'  border='0' style='background:{4}' cellpadding='0' cellspacing='0' width='100%' height='100%'>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/user.png' /></td><td align='right'>接&nbsp;收&nbsp;人&nbsp;：</td><td align='left'>{0}</td></tr>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/node.png' /></td><td>接收时间：</td><td>{1}</td></tr>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/user.png' /></td><td align='right'>操&nbsp;作&nbsp;人&nbsp;：</td><td align='left'>{2}</td></tr>    
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/node.png' /></td><td>操作时间：</td><td>{3}</td></tr>
</table>
";
            cellHtml = string.Format(cellHtml
                , string.Format("<a href='#' onclick='viewDetail(\"{1}\",\"{2}\");'>{0}</a>", row["TaskUserName"], row["ID"], row["TaskUserID"])
                , row["CreateTime"].ToString() == "" ? "" : DateTime.Parse(row["CreateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                , string.Format("<a href='#' onclick='viewDetail(\"{1}\",\"{2}\");'>{0}</a>", row["ExecUserName"], row["ID"], row["ExecUserID"])
                , row["ExecTime"].ToString() == "" ? "" : DateTime.Parse(row["ExecTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                , row["ExecTime"].ToString() == "" ? "#B4D02E" : "#A1D8DD");


            StringBuilder sb = new StringBuilder();
            sb.Append("\n<tr style='height:100px;'>");
            if (row["CreateTime"].ToString() != "")
                sb.AppendFormat("<td align='left' style='padding-left:4px;background:#E9EBE7;'><font size='4' color='#555'><b>第{0}步</b></font><font color='red'><strong>{2}</strong></font><br>{1}</td>", idx.ToString(), row["ExecTime"].ToString() == "" ? "　日期：<br>　时间：" : DateTime.Parse(row["ExecTime"].ToString()).ToString("　日期：yyyy-MM-dd<br>　时间：HH:mm:ss"), row["ExecTime"].ToString() == "" ? "（执行中）" : "");
            else
                sb.AppendFormat("<td align='left' style='padding-left:4px;background:#E9EBE7;'><font size='4' color='#555'><b>第{0}步</b></font><font color='#A1D8DD'><strong>{2}</strong></font><br>{1}</td>", idx.ToString(), row["ExecTime"].ToString() == "" ? "　日期：<br>　时间：" : DateTime.Parse(row["ExecTime"].ToString()).ToString("　日期：yyyy-MM-dd<br>　时间：HH:mm:ss"), row["ExecTime"].ToString() == "" ? "（结束）" : "");


            foreach (string stepName in stepArr)
            {
                if (stepName == row["StepName"].ToString())
                    sb.AppendFormat("<td align='center' valign='center'>{0}</td>", cellHtml);
                else
                    sb.AppendFormat("<td>&nbsp;</td>");
            }

            sb.Append("</tr>");
            return sb.ToString();
        }

        public JsonResult GetUserInfo(string id, string userid)
        {
            string sql = string.Format("SELECT * FROM S_WF_INSTASKEXEC WHERE ID='{0}'", id);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dtExec = sqlHelper.ExecuteDataTable(sql);

            sql = string.Format("select ID, NAME as \"Name\",DEPTNAME as \"DeptName\",PHONE as \"Phone\",MOBILEPHONE as \"MobilePhone\" from S_A_User where ID='{0}'", userid);
            sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dtUser = sqlHelper.ExecuteDataTable(sql);

            dtUser.Columns.Add("CreateTime");
            dtUser.Columns.Add("ExecTime");
            dtUser.Columns.Add("ExecComment");

            DataRow row = dtUser.Rows[0];
            row["CreateTime"] = dtExec.Rows[0]["CreateTime"];
            row["ExecTime"] = dtExec.Rows[0]["ExecTime"];
            row["ExecComment"] = dtExec.Rows[0]["ExecComment"];
            return Json(row);
        }
    }
}
