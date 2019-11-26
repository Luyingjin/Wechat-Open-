using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Config;
using System.Data;
using Formula.Helper;
using Formula;
using System.DirectoryServices;


namespace Portal
{
    public partial class LoginBegin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string isLogin = this.Request.Form["Todo"];
            if (!String.IsNullOrEmpty(isLogin))
            {
                string state = string.Empty;
                string desc = string.Empty;
                string from = string.Empty;
                string sysName = this.Request["LoginName"];
                string Password = this.Request["Password"];
                string domain = this.Request["Domain"];

                // UserLogin(sysName, Password);
                int ErrPwdLockTime = 10;
                int ErrPwdTimes = 3;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrPwdLockTime"]))
                    ErrPwdLockTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ErrPwdLockTime"]);
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrPwdTimes"]))
                    ErrPwdTimes = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ErrPwdTimes"]);
                try
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_A_User where Code='{0}' and IsDeleted='0'", sysName));

                    //锁定时间
                    if (dt.Rows.Count > 0)
                    {
                        string strErrTime = dt.Rows[0]["ErrorTime"].ToString();
                        if (strErrTime != "" && DateTime.Parse(strErrTime).AddMinutes(ErrPwdLockTime) < DateTime.Now)
                        {
                            sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorTime=null,ErrorCount=0 where Code='{0}'", sysName));
                            dt.Rows[0]["ErrorCount"] = 0;
                        }
                    }

                    if (dt.Rows.Count == 0)
                    {
                        state = "F";
                        desc = "用户名错误";
                        from = "";
                    }
                    else if (dt.Rows[0]["ErrorCount"].ToString() != "" && Convert.ToInt32(dt.Rows[0]["ErrorCount"]) >= ErrPwdTimes)
                    {
                        state = "F";
                        desc = string.Format("输入密码错误次数超过{0}次，请联系管理员！", ErrPwdTimes);
                        from = "";
                        if (dt.Rows[0]["ErrorTime"].ToString() == "")
                        {
                            if (Config.Constant.IsOracleDb)
                            {
                                sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorTime=to_date('{1}','yyyy/MM/dd hh24:mi:ss') where Code='{0}'", sysName, DateTime.Now));
                            }
                            else
                            {
                                sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorTime='{1}' where Code='{0}'", sysName, DateTime.Now));
                            }
                        }
                    }
                    else if (dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", sysName, Password), "SHA1")
                        && dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", sysName.ToLower(), Password), "SHA1"))
                    {
                        state = "F";
                        desc = "密码错误！";
                        from = "";
                        sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorCount=ErrorCount+1 where Code='{0}'", sysName));
                    }
                    else
                    {
                        //认证成功
                        System.Web.Security.FormsAuthentication.SetAuthCookie(sysName, false);
                        state = "T";
                        desc = "";
                        from = "";
                        if (Config.Constant.IsOracleDb)
                        {
                            sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set  ErrorCount=0,LastLoginTime=to_date('{1}','yyyy/MM/dd hh24:mi:ss') where Code='{0}'", sysName, DateTime.Now));
                        }
                        else
                        {
                            sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorCount=0,LastLoginTime='{1}' where Code='{0}'", sysName, DateTime.Now));
                        }

                        //清空菜单权限缓存
                        string key = string.Format("{0}_GetResTree_{1}", dt.Rows[0]["ID"], Config.Constant.MenuRooID);
                        CacheHelper.Remove(key);
                    }
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp);
                    state = "F";
                    desc = exp.Message;
                    from = "";
                }
                var response = new
                {
                    State = state,
                    Desc = desc,
                    From = from
                };
                Response.Write(Formula.Helper.JsonHelper.ToJson(response));
                Response.End();
            }
        }
        /// <summary>
        /// 公司内部系统域登陆
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        public bool GoodwayLogin(string loginName, string password)
        {
            bool isExist = false;
            try
            {
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                DirectoryEntry entry = new DirectoryEntry("LDAP://10.10.1.249", @"goodwaysoft\" + loginName, password);
                DirectorySearcher deSearch = new DirectorySearcher(entry);
                deSearch.Filter = "(&(objectCategory=Person)(objectClass=User))";
                SearchResultCollection results = deSearch.FindAll();
                isExist = results.Count > 0;
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);
                isExist = false;
            }
            return isExist;
        }
    }
}