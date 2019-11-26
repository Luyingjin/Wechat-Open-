namespace Formula
{
    using System;
    using System.Text;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public class LogWriter
    {
        protected static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("Log");
        protected static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("Exception");

        /// <summary>
        /// 记录普通信息日志
        /// </summary>
        /// <param name="info">建议信息格式：方法名-内容-开始/结束</param>
        public static void Info(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        /// <summary>
        /// 记录错误异常日志
        /// </summary>
        /// <param name="info">附加信息</param>
        public static void Error(string info)
        {
            Error(null, info);
        }

        /// <summary>
        /// 记录错误异常日志
        /// </summary>
        /// <param name="info">附加信息</param>
        /// <param name="ex">错误</param>
        public static void Error(Exception ex, string info = "")
        {
            if (!string.IsNullOrEmpty(info) && ex == null)
            {
                logerror.ErrorFormat("【附加信息】：{0}<br>", new object[] { info });
            }
            else if (!string.IsNullOrEmpty(info) && ex != null)
            {
                string errorMsg = BeautyErrorMsg(ex);
                logerror.ErrorFormat("【附加信息】：{0}<br>{1}", new object[] { info, errorMsg });
            }
            else if (string.IsNullOrEmpty(info) && ex != null)
            {
                string errorMsg = BeautyErrorMsg(ex);
                logerror.Error(errorMsg);
            }
        }

        /// <summary>
        /// 美化错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>错误信息</returns>
        private static string BeautyErrorMsg(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("【异常信息】：{0} <br>", ex.Message.Replace("\r\n", " "));
            if (ex.TargetSite != null) sb.AppendFormat("【异常方法】：{0}（{1}）<br>", ex.TargetSite.Name, ex.TargetSite.DeclaringType.FullName);
            sb.AppendFormat("【异常类型】：{0} <br>", ex.GetType().Name);
            if (ex.StackTrace != null) sb.AppendFormat("【堆栈调用】：{0} <br>", ex.StackTrace.Trim());
            if (ex.InnerException != null)
            {
                sb.Append(BeautyInnerExceptionMsg(ex.InnerException));
            }

            if (System.Web.HttpContext.Current != null)
            {
                var user = System.Web.HttpContext.Current.User.Identity.Name;
                var url = System.Web.HttpContext.Current.Request.Url;
                var IpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                IpAddress = IpAddress == "::1" ? "127.0.0.1" : IpAddress;
                sb.AppendFormat("【当前用户】：{0} <br>", user == "" ? "匿名用户" : user);
                sb.AppendFormat("【请求地址】：{0} {1} <br>", url, IpAddress);

                var forms = System.Web.HttpContext.Current.Request.Form;
                var sbForm = new StringBuilder();
                foreach (var key in forms.AllKeys)
                {
                    sbForm.AppendFormat("{0} = {1}<br>", key, forms[key]);
                }
                sb.AppendFormat("【表单参数】：{0} <br>", sbForm.ToString() == "" ? "无" : sbForm.ToString());
            }
            var errorMsg = sb.ToString();
            errorMsg = errorMsg.Replace("\r\n", "<br>");
            errorMsg = errorMsg.Replace("位置", "<strong style=\"color:red\">位置</strong>");
            errorMsg = errorMsg.Replace("行号", "　<strong style=\"color:red\">行号</strong>");
            return errorMsg;
        }

        private static string BeautyInnerExceptionMsg(Exception ex)
        {
            var sb = new StringBuilder();

            // 递归输出内部错误
            if (ex.InnerException != null)
            {
                sb.Append(BeautyInnerExceptionMsg(ex.InnerException));
            }

            sb.AppendFormat("【内部异常信息】：{0} <br>", ex.Message.Replace("\r\n", " "));
            if (ex.TargetSite != null) sb.AppendFormat("【内部异常方法】：{0}（{1}）<br>", ex.TargetSite.Name, ex.TargetSite.DeclaringType.FullName);
            sb.AppendFormat("【内部异常类型】：{0} <br>", ex.GetType().Name);
            if (ex.StackTrace != null) sb.AppendFormat("【内部堆栈调用】：{0} <br>", ex.StackTrace.Trim());

            return sb.ToString();
        }
    }
}
