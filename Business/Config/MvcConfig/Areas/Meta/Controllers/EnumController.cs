using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula;
using Config;
using System.Data;

namespace MvcConfig.Areas.Meta.Controllers
{
    public class EnumController : BaseController
    {
        public JsonResult GetList(QueryBuilder qb)
        {       
            string enumKey = Request["EnumKey"];
            string category = Request["Category"];
            string subCategory = Request["SubCategory"];

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataRow enumDef = FormulaHelper.GetService<IEnumService>().GetEnumDefRow(enumKey);

            GridData gridData = null;

            DataTable dtResult = null;

            if (enumDef["Type"].ToString() == "Normal")
            {

                string sql = string.Format("select Code as id,Code as value,Name as text,Category,SubCategory,SortIndex from S_M_EnumItem where EnumDefID='{0}' {1} {2} order by SortIndex ", enumDef["ID"].ToString(), category, subCategory);
                //gridData = sqlHelper.ExecuteGridData(sql,qb);
                dtResult = sqlHelper.ExecuteDataTable(sql, qb);
            }
            else
            {
                sqlHelper = SQLHelper.CreateSqlHelper(enumDef["ConnName"].ToString());

                string sql = enumDef["Sql"].ToString();

                sql = string.Format("select * from ({0}) table1 where 1=1 {1} {2}"
                    , sql
                    , string.IsNullOrEmpty(category) ? "" : string.Format(" and Category='{0}'", category)
                    , string.IsNullOrEmpty(subCategory) ? "" : string.Format(" and SubCategory='{0}'", subCategory)
                    );

                sql = sql + " " + enumDef["Orderby"].ToString();        

                dtResult = sqlHelper.ExecuteDataTable(sql, qb);
            }


            if (!dtResult.Columns.Contains("ID"))
            {
                dtResult.Columns.Add("ID");
                foreach (DataRow row in dtResult.Rows)
                {
                    row["ID"] = row["value"];
                }
            }
            if (!dtResult.Columns.Contains("Name"))
            {
                dtResult.Columns.Add("Name");
                foreach (DataRow row in dtResult.Rows)
                {
                    row["Name"] = row["text"];
                }
            }
            gridData = new GridData(dtResult);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }
    }
}
