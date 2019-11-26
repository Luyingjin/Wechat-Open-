using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;

namespace Base.Areas.PortalBlock.Controllers
{
    public class FeedbackController : BaseController<S_H_Feedback>
    {
        //
        // GET: /PortalBlock/Feedback/

        public JsonResult Add()
        {
            S_H_Feedback model = UpdateEntity<S_H_Feedback>(string.Empty);
            entities.Set<S_H_Feedback>().Add(model);
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public override JsonResult GetModel(string id)
        {
            S_H_Feedback model = GetEntity<S_H_Feedback>(id);
            if (string.IsNullOrEmpty(id))
            {
                model.IsUse = "T";

            }
            return Json(model);
        }
        public ActionResult MyFeedbackList()
        {
            return View("");
        }
        public JsonResult GetMyFeedbackList(MvcAdapter.QueryBuilder qb)
        {
            var data = entities.Set<S_H_Feedback>().Where(p => p.CreateUserID == Formula.FormulaHelper.UserID).WhereToGridData(qb);
            return Json(data);
        }
    }
}
