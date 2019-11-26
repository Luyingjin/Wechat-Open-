using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Handlers;
using System.Text;
using Config;
using Formula;

namespace Portal.Door
{
	/// <summary>
	/// HomeBoardNew 的摘要说明。
	/// </summary>
    public class BaseHomeBoard : BasePage
	{
        public string Html = "";
		public string LayoutType="";
        public string UserID = FormulaHelper.UserID;
        public string UserName = FormulaHelper.GetUserInfo().UserName;
        public string TemplateID = "";
        public string BaseType = "Portal";

		private void Page_Load(object sender, System.EventArgs e)
		{
            if (this.Request["BlockType"] != null)
                this.BaseType = this.Request["BlockType"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable("select ID from S_P_DoorBaseTemplate where ID='" + this.Request["TemplateId"] + "'");
            
            if (dt!=null && dt.Rows.Count > 0) {
                this.TemplateID = dt.Rows[0]["ID"].ToString();
                Html = BaseBlock.GetBlocks(ref LayoutType, "Portal", this.TemplateID);
                if (Html == "F")
                    Html = StaticHTML();
            }
		}
        /// <summary>
        /// 异常界面
        /// </summary>
        /// <returns></returns>
        private string StaticHTML()
        {
            return "<br><br><span style='width:100%;align:center'><b>当前用户无主页配置！</b></span>";
        }

    }
}
