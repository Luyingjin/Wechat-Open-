using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Desktop.Controllers
{
    public class PluginController : BaseController
    {
        //
        // GET: /Desktop/Plugin/

        public ActionResult Weather()
        {
            return View();
        }

    }
}
