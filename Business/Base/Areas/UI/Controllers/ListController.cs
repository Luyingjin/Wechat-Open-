using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using Base.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Base.Logic.BusinessFacade;
using Formula;
using Formula.Exceptions;

namespace Base.Areas.UI.Controllers
{
    public class ListController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name from S_M_Category where FullID like '{0}%'", "0"));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                qb.Add("CategoryID", QueryMethod.Equal, Request["CategoryID"]);
            var list = entities.Set<S_UI_List>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, SQL = c.SQL, ModifyTime = c.ModifyTime });
            GridData data = new GridData(list);
            data.total = qb.TotolCount;

            return Json(data);
        }

        #endregion

        #region 基本信息

        public ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().Where(c => !string.IsNullOrEmpty(c.ParentID)).Select(c => new { value = c.ID, text = c.Name }));
            return View();
        }

        public JsonResult GetModel(string id)
        {
            return JsonGetModel<S_UI_List>(id);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_List>();
            if (entities.Set<S_UI_List>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("列表编号重复，表单名称“{0}”，表单编号：“{1}”", entity.Name, entity.Code));
            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;

            if (entity._state == EntityStatus.added.ToString())
            {
                entity.LayoutButton = @"[
{'id':'btnAdd', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}','iconcls':'icon-add','text':'添加','Enabled':'true','Visible':'true','Settings':'{PopupTitle:\'添加\'}'},
{'id':'btnModify', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&ID={ID}','iconcls':'icon-edit','text':'修改','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'修改\'}'},
{'id':'btnDelete', 'iconcls':'icon-remove','text':'删除','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'del();\',SelectMode:\'mustSelectRow\',Confirm:\'true\'}'},
{'id':'btnView','URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&ID={ID}&FuncType=View','iconcls':'icon-search','text':'查看','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'查看\'}'},
{'id':'btnStart','URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&FlowCode={TmplCode}','iconcls':'icon-add','text':'启动流程','Enabled':'true','Visible':'true','Settings':'{PopupTitle:\'启动\'}'},
{'id':'btnView', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&FlowCode={TmplCode}&ID={ID}','iconcls':'icon-search','text':'查看流程表单','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'查看\'}'},
{'id':'btnSelect','iconcls':'icon-refer','text':'选择','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'returnList();\',SelectMode:\'mustSelectRow\',Confirm:\'false\'}'},
{'id':'btnTrace','URL':'/MvcConfig/Workflow/Trace/Diagram?ID={ID}&FuncType=FlowTrace','iconcls':'icon-flowstart','text':'流程跟踪','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'流程跟踪\'}'},
{'id':'btnExportExcel','iconcls':'icon-excel','text':'导出Excel','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'exportList();\'}'},
{'id':'btnExportWord','iconcls':'icon-word','text':'导出Word','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'exportWord();\'}'}
]";
                entity.LayoutButton.Replace("\r\n", "");

                entity.LayoutGrid = "{\"multiSelect\":\"true\",\"allowAlternating\":\"false\",\"frozenStartColumn\":\"\",\"frozenEndColumn\":\"\",\"drawcell\":\"\"}";

                entity.LayoutField = "[]";

                entity.LayoutSearch = "[]";
            }


            return JsonSave<S_UI_List>(entity);
        }

        public JsonResult Delete()
        {
            return JsonDelete<S_UI_List>(Request["ListIDs"]);
        }

        #endregion

        #region Grid属性编辑

        public JsonResult GetLayoutGrid(string id)
        {
            return Json(entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id).LayoutGrid);
        }

        public JsonResult SaveLayoutGrid(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            entity.LayoutGrid = Request["FormData"];

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 字段布局

        public ActionResult SettingsButtonEdit()
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(0, new { value = "SystemOrg", text = "选择部门" });
            list.Insert(0, new { value = "SystemUser", text = "选择用户" });
            ViewBag.SelectorEnum = JsonHelper.ToJson(list);
            return View();
        }

        public JsonResult GetLayoutField(string id)
        {
            return Json(entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id).LayoutField);
        }

        public JsonResult SaveLayoutField(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            entity.LayoutField = Request["layoutField"];

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportField(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            var list = JsonHelper.ToList(entity.LayoutField ?? "[]");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
            string sql = entity.SQL.Split(new string[] { "ORDER BY", "order by", "Order By", "Order by" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from ({0}) as table1 where 1=2", sql));

            var tableNames = entity.TableNames.Split(',');
            var mFields = entities.Set<S_M_Field>().Where(c => tableNames.Contains(c.S_M_Table.Code)).ToList();

            foreach (DataColumn col in dt.Columns)
            {
                string code = col.ColumnName;
                string name = code;
                if (list.Where(c => c["field"].ToString() == code).Count() > 0)
                    continue;
                var mField = mFields.FirstOrDefault(c => c.Code == code);
                if (mField != null && !string.IsNullOrEmpty(mField.Name))
                    name = mField.Name;
                var dic = new Dictionary<string, object>();
                dic.Add("field", code);
                dic.Add("header", name);
                dic.Add("width", "");
                dic.Add("align", "left");
                dic.Add("Visible", "true");
                dic.Add("allowSort", "true");
                dic.Add("AllowSearch", "false");
                dic.Add("QueryMode", "");

                if (col.DataType == typeof(DateTime))
                    dic.Add("Settings", "{dateFormat:'yyyy-MM-dd'}");
                else
                    dic.Add("Settings", "{}");
                list.Add(dic);
            }

            return Json(list);
        }

        #endregion

        #region 按钮布局

        public ActionResult SettingsButton(string id)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            ViewBag.FieldEnum = listDef.LayoutField;
            return View();
        }

        public ActionResult LayoutButtonEdit()
        {
            string path = Server.MapPath("/CommonWebResource/Theme/Default/MiniUI/icons");
            List<object> list = new List<object>();
            foreach (string item in System.IO.Directory.EnumerateFiles(path))
            {
                string name = item.Split('\\').Last().Split('.').First();
                list.Add(new { value = "icon-" + name, text = name });
            }
            ViewBag.IconEnum = JsonHelper.ToJson(list);
            return View();
        }

        public JsonResult GetLayoutButton(string id)
        {
            return Json(entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id).LayoutButton);
        }

        public JsonResult SaveLayoutButton(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            entity.LayoutButton = Request["layoutButton"];

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 克隆

        public JsonResult Clone(string listID)
        {
            var listInfo = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == listID);
            var newFormInfo = new S_UI_List();
            FormulaHelper.UpdateModel(newFormInfo, listInfo);
            newFormInfo.ID = FormulaHelper.CreateGuid();
            newFormInfo.Code += "_copy";
            newFormInfo.Name += "(副本)";
            newFormInfo.ModifyTime = null;
            newFormInfo.ModifyUserID = "";
            newFormInfo.ModifyUserName = "";
            entities.Set<S_UI_List>().Add(newFormInfo);
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 枚举选择

        public JsonResult GetEnumList(QueryBuilder qb)
        {
            var result = entities.Set<S_M_EnumDef>().WhereToGridData(qb);
            return Json(result);
        }

        #endregion

        #region 创建菜单入口

        public JsonResult CreateMenu(string ListID)
        {
            var list = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == ListID);

            string url = "/MvcConfig/UI/List/PageView?TmplCode=" + list.Code;

            if (entities.Set<S_A_Res>().Count(c => c.Url.StartsWith(url)) > 0)
            {
                throw new BusinessException("菜单入口已存在！");
            }

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == list.CategoryID);
            var pMenu = entities.Set<S_A_Res>().SingleOrDefault(c => c.Name == category.Name);
            if (pMenu == null)
                pMenu = entities.Set<S_A_Res>().SingleOrDefault(c => c.ID == Config.Constant.MenuRooID);
            var menu = new S_A_Res();
            menu.ID = FormulaHelper.CreateGuid();
            menu.Name = list.Name;
            menu.Url = url;
            menu.ParentID = pMenu.ID;
            menu.FullID = pMenu.FullID + "." + menu.ID;
            menu.SortIndex = 9999;
            menu.Type = "Menu";
            entities.Set<S_A_Res>().Add(menu);
            S_A__OrgRes orgRes = new S_A__OrgRes();
            orgRes.OrgID = Config.Constant.OrgRootID;
            orgRes.ResID = menu.ID;
            entities.Set<S_A__OrgRes>().Add(orgRes);
            entities.SaveChanges();
            return Json(new { ID = menu.ID });
        }

        #endregion

    }
}
