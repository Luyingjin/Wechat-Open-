using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal
{
    public class BasePage : System.Web.UI.Page
    {
        public string SysColor
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["GlobalSysColor"] ?? "Default";
            }
        }
    }
}