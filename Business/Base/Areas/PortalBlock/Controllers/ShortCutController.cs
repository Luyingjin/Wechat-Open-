using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;

namespace Base.Areas.PortalBlock.Controllers
{
    public class ShortCutController : BaseController
    {
        //
        // GET: /PortalBlock/ShortCut/

        public JsonResult Add()
        {
            S_H_ShortCut model = UpdateEntity<S_H_ShortCut>(string.Empty);
            entities.Set<S_H_ShortCut>().Add(model);
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult Select(string count)
        {
            int iCount = 5;
            if (!string.IsNullOrEmpty(count))
                iCount = Convert.ToInt32(count);
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            List<S_H_ShortCut> list = entities.Set<S_H_ShortCut>().Where(s => s.CreateUserID == userID && s.IsUse == "T").OrderByDescending(t => t.CreateTime).Take(iCount).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                entities.Set<S_H_ShortCut>().Delete(s => s.ID == id);
                entities.SaveChanges();
            }
            return Json(string.Empty);
        }

        public JsonResult SortShortCut(string pageIndex, string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                string[] arr = ids.Split(',');
                List<S_H_ShortCut> list = entities.Set<S_H_ShortCut>().Where(c => arr.Contains(c.ID)).ToList();
                for (int i = 0; i < arr.Length; i++)
                {
                    string id = arr[i];
                    S_H_ShortCut model = list.Find(c => c.ID == id);
                    if (model != null)
                    {
                        model.PageIndex = Convert.ToInt32(pageIndex);
                        model.SortIndex = i;
                    }
                }
                entities.SaveChanges();
            }
            return Json(string.Empty);
        }
    }
}
