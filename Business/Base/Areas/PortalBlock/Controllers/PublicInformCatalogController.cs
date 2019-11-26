using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Base.Logic.Domain;

namespace Base.Areas.PortalBlock.Controllers
{
    public class PublicInformCatalogController : BaseController<S_I_PublicInformCatalog>
    {
        public override JsonResult Save()
        {
            S_I_PublicInformCatalog model = UpdateEntity<S_I_PublicInformCatalog>();
            string key = model.CatalogKey;
            string id = model.ID;
            if (entities.Set<S_I_PublicInformCatalog>().Where(t => t.CatalogKey == key && t.ID != id).Count() > 0)
                throw new Exception("栏目Key重复!");
            S_P_DoorBlock block = entities.Set<S_P_DoorBlock>().FirstOrDefault(c => c.BlockKey == key);
            if (block != null)
            {
                block.BlockTitle = model.CatalogName;
                block.RepeatItemCount = model.InHomePageNum;
            }
            return base.Save();
        }

        public override JsonResult Delete()
        {
            string[] ids = Request["ListIDs"].Split(',');
            List<S_I_PublicInformCatalog> list = entities.Set<S_I_PublicInformCatalog>().Where(c => ids.Contains(c.ID)).ToList();
            foreach (S_I_PublicInformCatalog item in list)
            {
                string name = item.CatalogName;
                string key = item.CatalogKey;
                entities.Set<S_P_DoorBlock>().Delete(c => c.BlockName == name && c.BlockKey == key);
            }
            return base.Delete();
        }

        //
        // GET: /PortalBlock/PublicInform/
        public override JsonResult GetModel(string id)
        {
            S_I_PublicInformCatalog model = GetEntity<S_I_PublicInformCatalog>(id);
            if (string.IsNullOrEmpty(id))
            {
                model.IsOnHomePage = "1";
                model.InHomePageNum = 8;
            }
            return Json(model);
        }
        
        public JsonResult GetData()
        {
            IList<S_I_PublicInformCatalog> list = entities.Set<S_I_PublicInformCatalog>().ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
