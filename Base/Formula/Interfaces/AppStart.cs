using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Formula.Logging;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Formula.Interfaces.AppStart), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Formula.Interfaces.AppStart), "Stop")]

namespace Formula.Interfaces
{
    /// <summary>
    /// 在应用程序启动时需要注册的服务
    /// </summary>
    public class AppStart
    {
        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            FormulaHelper.RegisterService<IUserService, UserService>();
            FormulaHelper.RegisterService<IOrgService, OrgService>();
            FormulaHelper.RegisterService<IRoleService, RoleService>();
            FormulaHelper.RegisterService<IEnumService, EnumService>();
            FormulaHelper.RegisterService<IResService, ResService>();
            FormulaHelper.RegisterService<IMessageService, MessageService>();
            FormulaHelper.RegisterService<IWorkflowService, WorkflowService>();
            FormulaHelper.RegisterService<IDataLogService, DataLogService>();
            FormulaHelper.RegisterService<ICalendarService, CalendarService>();

            // 配置Log4net
            Log4NetConfig.Configure();

            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(Formula.HttpModule.ScriptModule));
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            // TODO 是否需要关闭的服务，或者发送邮件
        }
    }
}
