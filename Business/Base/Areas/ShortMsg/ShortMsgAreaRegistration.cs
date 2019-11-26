using System.Web.Mvc;

namespace Base.Areas.ShortMsg
{
    public class ShortMsgAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ShortMsg";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ShortMsg_default",
                "ShortMsg/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
