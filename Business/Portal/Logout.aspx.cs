using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Formula.Helper;
using Formula;
using log4net;

namespace Portal
{
    public partial class Logout : BasePage
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (this.Request["RequestAction"] == "Logout")
            {
                var user = FormulaHelper.GetUserInfo();
                LogWriter.Info(string.Format("提示: 用户 {0}[{1}] 于 {2} 退出系统！", user.UserName, user.Code, DateTime.Now.ToString()));
                System.Web.Security.FormsAuthentication.SignOut();
            }
        }

    }
}