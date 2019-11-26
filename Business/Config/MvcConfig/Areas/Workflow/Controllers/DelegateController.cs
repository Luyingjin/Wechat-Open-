using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula;
using Config;
using System.Data;
using Formula.Helper;
using System.Text;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class DelegateController : BaseController
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
            string sql = @"select S_WF_DefFlow.ID,Code,Name,Description,DelegateUserName,BeDelegateUserName,DelegateTime,BeginTime,EndTime from S_WF_DefFlow 
left join S_WF_DefDelegate on DefFlowID=S_WF_DefFlow.iD and DelegateUserID='{0}'";
            if (Config.Constant.IsOracleDb)
            {
                sql = "SELECT S_WF_DEFFLOW.ID,CODE as \"Code\",NAME as \"Name\",DESCRIPTION as \"Description\",DELEGATEUSERNAME as \"DelegateUserName\",BEDELEGATEUSERNAME as \"BeDelegateUserName\",DELEGATETIME  as \"DelegateTime\",BEGINTIME  as \"BeginTime\",ENDTIME  as \"EndTime\" FROM S_WF_DEFFLOW LEFT JOIN S_WF_DEFDELEGATE ON DEFFLOWID=S_WF_DEFFLOW.ID AND DELEGATEUSERID='{0}'";
            }

            string categoryID = Request["NodeFullID"].Split('.').Last();
            if (categoryID != "0")
            {
                sql += string.Format(" where CategoryID='{0}'", categoryID);
            }

            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult GetModel(string id)
        {
            return Json("");
        }

        public JsonResult DelegateFlow()
        {
            Dictionary<string, object> formDic = JsonHelper.ToObject<Dictionary<string, object>>(Request["FormData"]);
            DateTime beginTime = DateTime.Parse(formDic["BeginTime"].ToString());
            beginTime = beginTime.Date;
            DateTime endTime = DateTime.Parse(formDic["EndTime"].ToString());
            endTime = endTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            string defFlowIDs = Request["defFlowIDs"];
            if (defFlowIDs == "all")
                return DelegateAllFlow();

            UserInfo user = FormulaHelper.GetUserInfo();


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            string sql = " delete from S_WF_DefDelegate where DelegateUserID='{0}' and DefFlowID in('{1}')";
            sql = string.Format(sql, user.UserID, defFlowIDs.Replace(",", "','"));

            StringBuilder sb = new StringBuilder();
            foreach (string defFlowID in defFlowIDs.Split(','))
            {
                if (defFlowID == "")
                    continue;

                if (Config.Constant.IsOracleDb)
                {
                    sb.AppendFormat(@" 
                     insert into S_WF_DefDelegate (ID,DefFlowID,DelegateUserID,DelegateUserName,BeDelegateUserID,BeDelegateUserName,DelegateTime,BeginTime,EndTime) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}',to_date('{5}','yyyy/MM/dd hh24:mi:ss'),to_date('{6}','yyyy/MM/dd hh24:mi:ss'),to_date('{7}','yyyy/MM/dd hh24:mi:ss'))"
                       , defFlowID
                       , user.UserID
                       , user.UserName
                       , formDic["BeDelegateUserID"]
                       , formDic["BeDelegateUserName"]
                       , DateTime.Now
                       , beginTime
                       , endTime
                       );
                }
                else
                {
                    sb.AppendFormat(@" 
                     insert into S_WF_DefDelegate (ID,DefFlowID,DelegateUserID,DelegateUserName,BeDelegateUserID,BeDelegateUserName,DelegateTime,BeginTime,EndTime) values(newid(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
                       , defFlowID
                       , user.UserID
                       , user.UserName
                       , formDic["BeDelegateUserID"]
                       , formDic["BeDelegateUserName"]
                       , DateTime.Now
                       , beginTime
                       , endTime
                       );
                }
            }

            if (Config.Constant.IsOracleDb)
            {
                sqlHelper.ExecuteNonQuery(sql);
                if (sb.Length > 0)
                {
                    sqlHelper.ExecuteNonQuery(sb.ToString());
                }
            }
            else
            {
                sqlHelper.ExecuteNonQuery(sql + sb.ToString());
            }

            return Json("");
        }

        public JsonResult CancelDelegateFlow()
        {
            string defFlowIDs = Request["ListIDs"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            string sql = " delete from S_WF_DefDelegate where DelegateUserID='{0}' and DefFlowID in('{1}')";
            sql = string.Format(sql, FormulaHelper.UserID, defFlowIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }


        public JsonResult DelegateAllFlow()
        {

            UserInfo user = FormulaHelper.GetUserInfo();

            Dictionary<string, object> formDic = JsonHelper.ToObject<Dictionary<string, object>>(Request["FormData"]);
            DateTime beginTime = DateTime.Parse(formDic["BeginTime"].ToString());
            beginTime = beginTime.Date;
            DateTime endTime = DateTime.Parse(formDic["EndTime"].ToString());
            endTime = endTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            string sql = " delete from S_WF_DefDelegate where DelegateUserID='{0}'";
            sql = string.Format(sql, FormulaHelper.UserID);
            if (Config.Constant.IsOracleDb)
            {
                sqlHelper.ExecuteNonQuery(sql);
            }

            var oracleSql = string.Format(@" insert into S_WF_DefDelegate(ID,DefFlowID,DelegateUserID,DelegateUserName,BeDelegateUserID,BeDelegateUserName,DelegateTime,BeginTime,EndTime) 
select ID=newid(),ID,'{0}','{1}','{2}','{3}','{4}','{5}','{6}'from S_WF_DefFlow"
                , user.UserID
                , user.UserName
                , formDic["BeDelegateUserID"]
                , formDic["BeDelegateUserName"]
                , DateTime.Now
                , beginTime
                , endTime
                );
            if (Config.Constant.IsOracleDb)
            {
                sqlHelper.ExecuteNonQuery(oracleSql.Replace("newid()", "sys_guid()"));
            }

            sql += oracleSql;
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult CancelDelegateAllFlow()
        {
            UserInfo user = FormulaHelper.GetUserInfo();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            string sql = " delete from S_WF_DefDelegate where DelegateUserID='{0}'";
            sql = string.Format(sql, FormulaHelper.UserID);
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

    }
}
