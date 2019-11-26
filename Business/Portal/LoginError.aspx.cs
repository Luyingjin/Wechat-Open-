using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal
{
    public partial class LoginError : System.Web.UI.Page
    {
        protected string ErrorUrl = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer != null)
                this.ErrorUrl = Request.UrlReferrer.ToString();
        }
    }
}