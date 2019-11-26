using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Formula;
using Base.Logic.Domain;

namespace Base.Areas.Desktop.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Desktop/Home/

        public ActionResult Index()
        {
            string userID = FormulaHelper.UserID;
            var db = FormulaHelper.GetEntities<BaseEntities>();
            var list = db.S_H_ShortCut.Where(c => c.CreateUserID == userID && c.IsUse == "T").OrderBy(c => c.SortIndex).ToList();
            var dic = new Dictionary<string, DeskIcon>();
            foreach (var shortCut in list)
            {
                shortCut.IconImage = string.IsNullOrWhiteSpace(shortCut.IconImage) ? "001.png" : shortCut.IconImage;
                dic.Add(shortCut.ID, new DeskIcon
                {
                    title = shortCut.Name,
                    url = shortCut.Url,
                    winWidth = 900,
                    winHeight = 600,
                });
            }

            ViewBag.DeskIconData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            ViewBag.ShortCuts_Page0 = list.Where(c => c.PageIndex == 0);
            ViewBag.ShortCuts_Page1 = list.Where(c => c.PageIndex == 1);
            ViewBag.ShortCuts_Page2 = list.Where(c => c.PageIndex == 2);
            ViewBag.ShortCuts_Page3 = list.Where(c => c.PageIndex == 3);
            return View();
        }

        public ActionResult AddIcon()
        {
            // 获取图标
            var iconPath = Server.MapPath("/Desktop");
            var files = System.IO.Directory.GetFiles(iconPath);
            var count = files.Length > 32 ? 32 : files.Length;

            var icons = new string[count];
            for (var i = 0; i < count; i++)
            {
                icons[i] = System.IO.Path.GetFileName(files[i]);
            }

            ViewBag.Icons = icons;
            ViewBag.PageIndex = Request["PageIndex"];
            return View();
        }

        public JsonResult SaveIcon()
        {
            var model = UpdateEntity<S_H_ShortCut>();
            if (string.IsNullOrEmpty(model.ID))
                model.ID = Formula.FormulaHelper.CreateGuid();
            entities.SaveChanges();
            return Json(model);
        }

        public class DeskIcon
        {
            public int pageIndex { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public int winWidth { get; set; }
            public int winHeight { get; set; }
        }

        public JavaScriptResult GetDesktopData()
        {
            var db = FormulaHelper.GetEntities<BaseEntities>();
            var shortCuts = db.S_H_ShortCut.ToList();
            //var menus = privilegeService.GetMenuPrivileges() as List<IPrivilege>;
            //List<nbJsonTreeNode> list = BuildSubTreeNode(menus);

            //JavaScriptSerializer s = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();
            //s.Serialize(list, sb);
            //sb.Insert(0, "var menudata=");
            return JavaScript(sb.ToString());
        }
    }
}
