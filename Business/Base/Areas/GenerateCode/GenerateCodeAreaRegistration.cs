using System.Web.Mvc;

namespace Base.Areas.GenerateCode
{
    public class GenerateCodeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GenerateCode";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GenerateCode_default",
                "GenerateCode/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
