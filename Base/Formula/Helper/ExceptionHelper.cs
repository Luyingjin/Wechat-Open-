using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Formula.Exceptions;

namespace Formula.Helper
{
    public class ExceptionHelper
    {
        //private static string throwException = System.Configuration.ConfigurationManager.AppSettings["ThrowException"];

        public static void DealException()
        {
            try
            {
                HttpContext context = HttpContext.Current;

                Exception objErr = context.Server.GetLastError().GetBaseException();

                ////如果不抛出异常则清理异常
                //if (throwException != "True")
                //    context.Server.ClearError();
                context.Server.ClearError();
                DealException(objErr);
            }
            catch { }
        }

        public static void DealException(Exception objErr)
        {
            ExceptionPolicy.HandleException(objErr, Constant.ExceptionPolicyName);
        }


        public static void DealExceptionAndRedirect()
        {
            HttpContext context = HttpContext.Current;
            Exception objErr = context.Server.GetLastError().GetBaseException();


            if (objErr is BusinessException || objErr is BusinessValidationException)
            {
                context.Server.ClearError();
                HttpContext.Current.Response.Write(string.Format("<script>alert('{0}');</script>", objErr.Message));
                return;
            }

            DealExceptionAndRedirect(objErr);
        }

        public static void DealExceptionAndRedirect(Exception objErr)
        {

            ExceptionPolicy.HandleException(objErr, Constant.ExceptionPolicyName);


            if (objErr.Message.Contains("/config/DenyAccessError.aspx") || objErr.Message.Contains("/config/Error.aspx"))
                return;



            // 当为安全性异常，定位到权限页面
            if (objErr is System.Security.SecurityException || objErr is AccessControlException)
            {
                System.Web.HttpContext.Current.Response.Redirect("/config/DenyAccessError.aspx");
            }
            else
            {
                string ErrorMessage = objErr.Message;
                string StackMessage = objErr.StackTrace;
                string InnerMessage = GetInnerExceptionMessage(objErr.InnerException);

                string[] s = StackMessage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                StackMessage = string.Join("\r\n", s.Take(3));

                string urlParams = string.Format("ErrorMessage={0}&InnerMessage={1}&StackMessage={2}"
                    , Uri.EscapeUriString(ErrorMessage)
                    , Uri.EscapeUriString(InnerMessage)
                    , Uri.EscapeUriString(StackMessage));

                string url = "/config/Error.aspx";

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ExceptionPage"]))
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["ExceptionPage"];
                }

                System.Web.HttpContext.Current.Response.Redirect(url + "?" + urlParams);
            }

        }

        private static string GetInnerExceptionMessage(Exception ex)
        {
            if (ex == null)
                return "";

            string innerMessage = GetInnerExceptionMessage(ex.InnerException);

            if (innerMessage == null || innerMessage == "")
                return ex.Message;

            return ex.Message + "<br>" + innerMessage;
        }
    }

}
