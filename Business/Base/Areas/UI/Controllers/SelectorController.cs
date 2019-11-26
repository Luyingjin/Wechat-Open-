using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Base.Logic.Domain;
using Config;
using System.Data;
using System.Text;
using Formula.Helper;

namespace Base.Areas.UI.Controllers
{
    public class SelectorController : BaseController
    {
        public ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().Where(c => !string.IsNullOrEmpty(c.ParentID)).Select(c => new { value = c.ID, text = c.Name }));
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            return JsonGetList<S_UI_Selector>(qb);
        }

        public JsonResult GetModel(string id)
        {
            return JsonGetModel<S_UI_Selector>(id);
        }

        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Selector>();
            if (entities.Set<S_UI_Selector>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("列表选择编号重复"));
               
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Delete(string ListIDs)
        {
            return JsonDelete<S_UI_Selector>(ListIDs);
        }
    }
}
