using System.Web.Mvc;

namespace Base.Areas.AutoReport
{
    public class AutoReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AutoReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AutoReport_default",
                "AutoReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
