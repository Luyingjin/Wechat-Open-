using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatTask.Common
{
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
                logerror.ErrorFormat("【异常信息】：{0}", new object[] { info });
            }
            else if (!string.IsNullOrEmpty(info) && ex != null)
            {
                string errorMsg = BeautyErrorMsg(ex);
                logerror.ErrorFormat(@"【异常说明】：{0}
【异常信息】：{1}", new object[] { info, errorMsg });
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
            sb.AppendFormat("【异常信息】：{0} ", ex.Message);
            if (ex.TargetSite != null) sb.AppendFormat("【异常方法】：{0}（{1}）", ex.TargetSite.Name, ex.TargetSite.DeclaringType.FullName);
            sb.AppendFormat("【异常类型】：{0} ", ex.GetType().Name);
            if (ex.StackTrace != null) sb.AppendFormat("【堆栈调用】：{0} ", ex.StackTrace.Trim());
            if (ex.InnerException != null)
            {
                sb.Append(BeautyInnerExceptionMsg(ex.InnerException));
            }
            return sb.ToString();
        }

        private static string BeautyInnerExceptionMsg(Exception ex)
        {
            var sb = new StringBuilder();

            // 递归输出内部错误
            if (ex.InnerException != null)
            {
                sb.Append(BeautyInnerExceptionMsg(ex.InnerException));
            }

            sb.AppendFormat("【内部异常信息】：{0}", ex.Message);
            if (ex.TargetSite != null) sb.AppendFormat("【内部异常方法】：{0}（{1}）", ex.TargetSite.Name, ex.TargetSite.DeclaringType.FullName);
            sb.AppendFormat("【内部异常类型】：{0} ", ex.GetType().Name);
            if (ex.StackTrace != null) sb.AppendFormat("【内部堆栈调用】：{0} ", ex.StackTrace.Trim());

            return sb.ToString();
        }
    }
}
