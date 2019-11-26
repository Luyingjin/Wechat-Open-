using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using Config;
using Base.Logic.Domain;
using System.Transactions;
using Formula.Exceptions;
using Formula.Helper;
using Workflow.Logic;
using System.Configuration;
using Base.Logic.BusinessFacade;
using System.Data;
using System.Text.RegularExpressions;
using Base.Logic.Model.UI.Form;
using System.Text;
using System.IO;
using System.Data.Entity;
using MvcAdapter;

namespace MvcConfig.Areas.UI.Controllers
{
    public class FormController : BaseController, IFlowController
    {
        FlowService _flowService = null;
        FlowService flowService
        {
            get
            {
                if (_flowService == null)
                {
                    _flowService = new FlowService(this, Request["FormData"], Request["ID"], Request["TaskExecID"]);
                }
                return _flowService;
            }
        }

        public ActionResult PageView()
        {
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            DataTable formDataDT = null;
            if (Request["IsPreView"] == "True")
            {
                formDataDT = new DataTable();
                var row = formDataDT.NewRow();
                formDataDT.Rows.Add(row);
            }
            else
            {
                formDataDT = GetModel(Request["ID"]);
            }
            ViewBag.FormData = JsonHelper.ToJson(formDataDT).Trim('[', ']');
            ViewBag.FormHtml = uiFO.CreateFormHtml(Request["TmplCode"], formDataDT.Rows[0]);
            ViewBag.HiddenHtml = uiFO.CreateFormHiddenHtml(Request["TmplCode"]);
            ViewBag.Script = uiFO.CreateFormScript(Request["TmplCode"]);

            return View();
        }

        #region 导出HTML

        public FileResult ExportHtml()
        {
            string tmplCode = Request["TmplCode"];
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            StringBuilder html = new StringBuilder(@"
<div class='mini-toolbar' id='btnDiv' style='width: 100%; position: fixed; top: 0; left: 0; z-index: 100;'>
        <table>
            <tr>
                <td>
                    <a id='btnSave' class='mini-button' plain='true' iconcls='icon-save' onclick='save();'>保存</a>
                    <a class='mini-button' plain='true' iconcls='icon-cancel' onclick='closeWindow()'>取消</a>
                </td>
                <td id='btnRight'>
                </td>
            </tr>
        </table>
</div>
<form id='dataForm' method='post' align='center' style='top: 30px; position: relative;'>
<input name='ID' class='mini-hidden' />
");

            html.AppendLine();
            html.Append(uiFO.CreateFormHiddenHtml(tmplCode));
            html.Append("\n<div class='formDiv'>");
            html.AppendLine();
            html.Append(uiFO.CreateFormHtml(tmplCode, null));
            html.AppendLine();
            html.Append(@"
</div>
</form>
<script type='text/javascript'>
");
            html.Append(uiFO.CreateFormScript(tmplCode, true));
            html.AppendLine();
            html.Append("</script>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(html.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", Request["TmplCode"] + ".cshtml");

        }





        #endregion

        #region 保存表单

        public JsonResult Save()
        {
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);

            StringBuilder sql = new StringBuilder();
            string formID = "";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);

            string formData = Request["FormData"];
            Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
            var oldDic = JsonHelper.ToObject<Dictionary<string, string>>(Request["OldFormData"]);
            formID = dic["ID"];

            //校验唯一
            ValidateUnique(formInfo, dic, formID);

            if (sqlHelper.ExecuteScalar(string.Format("select count(1) from {0} where ID='{1}'", formInfo.TableName, formID)).ToString() == "0")
            {
                if (dic.ContainsKey("SerialNumber")) //重新获取并应用流水号
                    dic["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                sql.Append(dic.CreateInsertSql(formInfo.ConnName, formInfo.TableName, formID));
            }
            else
            {
                DataTable dt = GetModel(formID);
                string json = JsonHelper.ToJson(dt);
                var currentDic = JsonHelper.ToObject<List<Dictionary<string, string>>>(json)[0];
                sql.Append(dic.CreateUpdateSql(oldDic, currentDic, formInfo.ConnName, formInfo.TableName, formID));
            }

            #region 保存子表

            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
            foreach (var item in items)
            {
                if (item.ItemType != "SubTable")
                    continue;

                if (!dic.ContainsKey(item.Code) || string.IsNullOrEmpty(dic[item.Code]))
                    continue;
                var subTableDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic[item.Code]);
                var ids = subTableDataList.Where(c => c.ContainsKey("ID") && !string.IsNullOrEmpty(c["ID"])).Select(c => c["ID"]).ToList();
                //获取全部子表项ID
                var oldIds = sqlHelper.ExecuteDataTable(string.Format("select ID from {0}_{1} where {0}ID='{2}'", formInfo.TableName, item.Code, formID))
                   .AsEnumerable().Select(c => c.Field<string>("ID")).ToList();
                string notExistIDs = string.Join("','", oldIds.Where(c => !ids.Contains(c)).ToArray());
                //删除已经不存在的ID
                sql.AppendFormat("\n delete from {0}_{1} where ID in('{2}')", formInfo.TableName, item.Code, notExistIDs);

                int index = 0;
                foreach (var subItem in subTableDataList)
                {
                    subItem[formInfo.TableName + "ID"] = formID;//父表ID
                    subItem["SortIndex"] = index++.ToString();
                    sql.Append("\n");

                    if (subItem.ContainsKey("ID") && !string.IsNullOrEmpty(subItem["ID"]))
                    {
                        sql.Append(subItem.CreateUpdateSql(formInfo.ConnName, formInfo.TableName + "_" + item.Code, subItem["ID"]));
                    }
                    else
                    {
                        sql.Append(subItem.CreateInsertSql(formInfo.ConnName, formInfo.TableName + "_" + item.Code, FormulaHelper.CreateGuid()));
                    }
                }
            }


            #endregion

            if (sql.ToString() != "")
                sqlHelper.ExecuteNonQuery(sql.ToString());

            DataTable formDataDT = GetModel(formID);
            return Json(JsonHelper.ToJson(formDataDT).Trim('[', ']'));
        }

