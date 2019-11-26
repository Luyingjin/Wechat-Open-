using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Collections;
using System.Data;
using System.Text;
using Formula;
using Formula.Helper;
namespace Portal
{
    public partial class MenuBlock : BasePage
    {
        protected string UserId = string.Empty;
        protected string SubID = string.Empty;
        protected string SubTitle = string.Empty;

        protected string SubHomeSrc = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["ID"]))
            {
                SubID = Request["ID"];
                var ogAuth = AuthHelper.getMenuByID(SubID);
                SubTitle = ogAuth["Name"].ToString();
                SubHomeSrc = ogAuth["Url"].ToString();
            }

            SetStyleIcons();

            UserId = FormulaHelper.UserID;
        }

        private void SetStyleIcons()
        {
            string key = "menuIconCacheKey";

            object iconCls = CacheHelper.Get(key);
            if (iconCls == null)
            {
                DataTable authList = AuthHelper.getMenuIcon();

                List<string> clsNameLsit = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (DataRow item in authList.Rows)
                {
                    string clsName = GetPageName(item["IconUrl"].ToString());
                    if (!clsNameLsit.Contains(clsName))
                    {
                        sb.Append("." + clsName + "{background-image:url(" + item["IconUrl"].ToString() + ")}\r\n");
                        clsNameLsit.Add(clsName);
                    }
                }
                iconCls = sb;
                CacheHelper.Set(key, iconCls);
            }
            icons.InnerText = iconCls.ToString();

           
        }


        public static string GetPageName(string url)
        {
            string pageName = string.Empty;
            if (!string.IsNullOrEmpty(url) && url.LastIndexOf("/") > -1 && url.LastIndexOf("/") < url.Length)
            {
                string fileFullName = url.Substring(url.LastIndexOf("/") + 1);
                if (fileFullName.LastIndexOf(".") > 0)
                {
                    pageName = fileFullName.Substring(0, fileFullName.LastIndexOf("."));
                }
            }
            return pageName;
        }

    }
}