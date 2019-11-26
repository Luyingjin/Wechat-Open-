using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.BusinessFacade;
using Formula;
using MvcAdapter;
using Base.Logic.Domain;
using Config;
using System.Text;
using System.IO;
using System.Data;
//using Workflow.Logic.BusinessFacade;
using Formula.Helper;

namespace MvcConfig.Areas.UI.Controllers
{
    public class ListController : BaseController
    {

        public ActionResult PageView(string tmplCode)
        {
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            ViewBag.ListHtml = uiFO.CreateListHtml(tmplCode);
            ViewBag.Script = uiFO.CreateListScript(tmplCode);
            return View();
        }

        public JsonResult GetList(string tmplCode, QueryBuilder qb)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            string sql = uiFO.ReplaceString(listDef.SQL);

            DataTable dtTmpl = sqlHeler.ExecuteDataTable(string.Format("select * from ({0}) as T where 1=2", sql));

            //地址栏参数作为查询条件
            foreach (string key in Request.QueryString.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                if ("ID,FullID,FULLID,TmplCode,IsPreView,_winid,_t".Split(',').Contains(key) || key.StartsWith("$"))
                    continue;
                if (dtTmpl.Columns.Contains(key))
                    qb.Add(key, QueryMethod.Equal, Request[key], "Group1", null);
            }

            var data = sqlHeler.ExecuteGridData(sql, qb);

            #region 计算汇总

            var fields = JsonHelper.ToList(listDef.LayoutField);
            StringBuilder sb = new StringBuilder();
            foreach (var field in fields)
            {
                if (field.ContainsKey("Settings") == false)
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("Collect") == false || settings["Collect"].ToString() == "")
                    continue;
                sb.AppendFormat(",{0}={1}({0})", field["field"], settings["Collect"]);
                if (settings["Collect"].ToString() == "sum")
                    data.sumData.Add(field["field"].ToString(), null);
                else
                    data.avgData.Add(field["field"].ToString(), null);
            }
            if (sb.Length > 0)
            {
                string collectSql = string.Format("select {0} from (select * from ({1}) as tb where 1=1 {2}) as T"
                    , sb.ToString().Trim(',')
                    , sql
                    , qb.GetWhereString(false) + FormulaHelper.CreateAuthDataFilter().GetWhereString(false)
                    );
                DataTable dtCollect = sqlHeler.ExecuteDataTable(collectSql);

                foreach (DataColumn col in dtCollect.Columns)
                {
                    if (data.sumData.ContainsKey(col.ColumnName))
                        data.sumData[col.ColumnName] = dtCollect.Rows[0][col];
                    else
                        data.avgData[col.ColumnName] = dtCollect.Rows[0][col];
                }
            }

            #endregion

            return Json(data);
        }

        public JsonResult Delete(string tmplCode, string listIDs)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);
            string sql = string.Format("delete from {0} where ID in('{1}')", listDef.TableNames.Split(',')[0], listIDs.Replace(",", "','"));
            sqlHeler.ExecuteNonQuery(sql);

            //FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
            //foreach (string id in listIDs.Split(','))
            //{
            //    flowFO.DeleteFlowByFormInstanceID(id);
            //}

            return Json("");
        }

        #region 导出HTML

        public FileResult ExportHtml()
        {
            string tmplCode = Request["TmplCode"];
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            StringBuilder html = new StringBuilder();
            html.Append(uiFO.CreateListHtml(tmplCode));
            html.AppendLine();
            html.Append("<script type='text/javascript'>");
            html.Append(uiFO.CreateListScript(tmplCode, true));
            html.AppendLine();
            html.Append("</script>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(html.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", Request["TmplCode"] + ".cshtml");

        }





        #endregion


        #region entities

        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                return FormulaHelper.GetEntities<BaseEntities>();
            }
        }

        #endregion

    }
}
