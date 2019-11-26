using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System.Reflection;

namespace Formula.Helper
{
    public class LogHelper
    {
        public static void Write(string title,string content)
        {
            LogEntry log = new LogEntry();
            log.EventId = 100;
            log.Message = content;
            log.Categories.Add(Constant.LogCategory);
            log.Severity = TraceEventType.Information;
            log.Priority = 5;
            log.Title = title;
            Logger.Write(log);

        }
        public static void Write(string userName, string userId, string actionType, string content, string userIP)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseSystemLog"] == "True")
            {
                LogEntry log = new LogEntry();
                log.EventId = 100;
                log.Message = "操作人：" + userName + "(" + userId + ")    操作时间：" + DateTime.Now.ToString()
                    + "    操作类型：" + actionType + "    描述和结果：" + content;
                //+ "    操作人IP：" + userIP;
                log.Categories.Add(Constant.LogCategory);
                log.Severity = TraceEventType.Information;
                log.Priority = 5;
                Logger.Write(log);
            }
        }
    }
}
