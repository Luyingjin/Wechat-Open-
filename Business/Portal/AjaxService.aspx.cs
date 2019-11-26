using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using System.Data;
using Formula;
using Config;
using Formula.Helper;

namespace Portal
{
    public partial class AjaxService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /////////////////
            String methodName = Request["method"];
            Type type = this.GetType();
            MethodInfo method = type.GetMethod(methodName);
            if (method == null) throw new Exception("method is null");

            try
            {
                method.Invoke(this, null);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);

                Hashtable result = new Hashtable();
                result["error"] = -1;
                result["message"] = ex.Message;
                result["stackTrace"] = ex.StackTrace;
                String json = JsonHelper.ToJson(result); //PluSoft.Utils.JSON.Encode(result);
                Response.Clear();
                Response.Write(json);
            }
        }

        public void GetMenuData()
        {

            DataTable dt = AuthHelper.getUserMenu(FormulaHelper.UserID);
            var authList = dt.AsEnumerable();
            string rootKey = string.IsNullOrEmpty(this.Request["RootKey"]) ? null : this.Request["RootKey"];

            if (string.IsNullOrEmpty(rootKey))
                rootKey = AuthHelper.getUserMenuRootID(FormulaHelper.UserID);

            var menuAuths = authList.AsEnumerable().Where(c => c["ParentID"].ToString() == rootKey);

            ArrayList alData = new ArrayList();
            foreach (DataRow item in menuAuths)
            {
                Hashtable node = new Hashtable();
                node["name"] = item["Name"].ToString();
                node["actionkey"] = item["ID"].ToString();
                node["linkurl"] = item["Url"].ToString();

                var subGroup = authList.Where(u => u["ParentID"].ToString() == item["ID"].ToString()).ToList();
                if (subGroup.Count > 0)
                {
                    GetSubData(node, item, authList);
                }
                alData.Add(node);
            }
            string jsonMenuData = JsonHelper.ToJson(alData); //PluSoft.Utils.JSON.Encode(alData);
            Response.Write(jsonMenuData);
        }

        private void GetSubData(Hashtable node, DataRow auth, EnumerableRowCollection<DataRow> authList)
        {
            ArrayList subNodes = new ArrayList();
            var groupList = authList.Where(au => au["ParentID"].ToString() == auth["ID"].ToString()).ToList();
            foreach (var item in groupList)
            {
                Hashtable subNode = new Hashtable();
                subNode["name"] = item["Name"].ToString();
                subNode["actionkey"] = item["ID"].ToString();
                subNode["linkurl"] = item["Url"].ToString();

                var subGroup = authList.Where(u => u["ParentID"].ToString() == item["ID"].ToString()).ToList();
                if (subGroup.Count > 0)
                {
                    GetSubData(subNode, item, authList);
                }
                subNodes.Add(subNode);
            }
            if (subNodes.Count > 0)
            {
                node["children"] = subNodes;
            }
        }

        public void getsubtree()
        {
            string id = Request["id"];
            var auths = AuthHelper.getUserMenu(FormulaHelper.UserID, id, true);

            ArrayList treeData = new ArrayList();
            foreach (DataRow item in auths.Rows)
            {
                Hashtable node = new Hashtable();
                node["text"] = item["Name"].ToString();
                node["id"] = item["ID"].ToString();
                node["iconCls"] = MenuBlock.GetPageName(item["IconUrl"].ToString());
                node["url"] = item["Url"].ToString();
                node["pid"] = item["ParentID"].ToString();

                treeData.Add(node);
            }

            String json = JsonHelper.ToJson(treeData);
            Response.Write(json);
        }

        public void refresh()
        {
            string userID = FormulaHelper.UserID; ;
            ArrayList al = new ArrayList();
            Hashtable result = new Hashtable();
            //result["NewTask"] = GetTaskCount(userID).ToString();
            result["NewMessage"] = GetMsgCount(userID).ToString();
            result["NewAlarm"] = GetAlarmCount().ToString();
            //Response.Write(PluSoft.Utils.JSON.Encode(result));
            Response.Write(JsonHelper.ToJson(result));
        }
        private int GetTaskCount(string userID)
        {
            string sql = @"select count(1) from S_WF_InsTaskExec 
join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings='' or WaitingRoutings is null) and (WaitingSteps='' or WaitingSteps is null) and (S_WF_InsTaskExec.CreateTime >= {1}) and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  ";

            sql = string.Format(sql, FormulaHelper.UserID,
                Config.Constant.IsOracleDb ? string.Format("to_date('{0}','yyyy-MM-dd')", DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd")) : string.Format("'{0}'", DateTime.Now.AddYears(-1))
                );

            object scal = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow).ExecuteScalar(sql);

            int designCount = 0;
            try
            {
                sql = "select count(ID) from S_W_Activity where OwnerUserID='" + userID + "' and State='Create' and ActivityKey in ('Design','Collact','Audit','Approve')";
                object designCountObject = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteScalar(sql);

                designCount = Convert.ToInt32(designCountObject);
            }
            catch (System.Exception ex)
            {}

            return Convert.ToInt32(scal) + designCount;
        }

        private int GetMsgCount(string userID)
        {
            string sql = "select count(ID) from S_S_MsgReceiver where UserID='{0}' and (IsDeleted='0' or isDeleted is null) and FirstViewTime is null";

            object scal = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(string.Format(sql, FormulaHelper.UserID));

            return Convert.ToInt32(scal);
        }
        private int GetAlarmCount()
        {
            string sql = string.Format("SELECT count(ID) FROM S_S_Alarm WHERE (IsDelete IS NULL OR IsDelete<>'T') AND OwnerID='{0}' AND DeadlineTime>='{1}'", FormulaHelper.UserID, DateTime.Now);
            object scal = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(sql);
            return Convert.ToInt32(scal);
        }
    }
}