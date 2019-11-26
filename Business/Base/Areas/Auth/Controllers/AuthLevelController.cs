using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Helper;
using Base.Logic.Domain;

namespace Base.Areas.Auth.Controllers
{
    public class AuthLevelController : BaseController
    {
        public JsonResult GetList()
        {
            return Json(entities.Set<S_A_AuthLevel>());
        }

        public JsonResult SaveList()
        {
            return JsonSaveList<S_A_AuthLevel>();
        }

        public JsonResult GetCheckedMenu(string id)
        {
            var level = entities.Set<S_A_AuthLevel>().SingleOrDefault(c => c.ID == id);
            if (level == null)
                return Json(new S_A_Res[] { });
            var arr = level.MenuRootFullID.Split(',');
            var result = entities.Set<S_A_Res>().Where(c => arr.Contains(c.FullID));
            return Json(result);
        }
        public JsonResult GetCheckedRule(string id)
        {
            var level = entities.Set<S_A_AuthLevel>().SingleOrDefault(c => c.ID == id);
            if (level == null)
                return Json(new S_A_Res[] { });
            var arr = level.RuleRootFullID.Split(',');
            var result = entities.Set<S_A_Res>().Where(c => arr.Contains(c.FullID));
            return Json(result);
        }
    }
}
