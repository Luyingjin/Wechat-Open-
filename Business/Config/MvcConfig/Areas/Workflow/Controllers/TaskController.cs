using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula;
using System.Data;
using Formula.Helper;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class TaskController : BaseController
    {
        public JsonResult GetMyTaskTree()
        {
            #region 待办
            Dictionary<string, object> dicUndo = new Dictionary<string, object>();
            dicUndo["value"] = "Undo";
            dicUndo["text"] = "待办任务";
            dicUndo["name"] = "待办任务";
            dicUndo["iconCls"] = "Undo";
            dicUndo["type"] = "Status";
            List<Dictionary<string, object>> undoCategory = GetCategorys();
            dicUndo["children"] = undoCategory;
            #endregion

            #region 已办
            Dictionary<string, object> dicDone = new Dictionary<string, object>();
            dicDone["value"] = "Done";
            dicDone["text"] = "已办任务";
            dicDone["name"] = "已办任务";
            dicDone["iconCls"] = "Done";
            dicDone["type"] = "Status";
            List<Dictionary<string, object>> doneCategory = GetCategorys();
            dicDone["children"] = doneCategory;
            #endregion

            #region 已委托
            Dictionary<string, object> dicDelegate = new Dictionary<string, object>();
            dicDelegate["value"] = "Delegate";
            dicDelegate["text"] = "已委托任务";
            dicDelegate["name"] = "已委托任务";
            dicDelegate["iconCls"] = "Delegate";
            dicDelegate["type"] = "Status";
            #endregion

            #region 设计
            Dictionary<string, object> dicDesign = new Dictionary<string, object>();
            dicDesign["value"] = "Design";
            dicDesign["text"] = "设计任务";
            dicDesign["name"] = "设计任务";
            dicDesign["iconCls"] = "Design";
            dicDesign["type"] = "Status";
            #endregion

            List<Dictionary<string, object>> treeData = new List<Dictionary<string, object>>();
            treeData.Add(dicUndo);
            treeData.Add(dicDone);
            treeData.Add(dicDelegate);
            treeData.Add(dicDesign);

            return Json(treeData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimeIntervals()
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Type source = typeof(TimeInterval);
            foreach (string name in Enum.GetNames(source))
            {
                //object enumValue = Enum.Parse(source, name);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string enumDesc = GetEnumDesc(source, name);
                dic["title"] = enumDesc;
                dic["name"] = name;
                list.Add(dic);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUndoCategoryCount()
        {
            string sql = @"select count(S_WF_InsTaskExec.ID) as eCount,S_WF_InsDefFlow.CategoryID
                            from S_WF_InsTaskExec
                            join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
                            join S_WF_InsFlow on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
                            join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
                            join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
                            group by S_WF_InsDefFlow.CategoryID
                            ";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            dt.Columns["eCount"].ColumnName = "Count";
            int total = 0;
            foreach (DataRow row in dt.Rows)
            {
                total += int.Parse(row["Count"].ToString());
            }
            dt.Rows.Add(total, "");
            return Json(dt, JsonRequestBehavior.AllowGet);

        }      

        #region 改版任务中心
        public ActionResult MyTaskCenter()
        {
            ViewBag.NomalTaskCount = "";
            ViewBag.DesignTaskCount = "";
            return View();
        }       
      
        public JsonResult GetMyUndoList(QueryBuilder qb, string queryTabData)
        {
            List<Dictionary<string, string>> ht = JsonHelper.ToObject<List<Dictionary<string, string>>>(queryTabData);
            var flowCategory = getQueryValue(ht, "FlowCategory");

            string sql = @"select S_WF_InsTaskExec.ID as ID
,InsDefStepID
,S_WF_InsDefFlow.Name as DefFlowName,
S_WF_InsDefStep.Name as DefStepName
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTaskExec.Type as TaskExecType
,S_WF_InsTask.ID as TaskID
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,S_WF_InsTask.Urgency
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserIDs
,SendTaskUserNames
,S_WF_InsTaskExec.TaskUserID
,S_WF_InsTaskExec.TaskUserName
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec
join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
";
            sql = string.Format(sql, FormulaHelper.UserID);
            if (!string.IsNullOrEmpty(flowCategory))
                sql += string.Format(" where S_WF_InsFlow.FlowCategory IN ('{0}') ", flowCategory.TrimEnd(',').Replace(",", "','"));

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
        /// <summary>
        /// 已办任务
        /// </summary>
        /// <param name="qb"></param>
        /// <param name="queryData"></param>
        /// <returns></returns>
        public JsonResult GetMyDoneList(QueryBuilder qb, string queryTabData)
        {
            List<Dictionary<string, string>> ht = JsonHelper.ToObject<List<Dictionary<string, string>>>(queryTabData);
            var timeInterval = getQueryValue(ht, "CreateTime");
            var flowCategory = getQueryValue(ht, "FlowCategory");

            string sql = @"
            select S_WF_InsTaskExec.ID as ID
            ,S_WF_InsTaskExec.ID as TaskExecID
            ,S_WF_InsTask.ID as TaskID
            ,S_WF_InsTaskExec.Type as TaskExecType
            ,S_WF_InsTask.InsDefStepID as StepID
            ,S_WF_InsTask.InsFlowID as FlowID
            ,S_WF_InsTask.Type as TaskType
            ,S_WF_InsTask.Urgency
            ,TaskName
            ,TaskCategory
            ,TaskSubCategory
            ,SendTaskUserIDs
            ,SendTaskUserNames
            ,S_WF_InsTaskExec.TaskUserID
            ,S_WF_InsTaskExec.TaskUserName
            ,S_WF_InsTask.Status as Status
            ,S_WF_InsTask.CreateTime as CreateTime
            ,S_WF_InsTaskExec.ExecTime
            ,FormInstanceID
            ,FlowName
            ,FlowCategory
            ,FlowSubCategory
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
            from S_WF_InsTaskExec
            join S_WF_InsTask on ExecTime is not null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (S_WF_InsTaskExec.CreateTime >= {2}'{1}') and S_WF_InsTask.ID=InsTaskID
            join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
            join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
            join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
            left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
            ";

            DateTime? start = null;
            DateTime? end = null;

            bool isHistory = false;//更早（一年前）
            if (!string.IsNullOrEmpty(timeInterval))
            {
                DateTime?[] startEnd = GetStartEnd((TimeInterval)Enum.Parse(typeof(TimeInterval), timeInterval));
                start = startEnd[0];
                end = startEnd[1];

                if (timeInterval == TimeInterval.MoreEarlier.ToString())
                    isHistory = true;
            }
            if (!string.IsNullOrEmpty(flowCategory) || start != null || end != null)
            {
                string where = " where 1=1 ";
                if (!string.IsNullOrEmpty(flowCategory))
                    where += string.Format(" AND (S_WF_InsFlow.FlowCategory IN ('{0}')) ", flowCategory.TrimEnd(',').Replace(",", "','"));

                if (start != null)
                    where += string.Format(" AND (S_WF_InsTaskExec.CreateTime >= '{0}')", Convert.ToDateTime(start).ToString("yyyy-MM-dd"));

                if (end != null)
                {
                    where += string.Format(" AND (S_WF_InsTaskExec.CreateTime <  '{0}')", Convert.ToDateTime(end).ToString("yyyy-MM-dd"));
                }

                sql += where;
            }
            if (isHistory == true)//历史只查五年以内,避免效率问题
                sql = string.Format(sql, FormulaHelper.UserID, DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd"), Config.Constant.IsOracleDb ? "date" : "");
            else
                sql = string.Format(sql, FormulaHelper.UserID, DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"), Config.Constant.IsOracleDb ? "date" : "");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
        public JsonResult GetMyDelegateList(QueryBuilder qb)
        {
            string sql = @"select S_WF_InsTaskExec.ID as ID
,InsDefStepID
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTaskExec.Type as TaskExecType
,S_WF_InsTask.ID as TaskID
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,S_WF_InsTask.Urgency
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserIDs
,SendTaskUserNames
,S_WF_InsTaskExec.TaskUserName
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,S_WF_InsTaskExec.ExecUserID
,S_WF_InsTaskExec.ExecUserName
,S_WF_InsTaskExec.ExecTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec
join S_WF_InsTask on TaskUserID = '{0}' and ExecUserID <> '{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
        public string getQueryValue(List<Dictionary<string, string>> ht, string queryField)
        {
            if (ht == null) return "";
            foreach (var item in ht)
            {
                if (item["queryfield"] == queryField)
                    return item["value"];
            }
            return "";
        }
        public static string GetDoneFlowCatalogyEnumData()
        {
            string sql = string.Format(@" select FlowCategory,Count(*) Quantity
 FROM S_WF_InsTaskExec inner join 
 S_WF_InsFlow ON S_WF_InsTaskExec.InsFlowID = S_WF_InsFlow.ID
 where ExecTime is not null  AND ExecUserID='{0}'
 GROUP BY FlowCategory", FormulaHelper.UserID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            Hashtable ht = new Hashtable();
            ht["enumkey"] = "FlowCategory";
            ht["title"] = "流程分类";
            ht["queryfield"] = "FlowCategory";
            ArrayList alItems = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["FlowCategory"].ToString())) continue;
                var quantity = row["Quantity"].ToString();
                var text = row["FlowCategory"].ToString() + string.Format("({0})", quantity);
                var itemHs = new Hashtable();
                itemHs["value"] = row["FlowCategory"].ToString();
                itemHs["text"] = text;
                itemHs["radio"] = "F";
                alItems.Add(itemHs);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }
        public static string GetUndoFlowCatalogyEnumData()
        {
            string sql = string.Format(@" select FlowCategory,Count(*) Quantity
from S_WF_InsTaskExec
join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
 GROUP BY FlowCategory", FormulaHelper.UserID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            Hashtable ht = new Hashtable();
            ht["enumkey"] = "FlowCategory";
            ht["title"] = "流程分类";
            ht["queryfield"] = "FlowCategory";
            ArrayList alItems = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["FlowCategory"].ToString())) continue;
                var quantity = row["Quantity"].ToString();
                var text = row["FlowCategory"].ToString() + string.Format("({0})", quantity);
                var itemHs = new Hashtable();
                itemHs["value"] = row["FlowCategory"].ToString();
                itemHs["text"] = text;
                itemHs["radio"] = "F";
                alItems.Add(itemHs);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }
        public static string GetTimeIntervalEnumData()
        {
            Hashtable ht = new Hashtable();
            ht["enumkey"] = "CreateTime";
            ht["title"] = "任务时间";
            ht["queryfield"] = "CreateTime";
            ArrayList alItems = new ArrayList();

            NameValueCollection nvc = GetNVCFromEnumValue(typeof(TimeInterval));
            foreach (string key in nvc)
            {
                Hashtable htItem = new Hashtable();
                htItem["value"] = nvc[key];
                htItem["text"] = key;
                htItem["radio"] = "T";
                alItems.Add(htItem);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }
        #endregion

        #region 私有方法

        private List<Dictionary<string, object>> GetCategorys()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string sql = "select * from S_M_Category";
            DataTable dt = SQLHelper.CreateSqlHelper("Base").ExecuteDataTable(sql);
            if (dt != null)
            {
                DataRow[] drs = dt.Select("ParentID='0'");
                foreach (DataRow dr in drs)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["value"] = Convert.ToString(dr["Code"]);
                    dic["text"] = Convert.ToString(dr["Name"]);
                    dic["name"] = Convert.ToString(dr["Name"]);
                    dic["iconCls"] = "Module";
                    dic["category"] = Convert.ToString(dr["ID"]);
                    dic["type"] = "Category";
                    list.Add(dic);
                }
            }
            return list;
        }

        /// <summary>
        /// 读取枚举的Description
        /// </summary>
        /// <param name="source">枚举Type</param>
        /// <param name="enumName">需要读取</param>
        /// <returns></returns>
        private static string GetEnumDesc(Type source, string enumName)
        {
            FieldInfo[] fieldinfo = source.GetFields();
            foreach (FieldInfo item in fieldinfo)
            {
                if (item.Name != enumName) continue;
                Object[] obj = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (obj.Length == 0) continue;
                DescriptionAttribute desc = (DescriptionAttribute)obj[0];
                return desc.Description;
            }
            return enumName;
        }

        private DateTime?[] GetStartEnd(TimeInterval ti)
        {
            DateTime? start = null, end = DateTime.Today.AddDays(1);
            switch (ti)
            {
                case TimeInterval.LastThreeDays:
                    start = DateTime.Today.AddDays(-2);
                    break;
                case TimeInterval.LastWeek:
                    start = DateTime.Today.AddDays(-6);
                    break;
                case TimeInterval.LastTwoWeeks:
                    start = DateTime.Today.AddDays(-13);
                    break;
                case TimeInterval.LastMonth:
                    start = DateTime.Today.AddMonths(-1);
                    break;
                case TimeInterval.LastYear:
                    start = DateTime.Today.AddYears(-1);
                    break;
                case TimeInterval.MoreEarlier:
                    start = null;
                    end = DateTime.Today.AddYears(-1);
                    break;
            }
            DateTime?[] startEnd = new DateTime?[2];
            startEnd[0] = start;
            startEnd[1] = end;
            return startEnd;
        }

        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        private static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    nvc.Add(strText, field.Name);
                }
            }
            return nvc;
        }

        #endregion
    }

    public enum TimeInterval
    {
        [Description("最近三天")]
        LastThreeDays,
        [Description("最近一周")]
        LastWeek,
        [Description("最近两周")]
        LastTwoWeeks,
        [Description("一个月内")]
        LastMonth,
        [Description("一年内")]
        LastYear,
        [Description("更早(一年前)")]
        MoreEarlier,
    }

}
