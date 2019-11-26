using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Formula;
using System.Data;


namespace Portal
{
    public partial class HomeBoardIframe : System.Web.UI.Page
    {
        protected string UserId = string.Empty;
        protected string UserName = string.Empty;
        protected string SystemName = string.Empty;
        protected DateTime CurrentTime = DateTime.Now;
        protected string HomePageUrl = "/portal/HomeBoard.aspx?BlockType=Portal";
        public string menuhtml = "";
        public string DeptMenuHtml = "";
        public string DeptMoreMenuHTML = "";
        public string DeptHomeEnabled = (System.Configuration.ConfigurationManager.AppSettings["DeptHomeEnabled"] == "True").ToString().ToLower();

        public string SysColor
        {
            get
            {
                string s = System.Configuration.ConfigurationManager.AppSettings["GlobalSysColor"];
                if (string.IsNullOrEmpty(s) || s == "Default")
                    return "blue";
                return s;
            }
        }

        public string Weekday
        {
            get
            {
                string weekstr = DateTime.Now.DayOfWeek.ToString();
                switch (weekstr)
                {
                    case "Monday": weekstr = "星期一"; break;
                    case "Tuesday": weekstr = "星期二"; break;
                    case "Wednesday": weekstr = "星期三"; break;
                    case "Thursday": weekstr = "星期四"; break;
                    case "Friday": weekstr = "星期五"; break;
                    case "Saturday": weekstr = "星期六"; break;
                    case "Sunday": weekstr = "星期日"; break;
                }
                return weekstr;
            }

        }
        //菜单html模版
        public string templatehtml = "<td noWrap=\"nowrap\"><a href=\"javascript:;\" id=\"{1}\"  onclick=\"onitemclick(this,'{1}','{2}','{3}')\">{0}</a></td>";
        private string DeptTemplateHtml = "<li DeptHomeID='{0}'>{1}</li>";
        private string DeptMoreTemplateHtml = "<li DeptHomeID='{0}'><span>{1}</span></li>";
        protected void Page_Load(object sender, EventArgs e)
        {

            var user = FormulaHelper.GetUserInfo();
            UserId = user.UserID;
            UserName = user.UserName;
            SystemName = user.Code;

            if (!IsPostBack)
            {
                int width = string.IsNullOrEmpty(Request["Width"]) ? 0 : Convert.ToInt32(Request["Width"]);
                int height = string.IsNullOrEmpty(Request["Height"]) ? 0 : Convert.ToInt32(Request["Height"]);

                string menuRootID = AuthHelper.getUserMenuRootID(user.UserID);
                DataTable menuDt = AuthHelper.getUserMenu(user.UserID, menuRootID);
                foreach (DataRow item in menuDt.Rows)
                {

                    menuhtml += string.Format(templatehtml, item["Name"], item["ID"], item["Url"], item["ChildCount"]);
                }

                DataTable manufactureDept = AuthHelper.GetDeptartment();
                foreach (DataRow item in manufactureDept.Rows)
                {
                    DeptMenuHtml += string.Format(DeptTemplateHtml, item["ID"], item["Name"]);
                    DeptMoreMenuHTML += string.Format(DeptMoreTemplateHtml, item["ID"], item["Name"]);
                }
            }

        }
    }
}