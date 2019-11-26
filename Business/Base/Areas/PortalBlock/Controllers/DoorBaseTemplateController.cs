using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;

namespace Base.Areas.PortalBlock.Controllers
{
    public class DoorBaseTemplateController : BaseController<S_P_DoorBaseTemplate>
    {
        //
        // GET: /PortalBlock/DoorBaseTemplate/
        public JsonResult Setting()
        {
            S_P_DoorBaseTemplate baseTmpl = entities.Set<S_P_DoorBaseTemplate>().First();
            if (baseTmpl != null)
            {
                Response.Redirect("/Portal/BaseHomeboard.aspx?TemplateId=" + baseTmpl.ID);
            }
            return Json(string.Empty);
        }

        public override JsonResult GetModel(string id)
        {
            S_P_DoorBaseTemplate model = GetEntity<S_P_DoorBaseTemplate>(id);
            if (string.IsNullOrEmpty(id))
            {
                model.BaseType = "Portal";
                model.IsDefault = "F";
                model.IsEdit = "T";
            }
            return Json(model);
        }

        public JsonResult GetData()
        {
            List<S_P_DoorBaseTemplate> list = entities.Set<S_P_DoorBaseTemplate>().ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
