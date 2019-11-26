using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Portal.Frameworks
{
    public partial class Portal : BasePage
    {
        protected string HOMEBOARD_URL ="";
        protected string ENTERPRISE_HOMEBOARD_URL = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string portalType = this.Request["PortalType"];
            if (portalType == "Person")
            {
                HOMEBOARD_URL = "/Base/Desktop/Home/Index";
            }
            else
            {
                switch (System.Configuration.ConfigurationManager.AppSettings["AuthMode"])
                {
                    case "Tree":
                        HOMEBOARD_URL = "HomeBoardIframe.aspx";
                        break;
                    case "Menu":
                        HOMEBOARD_URL = "HomeBoardIframe3.aspx";
                        break;
                }
            }
            switch (System.Configuration.ConfigurationManager.AppSettings["AuthMode"])
            {
                case "Tree":
                    ENTERPRISE_HOMEBOARD_URL = "HomeBoardIframe.aspx";
                    break;
                case "Menu":
                    ENTERPRISE_HOMEBOARD_URL = "HomeBoardIframe3.aspx";
                    break;
            }
        }
    }
}