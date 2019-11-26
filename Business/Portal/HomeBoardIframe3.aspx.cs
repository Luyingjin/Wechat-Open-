using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using Formula;
using Config;

namespace Portal
{
    public partial class HomeBoardIframe3 : BasePage
    {
        protected string UserId = string.Empty;
        protected string UserName = string.Empty;
        protected string SystemName = string.Empty;
        protected DateTime CurrentTime = DateTime.Now;
        protected string jsonMenuData = string.Empty;
        protected string HomePageUrl = "/portal/HomeBoard.aspx?BlockType=Portal";    

      
        
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

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = FormulaHelper.GetUserInfo();
            UserId = user.UserID;
            UserName = user.UserName;
            SystemName = user.Code;

            //this.HomePageUrl = SQLHelper.CreateSqlHelper("Admin").ExecuteScalar("select ");
            this.HomePageUrl = string.Format(HomePageUrl, "PRO0001I");
            if (!IsPostBack)
            {
                string menuRootID = AuthHelper.getUserMenuRootID(user.UserID);
                DataTable dt = AuthHelper.getUserMenu(user.UserID, menuRootID);
                InitMenu(dt);
            }

        }    
      

        private void InitMenu(DataTable meunTable)
        {
            this.meunInfo.InnerHtml = "";
            int i = 0;
            foreach (DataRow item in meunTable.Rows)
            {
                this.meunInfo.InnerHtml += "<li><a href=\"javascript:void(0);\" amenu=\"T\" id=\"" + item["ID"].ToString() + "\" onclick=\"menuclick(\'" + item["Name"].ToString() + "\',\'" + item["Url"].ToString() + "',true,this);return false;\"><span></span>"
                        + item["Name"].ToString() + " </a></li>";
                i++;
            }
        }
    }
}