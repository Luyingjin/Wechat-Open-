using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Config;
using System.Data;

namespace Config.Logic
{
    public class WorkflowService
    {
        public static string GetFlowCurrentStepCode()
        {
            string id = HttpContext.Current.Request["ID"];
            string taskExecID = HttpContext.Current.Request["TaskExecID"];
            string flowCode = HttpContext.Current.Request["FlowCode"];

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            string sql = "";

            if (string.IsNullOrEmpty(taskExecID) && string.IsNullOrEmpty(flowCode)) //非流程页面
            {
                return "";
            }
            else if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(flowCode)) //从表单列表添加
            {
                sql = string.Format(@"select Code from S_WF_DefStep where DefFlowID=(select ID from S_WF_DefFlow where Code='{0}') and S_WF_DefStep.Type='Inital' "
                    , flowCode);
            }
            else if (!string.IsNullOrEmpty(taskExecID)) //从任务列表打开
            {
                sql = string.Format(@"
select Code from S_WF_InsDefStep where ID in(
select InsDefStepID from S_WF_InsTask where InsFlowID =
(select InsFlowID from S_WF_InsTaskExec where ID='{0}') and S_WF_InsTask.Status='Processing'
) ", taskExecID);

            }
            else if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(flowCode)) //从表单列表编辑
            {
                sql = string.Format("select ID from S_WF_InsFlow where FormInstanceID='{0}'", id);
                object obj = sqlHelper.ExecuteScalar(sql);
                if (obj != null)
                {
                    sql = string.Format(@"
select Code from S_WF_InsDefStep where ID in(
select InsDefStepID from S_WF_InsTask where InsFlowID ='{0}' and S_WF_InsTask.Status='Processing'
) ", obj.ToString());
                }
                else
                {
                    sql = string.Format("select Code from S_WF_DefStep where DefFlowID=(select ID from S_WF_DefFlow where Code='{0}') and S_WF_DefStep.Type='Inital'", flowCode);
                }             
            }

            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            string result = string.Join(",", dt.AsEnumerable().Select(c => c["Code"].ToString()).ToArray());

            return result;
        }
    }
}