        #endregion

        #region 流程相关

        public virtual void UnExecTaskExec(string taskExecID)
        {
            FlowFO flowFO = new FlowFO();
            flowFO.UnExecTask(taskExecID);           
        }


        public virtual bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            FlowFO flowFO = new FlowFO();
            //流程表单定义的流程逻辑
            ExecFlowLogic(routing.Code, taskExec.S_WF_InsFlow.FormInstanceID);

            return flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);
        }

        protected virtual string GetRoutingIconCls(string routingCode)
        {
            return flowService.GetRoutingIconCls(routingCode);
        }

        #region Delete方法

        public JsonResult Delete()
        {
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            return Json("");
        }

        #endregion

        #region 委托、传阅、加签

        #region DelegateTask

        public virtual JsonResult DelegateTask()
        {
            flowService.DelegateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region AskTask

        public virtual JsonResult AskTask()
        {
            flowService.AskTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region WithdrawAskTask

        public virtual JsonResult WithdrawAskTask()
        {
            flowService.WithdrawAskTask(Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region CirculateTask

        public virtual JsonResult CirculateTask()
        {
            flowService.CirculateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region ViewTask

        public virtual JsonResult ViewTask()
        {
            flowService.ViewTask(Request["TaskExecID"], Request["ExecComment"]);
            return Json("");
        }


        #endregion

        #endregion

        #region DoBack

        public virtual JsonResult DoBack(string taskExecID, string routingID, string execComment)
        {
            flowService.DoBack(taskExecID, routingID, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirst

        public virtual JsonResult DoBackFirst(string taskExecId, string execComment)
        {
            flowService.DoBackFirst(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirstReturn

        public virtual JsonResult DoBackFirstReturn(string taskExecId, string execComment)
        {
            flowService.DoBackFirstReturn(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region Submit

        public virtual JsonResult Submit()
        {
            string id = flowService.Submit(GetQueryString("ID"), Request["RoutingID"], Request["TaskExecID"], Request["NextExecUserIDs"], Request["NextExecUserIDsGroup"], Request["nextExecRoleIDs"], Request["nextExecOrgIDs"], Request["ExecComment"]);
            return Json(new { ID = id });
        }

        #endregion

        #region DeleteFlow 其实为撤销方法

        public virtual JsonResult DeleteFlow()
        {
            flowService.DeleteFlow(GetQueryString("ID"), Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region GetFormControlInfo
        /// <summary>
        /// 获取表单控制信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFormControlInfo()
        {
            var dic = flowService.GetFormControlInfo(Request["TaskExecID"]);
            return Json(dic);
        }

        #endregion

        #region GetFlowButtons

        public virtual JsonResult GetFlowButtons()
        {
            var btnList = flowService.JsonGetFlowButtons(Request["ID"], Request["TaskExecID"]);
            return Json(btnList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetUser

        public string GetUserIDs(string roleIDs, string orgIDs)
        {
            return flowService.GetUserIDs(roleIDs, orgIDs);
        }

        public string UserNames(string userIDs)
        {
            return flowService.UserNames(userIDs);
        }

        #endregion

        public JsonResult GetBusLeftTaskList()
        {
            var obj = flowService.GetBusLeftTaskList(Request["ID"], Request["TaskExecID"]);
            return Json(obj);
        }

        #endregion

        #region 私有方法

        #region 获取表单数据

        private DataTable GetModel(string id)
        {
            flowService.SetTaskFirstViewTime(Request["TaskExecID"]);

            var uiFO = FormulaHelper.CreateFO<UIFO>();

            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);

            string sql = string.Format("select * from {0} where ID='{1}'", formInfo.TableName, id);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            if (dt.Rows.Count == 0)
            {
                #region 新对象默认值

                var row = dt.NewRow();
                row["ID"] = string.IsNullOrEmpty(id) ? FormulaHelper.CreateGuid() : id;
                dt.Rows.Add(row);

                List<DataRow> defaultValueRows = uiFO.GetDefaultValueRows(formInfo.DefaultValueSettings);
                var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
                foreach (DataColumn col in dt.Columns)
                {
                    var item = items.Where(c => c.Code == col.ColumnName).SingleOrDefault();
                    if (item == null || string.IsNullOrEmpty(item.DefaultValue))
                        continue;

                    if (item.ItemType == "ButtonEdit" && !string.IsNullOrEmpty(item.DefaultValue) && item.DefaultValue.Split(',').Count() == 2)
                    {
                        string v1 = uiFO.GetDefaultValue(item.Code, item.DefaultValue.Split(',')[0], defaultValueRows);
                        string v2 = uiFO.GetDefaultValue(item.Code, item.DefaultValue.Split(',')[1], defaultValueRows);
                        if (!string.IsNullOrEmpty(v1))
                            row[col] = v1;
                        if (!string.IsNullOrEmpty(v2))
                            row[col.ColumnName + "Name"] = v2;
                    }
                    else
                    {
                        string v = uiFO.GetDefaultValue(item.Code, item.DefaultValue, defaultValueRows);
                        if (!string.IsNullOrEmpty(v))
                            row[col] = v;
                    }
                }

                #endregion

                //设置默认流水号
                if (dt.Columns.Contains("SerialNumber") && !string.IsNullOrEmpty(formInfo.SerialNumberSettings))
                {
                    dt.Rows[0]["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, false, dt.Rows[0], null);
                }
            }
            else //获取子表数据
            {
                #region 获取子表数据

                var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
                foreach (var item in items)
                {
                    if (item.ItemType != "SubTable")
                        continue;

                    sql = string.Format("select * from {0}_{1} where {0}ID='{2}' order by SortIndex", formInfo.TableName, item.Code, id);

                    DataTable dtSubTable = sqlHelper.ExecuteDataTable(sql);

                    if (!dt.Columns.Contains(item.Code))
                        dt.Columns.Add(item.Code);
                    dt.Rows[0][item.Code] = JsonHelper.ToJson(dtSubTable);
                }

                #endregion
            }

            return dt;
        }

        #endregion

        #region 获取流水号

        private string GetSerialNumber(string formCode, string SerialNumberSettings, bool applySerialNumber, DataRow row = null, Dictionary<string, string> dic = null)
        {
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            var serialNumberDic = JsonHelper.ToObject(SerialNumberSettings);
            string tmpl = serialNumberDic["Tmpl"].ToString();
            string resetRule = serialNumberDic["ResetRule"].ToString();
            string CategoryCode = "";
            string SubCategoryCode = "";
            string OrderNumCode = "";
            string PrjCode = "";
            string OrgCode = "";
            string UserCode = "";

            if (serialNumberDic.ContainsKey("CategoryCode"))
                CategoryCode = uiFO.ReplaceString(serialNumberDic["CategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("SubCategoryCode"))
                SubCategoryCode = uiFO.ReplaceString(serialNumberDic["SubCategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrderNumCode"))
                OrderNumCode = uiFO.ReplaceString(serialNumberDic["OrderNumCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("PrjCode"))
                PrjCode = uiFO.ReplaceString(serialNumberDic["PrjCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrgCode"))
                OrgCode = uiFO.ReplaceString(serialNumberDic["OrgCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("UserCode"))
                UserCode = uiFO.ReplaceString(serialNumberDic["UserCode"].ToString(), row, dic);

            SerialNumberParam param = new SerialNumberParam()
            {
                Code = formCode,
                PrjCode = PrjCode,
                OrgCode = OrgCode,
                UserCode = UserCode,
                CategoryCode = CategoryCode,
                SubCategoryCode = SubCategoryCode,
                OrderNumCode = OrderNumCode
            };

            string SerialNumber = SerialNumberHelper.GetSerialNumberString(tmpl, param, resetRule, applySerialNumber);

            return SerialNumber;
        }

        #endregion

        #region 执行流程逻辑

        private void ExecFlowLogic(string routingCode, string id)
        {
            var uiFO = FormulaHelper.CreateFO<UIFO>();
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            var logicList = JsonHelper.ToObject<List<Dictionary<string, string>>>(formInfo.FlowLogic ?? "[]");
            var logic = logicList.SingleOrDefault(c => c["RoutingCode"] == routingCode);
            if (logic == null)
                return;

            string sql = string.Format("select * from {0} where ID='{1}'", formInfo.TableName, id);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            sql = uiFO.ReplaceString(logic["SQL"], dt.Rows[0]);
            sqlHelper = SQLHelper.CreateSqlHelper(logic["ConnName"]);
            sqlHelper.ExecuteNonQuery(sql);
        }

        #endregion

        #region 校验唯一性

        private void ValidateUnique(S_UI_Form formInfo, Dictionary<string, string> dic, string formID)
        {
            string sql = string.Format("select count(1) from {0} where ID!='{1}'", formInfo.TableName, formID);
            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);

            string fieldName = "";
            foreach (var item in items)
            {
                if (!dic.ContainsKey(item.Code))
                    continue;

                if (item.ItemType == "SubTable")
                {
                    if (!string.IsNullOrEmpty(item.Settings))
                    {
                        var _dic = JsonHelper.ToObject(item.Settings);
                        var subList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());

                        if (subList.Where(c => c.Unique == "true").Count() > 0)
                        {
                            var dataList = JsonHelper.ToList(dic[item.Code]);
                            List<string> tmpList = new List<string>();
                            foreach (var c in dataList)
                                tmpList.Add("");
                            string subFieldName = "";
                            foreach (var subItem in subList.Where(c => c.Unique == "true"))
                            {
                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    tmpList[i] += "_" + (!dataList[i].ContainsKey(subItem.Code) ? "" : dataList[i][subItem.Code].ToString());
                                }
                                subFieldName += subItem.Name + ",";
                            }

                            if (tmpList.Distinct().Count() < tmpList.Count)
                                throw new Exception(string.Format("唯一性校验失败：{0}", subFieldName.Trim(',')));
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Unique))
                        continue;

                    if (item.Unique == "true")
                    {
                        sql += string.Format(" and {0}='{1}'", item.Code, dic[item.Code]);
                        fieldName += item.Name + ",";
                    }
                }
            }
            if (fieldName != "")
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                var obj = sqlHelper.ExecuteScalar(sql);
                if (obj.ToString() != "0")
                    throw new Exception(string.Format("唯一性校验失败：{0}", fieldName.Trim(',')));

            }
        }



        #endregion

        #region entities

        protected DbContext baseEntities
        {
            get
            {
                return FormulaHelper.GetEntities<BaseEntities>();
            }
        }

        #endregion

        #endregion

        #region 接口实现

        public void SetFormFlowPhase(string id, string flowPhase, string stepName)
        {
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);

            string sql = string.Format("select * from {0} where 1=2", formInfo.TableName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            sql = "";
            if (dt.Columns.Contains("FlowPhase"))
                sql += string.Format(" update {0} set FlowPhase='{1}' where ID='{2}'", formInfo.TableName, flowPhase, id);
            if (dt.Columns.Contains("StepName"))
                sql += string.Format(" update {0} set StepName='{1}' where ID='{2}'", formInfo.TableName, stepName, id);
            sqlHelper.ExecuteNonQuery(sql);
        }

        public void DeleteForm(string ids)
        {
            string tmplCode = Request["TmplCode"];
            var form = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(form.ConnName);
            string sql = string.Format("delete from {0} where ID in('{1}')", form.TableName, ids.Replace(",", "','"));
            sqlHeler.ExecuteNonQuery(sql);
        }

        public bool ExistForm(string id)
        {
            string tmplCode = Request["TmplCode"];
            var form = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(form.ConnName);
            string sql = string.Format("select count(1) from {0} where ID ='{1}'", form.TableName, id);
            object obj = sqlHeler.ExecuteScalar(sql);
            return Convert.ToInt32(obj) > 0;
        }

        #endregion


    }

}
