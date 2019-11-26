using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Resources;
using Formula.Exceptions;
using System.Configuration;
using System.Web.Configuration;

namespace Formula.HttpModule
{
    public class ScriptModule : IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {

        }

        string loginUrl = "";

        public void Init(HttpApplication context)
        {
            // 捕获全局未处理的异常
            context.Error += new EventHandler(context_Error);

            context.PreSendRequestHeaders += new EventHandler(PreSendRequestHeadersHandler);
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/");
            SystemWebSectionGroup ws = (SystemWebSectionGroup)configuration.GetSectionGroup("system.web");
            loginUrl = ws.Authentication.Forms.LoginUrl.ToLower();

        }
        #endregion

        private void PreSendRequestHeadersHandler(object sender, EventArgs args)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpResponse response = application.Response;

            #region 处理页面权限
            //当前用户的数据级权限          
            //var resList = FormulaHelper.GetService<IResService>().GetRes(url, "Data", user.UserID);
            if (HttpContext.Current != null 
                && HttpContext.Current.User != null
                && HttpContext.Current.User.Identity != null
                && HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (application.Request.Headers["X-Requested-With"] != "XMLHttpRequest") //非Ajax请求
                {
                    string url = HttpContext.Current.Request.Url.PathAndQuery;

                    var resService = FormulaHelper.GetService<IResService>();

                    if (resService.GetRes(url, "Menu,Page").Count() > 0)
                    {
                        var user = FormulaHelper.GetUserInfo();
                        if (user != null)
                        {
                            if (resService.GetRes(url, "Menu,Page", user.UserID).Count() == 0)
                            {
                                response.Clear();
                                application.Server.Transfer("/MvcConfig/AccessError.html");
                            }
                        }
                    }
                }
            }
            #endregion



            if (response.StatusCode == 302)
            {
                if (application.Request.Headers["X-Requested-With"] == "XMLHttpRequest") //Ajax请求
                {
                    if (response.RedirectLocation.Split('?')[0].ToLower() == loginUrl)
                        throw new BusinessException("登录超时，请重新登录");
                }
            }
        }

        private void context_Error(object sender, EventArgs e)
        {
            //此处处理异常
            HttpContext ctx = HttpContext.Current;
            HttpResponse response = ctx.Response;
            HttpRequest request = ctx.Request;

            //获取到HttpUnhandledException异常，这个异常包含一个实际出现的异常
            Exception ex = ctx.Server.GetLastError();
            ////实际发生的异常
            //Exception iex = ex.InnerException;

            // 记入日志
            LogWriter.Error(ex);

            //// 清除异常
            //ctx.Server.ClearError();
        }

    }
}
