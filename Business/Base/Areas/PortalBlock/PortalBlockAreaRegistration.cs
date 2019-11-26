using System.Web.Mvc;

namespace Base.Areas.PortalBlock
{
    public class PortalBlockAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PortalBlock";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PortalBlock_default",
                "PortalBlock/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
