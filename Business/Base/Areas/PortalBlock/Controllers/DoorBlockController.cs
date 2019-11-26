using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula.Helper;
using Formula;

namespace Base.Areas.PortalBlock.Controllers
{
    public class DoorBlockController : BaseController<S_P_DoorBlock>
    {
        //
        public override JsonResult GetModel(string id)
        {
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(GetQueryString("OldID")))
                id = GetQueryString("OldID");
            S_P_DoorBlock model = GetEntity<S_P_DoorBlock>(id);
            if (string.IsNullOrEmpty(id))
            {
                S_P_DoorBaseTemplate baseTmpl = entities.Set<S_P_DoorBaseTemplate>().First();
                if (baseTmpl != null)
                {
                    model.TemplateId = baseTmpl.ID;
                    model.BlockType = "Portal";
                    model.IsEdit = "F";
                    model.IsHidden = "F";
                    model.Color = "Default";
                    model.ColorValue = "#CDCDCD";
                    model.BlockImage = "/Portal/Door/image/box.gif";
                    model.DelayLoadSecond = 0;
                }
            }
            if (GetQueryString("FuncType") == "Copy")
                model.ID = GetGuid();
            return Json(model);
        }


        public override ActionResult List()
        {
            List<S_P_DoorBaseTemplate> list = entities.Set<S_P_DoorBaseTemplate>().ToList();
            ViewBag.BaseTemplates = "var BaseTemplates = " + JsonHelper.ToJson(list);
            return base.List();
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            S_P_DoorBlock model = UpdateEntity<S_P_DoorBlock>();
            string blockKey = model.BlockKey;
            S_I_PublicInformCatalog piCatalog = entities.Set<S_I_PublicInformCatalog>().FirstOrDefault(c => c.CatalogKey == blockKey);
            if (piCatalog != null)
            {
                piCatalog.CatalogName = model.BlockTitle;
                piCatalog.InHomePageNum = model.RepeatItemCount;
            }
            return base.Save();
        }

        public JsonResult AddByCatalog(string catalogName, string catalogKey)
        {
            List<S_P_DoorBlock> listBlock = entities.Set<S_P_DoorBlock>().Where(c => c.BlockKey == catalogKey).ToList();
            if (listBlock.Count == 0)
            {
                List<S_P_DoorBlock> listTmpl = entities.Set<S_P_DoorBlock>().Where(c => c.BlockKey == "CNews").ToList();
                if (listTmpl.Count > 0)
                {
                    S_P_DoorBlock tmpl = listTmpl[0];
                    S_P_DoorBlock newBlock = new S_P_DoorBlock();
                    FormulaHelper.UpdateModel(newBlock, tmpl);
                    newBlock.ID = this.GetGuid();
                    newBlock.BlockName = catalogName;
                    newBlock.BlockTitle = catalogName;
                    newBlock.BlockKey = catalogKey;
                    newBlock.FootHtml = newBlock.FootHtml.Replace(tmpl.BlockKey, catalogKey);
                    newBlock.RelateScript = newBlock.RelateScript.Replace(tmpl.BlockName, catalogName).Replace(tmpl.BlockKey, catalogKey);
                    entities.Set<S_P_DoorBlock>().Add(newBlock);
                    entities.SaveChanges();
                }
            }
            return Json(string.Empty);
        }
    }
}
