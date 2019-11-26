using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Config;
using Config.Logic;

namespace Portal
{
    public partial class UpdatePwd : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string systemName = Request["LoginName"];
                this.SystemName.Text = systemName;
            }
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string systemName = this.SystemName.Text;
            string password = this.Password.Text;

            object obj = sqlHelper.ExecuteScalar(string.Format("select count(1) from S_A_User where Code='{0}' and Password='{1}'", systemName, FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", systemName.ToLower(), password), "SHA1")));

            if (Convert.ToInt32(obj) > 0)
            {//用户名密码正确
                if (this.NewPassword1.Text.Trim() == this.NewPassword2.Text.Trim())
                {
                    if (this.Password.Text.Trim() != this.NewPassword1.Text.Trim())
                    {
                        string newPassword = this.NewPassword1.Text.Trim();

                        string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", systemName, newPassword), "SHA1");
                        string sql = string.Format("update S_A_User set Password='{1}' where Code='{0}'", systemName, pwd);
                        sqlHelper.ExecuteNonQuery(sql);
                        Log(systemName, pwd);
                    }
                    this.LabelError.Text = "密码修改成功！";
                    //string url = "Login.aspx";
                    //Response.Redirect(url);

                }
                else
                    this.LabelError.Text = "新密码重复输入不一致！请重新输入";

            }
            else
                this.LabelError.Text = "用名密码不正确，请重新输入";

        }

        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            string url = "LoginBegin.aspx";
            Response.Redirect(url);
        }


        private static void Log(string systemName, string pwd)
        {
            string sql = @"
INSERT INTO [S_A_AuthLog]
           ([ID],[Operation],[OperationTarget],[RelateData],[ModifyUserName],[ModifyTime],[ClientIP])
     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')
";

            sql = string.Format(sql
              , GuidHelper.CreateGuid()
              , "个人修改密码"
              , systemName
              , pwd
              , ""
              , DateTime.Now
              , GetUserIP()
              );

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(sql);
        }

        private static string GetUserIP()
        {
            string result = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result != System.Web.HttpContext.Current.Request.UserHostAddress)
                result += "," + System.Web.HttpContext.Current.Request.UserHostAddress;

            return result;
        }
    }
}