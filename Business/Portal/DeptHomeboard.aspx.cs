using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Formula;
using Config;
using System.Data;
using Formula.Helper;

namespace Portal
{
    public partial class DeptHomeboard : BasePage
    {
        protected string HtmlCatalog = string.Empty;
        private SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

        protected void Page_Load(object sender, EventArgs e)
        {
            SetHtmlCatalog(this.Request["DeptHomeID"]);
        }

        private void SetHtmlCatalog(string deptHomeID)
        {
            string sql = @"select * from S_I_PublicInformCatalog c 
                            where c.ID in (select CatalogId from S_I_PublicInformation i where c.ID = i.CatalogId and charindex('{0}',i.DeptDoorId) > 0)
                            order by SortIndex";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, deptHomeID));
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["ID"].ToString();
                    string name = DBNull.Value.Equals(dr["CatalogName"]) ? string.Empty : dr["CatalogName"].ToString();
                    HtmlCatalog += string.Format("<li catalogID='{0}'>{1}</li>", id, name);
                }
            }
        }
    }
}