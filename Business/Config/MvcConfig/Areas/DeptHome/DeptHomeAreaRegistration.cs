using System.Web.Mvc;

namespace MvcConfig.Areas.DeptHome
{
    public class DeptHomeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DeptHome";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DeptHome_default",
                "DeptHome/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
