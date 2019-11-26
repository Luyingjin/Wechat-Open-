using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using Base.Logic.Domain;
using System.Text;
using System.Text.RegularExpressions;
using Formula.Helper;
using System.Collections.Specialized;
using Base.Logic.Model.UI.Form;
using Formula;
using Base.Logic.BusinessFacade;
using MvcAdapter;

namespace Base.Areas.UI.Controllers
{
    public class FormController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name from S_M_Category where FullID like '{0}%'", "0"));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                qb.Add("CategoryID", QueryMethod.Equal, Request["CategoryID"]);
            var list = entities.Set<S_UI_Form>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, TableName = c.TableName, ModifyTime = c.ModifyTime, Category = c.Category });
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
            return JsonGetModel<S_UI_Form>(id);
        }
        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Form>();
            if (entities.Set<S_UI_Form>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("表单编号重复，表单名称“{0}”，表单编号：“{1}”", entity.Name, entity.Code));

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.Items = "[]";
                entity.DefaultValueSettings = "[]";
                entity.FlowLogic = "[]";
                entity.Setttings = "{}";
                entity.SerialNumberSettings = "{\"Tmpl\":\"{YY}{MM}{DD}-{NNNN}\",\"ResetRule\":\"YearCode,MonthCode\"}";
            }
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Delete(string listIDs)
        {
            var ids = listIDs.Split(',');
            var arr = entities.Set<S_UI_Form>().Where(c => ids.Contains(c.ID)).ToArray();

            foreach (var item in arr)
            {
                if (!string.IsNullOrEmpty(item.ReleasedData))
                {
                    var dic = JsonHelper.ToObject(item.ReleasedData);
                    string sql = DeleteTable(dic["TableName"].ToString());
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(dic["ConnName"].ToString());
                    sqlHelper.ExecuteNonQuery(sql);
                }
                entities.Set<S_UI_Form>().Remove(item);
            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 布局信息

        public JsonResult GetLayout(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(new { layout = entity.Layout ?? "" });
        }

        [ValidateInput(false)]
        public JsonResult SaveLayout(string ID, string layout)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            entity.Layout = layout;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult CreateLayout(string ID, string mode)
        {
            int layoutMode = int.Parse(mode.Split(',')[0]);
            string decorative = mode.Split(',')[1]; //fieldset装饰或table装饰

            #region 装饰

            string strDecorativeStart = "";
            string strDecorativeEnd = "";
            string strDecorativeTable = "";

            if (decorative == "fieldset")
            {
                strDecorativeStart = @"
<fieldset class='formDiv'>
    <legend>{0}</legend>
";
                strDecorativeEnd = @"
</fieldset>
";
                strDecorativeTable = "style='width:100%;'cellpadding='2' border='0'";
            }
            else if (decorative == "table")
            {
                strDecorativeStart = @"
<table class='ke-zeroborder' style='width:100%;table-layout:fixed' cellspacing='0' cellpadding='2' border='0'>
	<tbody>
		<tr>
			<td style='text-align:left;width:30%;'>		
                版本/修改码：D/1		
			</td>
			<td style='text-align:center;'>		
                QM-P-03-02
			</td>
            <td style='text-align:right;width:30%;'>
                编号：{{流水号}}
			</td>
		</tr>
	</tbody>
</table>
";
                strDecorativeEnd = "";
                strDecorativeTable = "style='width:100%;table-layout:fixed'cellpadding='2' border='1'";
            }

            #endregion

            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            var listItems = JsonHelper.ToObject<List<FormItem>>(form.Items);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<h1 align='center'>{0}</h1>", form.Name);

            foreach (var group in listItems.Select(c => c.Group ?? "").Distinct().ToList())
            {
                var list = listItems.Where(c => c.Group == group).ToList();
                //补齐
                while (list.Count % layoutMode != 0)
                {
                    list.Add(new FormItem() { Code = "", Name = "" });
                }

                //开始装饰
                sb.AppendFormat(strDecorativeStart, group);

                //开始内容
                sb.AppendFormat(@"
    <table class='ke-zeroborder' {0}>", strDecorativeTable);

                string labelStyle = "style='width: 15%'";
                string inputStyle = "style='width: 35%'";
                if (layoutMode == 3)
                    inputStyle = "style='width: 18%'";

                bool firstRow = true;
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (i % layoutMode == 0)
                    {
                        sb.Append("<tr>");
                        if (firstRow == false)
                        {
                            labelStyle = "";
                            inputStyle = "";
                        }
                        firstRow = false;
                    }

                    sb.AppendFormat("<td {2}>{0}</td><td {3}>{1}</td>", item.Name, string.IsNullOrEmpty(item.Name) ? "" : "{" + item.Name + "}", labelStyle, inputStyle);

                    if (i % layoutMode == layoutMode - 1)
                        sb.Append("</tr>");
                }

                sb.Append(@"
    </table>
");
                //结束内容
                
                sb.Append(strDecorativeEnd);
                //结束装饰

            }


            return Json(new { layout = sb.ToString() });
        }

        #endregion

        #region 控件信息

        public JsonResult GetItemList(string formID)
        {
            return Json(entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID).Items);
        }

        public JsonResult SaveItemList()
        {
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            var list = JsonHelper.ToObject<List<FormItem>>(Request["ItemList"]);
            foreach (var item in list)
            {
                item.Name = item.Name.Trim();
                if (item.Name.Contains('\\') || item.Name.Contains('/'))
                    throw new Exception(string.Format("名称中不能包含特殊字符\\或/,出错的字段名：{0}", item.Name));
                if (item.Name == "流水号")
                    item.Code = "SerialNumber";
                else if (string.IsNullOrEmpty(item.Code))
                    item.Code = uiFO.GetHanZiPinYinString(item.Name);

                #region 子表字段

                if (item.ItemType == "SubTable" && !string.IsNullOrEmpty(item.Settings))
                {
                    var itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item.Settings);
                    var itemSubList = JsonHelper.ToObject<List<FormItem>>(itemDic["listData"].ToString());
                    foreach (var subItem in itemSubList)
                    {
                        subItem.Name = subItem.Name.Trim();
                        if (subItem.Name.Contains('\\') || subItem.Name.Contains('/'))
                            throw new Exception(string.Format("名称中不能包含特殊字符\\或/,出错的字段名：{0}", subItem.Name));
                        if (string.IsNullOrEmpty(subItem.Code))
                            subItem.Code = uiFO.GetHanZiPinYinString(subItem.Name);
                    }

                    #region 验证编号名称不能重复

                    var subFieldNameList = itemSubList.Select(c => c.Name).Distinct().ToList();
                    if (subFieldNameList.Count() < itemSubList.Count)
                    {
                        foreach (var c in itemSubList)
                        {
                            if (!subFieldNameList.Contains(c.Name))
                                throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Name));
                            else subFieldNameList.Remove(c.Name);
                        }
                    }

                    var subFieldCodeList = itemSubList.Select(c => c.Code).Distinct().ToList();
                    if (subFieldCodeList.Count() < itemSubList.Count)
                    {
                        foreach (var c in itemSubList)
                        {
                            if (!subFieldCodeList.Contains(c.Code))
                                throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Code));
                            else subFieldCodeList.Remove(c.Code);
                        }
                    }

                    #endregion

                    //修改完成赋值回去
                    itemDic["listData"] = itemSubList;
                    item.Settings = JsonHelper.ToJson(itemDic);
                }

                #endregion
            }

            #region 校验编号名称不能重复

            var fieldNameList = list.Select(c => c.Name).Distinct().ToList();
            if (fieldNameList.Count() < list.Count)
            {
                foreach (var c in list)
                {
                    if (!fieldNameList.Contains(c.Name))
                        throw new Exception(string.Format("控件名称不能重复：“{0}”", c.Name));
                    else fieldNameList.Remove(c.Name);
                }
            }

            var fieldCodeList = list.Select(c => c.Code).Distinct().ToList();
            if (fieldCodeList.Count() < list.Count)
            {
                foreach (var c in list)
                {
                    if (!fieldCodeList.Contains(c.Code))
                        throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Code));
                    else fieldCodeList.Remove(c.Code);
                }
            }

            #endregion


            string formID = Request["FormID"];
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            form.Items = JsonHelper.ToJson(list);

            var user = FormulaHelper.GetUserInfo();
            form.ModifyUserID = user.UserID;
            form.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        public ActionResult SettingsButtonEdit()
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(0, new { value = "SystemOrg", text = "选择部门" });
            list.Insert(0, new { value = "SystemUser", text = "选择用户" });
            ViewBag.SelectorEnum = JsonHelper.ToJson(list);
            return View();
        }

        #region 从布局导入

        public JsonResult ImportItemFromLayout(string formID)
        {
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (string.IsNullOrEmpty(form.Layout))
                throw new Exception("布局还没有建立");
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            Regex reg = new Regex(UIFO.uiRegStr);
            var matchs = reg.Matches(form.Layout, 0);

            var list = JsonHelper.ToObject<List<FormItem>>(form.Items ?? "[]");

            int index = list.Count;
            foreach (Match match in matchs)
            {
                string name = match.Value.Trim('{', '}');
                if (list.SingleOrDefault(c => c.Name == name) == null)
                    list.Add(new FormItem
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Code = uiFO.GetHanZiPinYinString(name),
                        Name = name,
                        Enabled = "true",
                        Visible = "true",
                        DefaultValue = "",
                        ItemType = "TextBox",
                        FieldType = "nvarchar(200)"
                    });
            }
            return Json(list);
        }

        #endregion

        #endregion

        #region 默认值

        public JsonResult getDefaultValueSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.DefaultValueSettings ?? "");
        }

        public JsonResult SaveDefaultValueSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.DefaultValueSettings = Request["DefaultValueSettings"];
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 流水号

        public JsonResult GetSerialNumberSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.SerialNumberSettings ?? "");
        }

        public JsonResult SaveSerialNumberSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.SerialNumberSettings = Request["FormData"];
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 流程逻辑

        public JsonResult GetFlowLoigcList(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.FlowLogic ?? "[]");
        }
        public JsonResult SaveFlowLoigcList(string formID, string listData)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.FlowLogic = listData;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }



        #endregion

        #region 发布

        public JsonResult ReleaseForm(string id)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == id);
            var dic = JsonHelper.ToObject(!string.IsNullOrEmpty(entity.ReleasedData) ? entity.ReleasedData : "{}");

            var items = JsonHelper.ToObject<List<FormItem>>(entity.Items ?? "[]");


            StringBuilder sb = new StringBuilder(@"
declare @tmp int
declare @existOldField int
declare @existField int
");

            if (dic.Count() == 0 || dic["ConnName"].ToString() != entity.ConnName) //首次发布版本或者换数据库
            {
                sb.Append(CreateTable(entity.TableName, entity.Name));

                foreach (var item in items)
                {
                    sb.Append(CreateField(entity.TableName, item.Code, item.FieldType, item.Name));

                    #region 同时处理ButtonEdit和SubTable
                    if (item.ItemType == "ButtonEdit")
                        sb.Append(CreateField(entity.TableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                    if (item.ItemType == "SubTable")
                    {
                        var subTableItems = getSubTableItem(item.Settings);
                        sb.Append(CreateSubTable(entity.TableName + "_" + item.Code, item.Name, subTableItems));
                    }
                    #endregion
                }
            }
            else
            {
                sb.Append(ModifyTable(dic["TableName"].ToString(), entity.TableName, dic["Name"].ToString(), entity.Name));

                var oldItems = JsonHelper.ToObject<List<FormItem>>(dic["Items"].ToString());
                string connName = entity.ConnName;
                string tableName = entity.TableName;

                foreach (var oldItem in oldItems)
                {
                    if (items.Where(c => c.ID == oldItem.ID).Count() == 0)
                    {
                        sb.Append(DeleteField(tableName, oldItem.Code));

                        #region //同时删除ButtonEdit和SubTable

                        if (oldItem.ItemType == "ButtonEdit")
                            sb.Append(DeleteField(tableName, oldItem.Code + "Name"));

                        if (oldItem.ItemType == "SubTable")
                        {
                            sb.Append(DeleteSubTable(tableName + "_" + oldItem.Code));
                        }
                        #endregion
                    }
                }
                foreach (var item in items)
                {
                    var oldItem = oldItems.SingleOrDefault(c => c.ID == item.ID);
                    if (oldItem == null)
                    {
                        sb.Append(CreateField(tableName, item.Code, item.FieldType, item.Name));

                        #region //同时创建ButtonEdit和SubTable

                        if (item.ItemType == "ButtonEdit")
                            sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                        if (item.ItemType == "SubTable")
                        {
                            var subTableItems = getSubTableItem(item.Settings);
                            sb.Append(CreateSubTable(entity.TableName + "_" + item.Code, item.Name, subTableItems));
                        }

                        #endregion
                    }
                    else
                    {
                        sb.Append(ModifyField(tableName
                             , oldItem.Code, oldItem.FieldType, oldItem.Name
                             , item.Code, item.FieldType, item.Name));

                        #region //同时修改ButtonEdit冗余字段

                        #region ButtonEdit

                        if (item.ItemType == "ButtonEdit" && oldItem.ItemType == "ButtonEdit")
                        {
                            sb.Append(ModifyField(tableName
                              , oldItem.Code + "Name", oldItem.FieldType, oldItem.Name
                              , item.Code + "Name", item.FieldType, item.Name + "名称"));
                        }
                        if (item.ItemType == "ButtonEdit" && oldItem.ItemType != "ButtonEdit")
                        {
                            sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                        }
                        if (item.ItemType != "ButtonEdit" && oldItem.ItemType == "ButtonEdit")
                        {
                            sb.Append(DeleteField(tableName, oldItem.Code + "Name"));
                        }

                        #endregion

                        #region SubTable

                        if (item.ItemType == "SubTable" && oldItem.ItemType == "SubTable")
                        {
                            var oldSubTableItems = getSubTableItem(oldItem.Settings);
                            var subTableItems = getSubTableItem(item.Settings);
                            sb.Append(ModifySubTable(tableName + "_" + oldItem.Code, tableName + "_" + item.Code, oldItem.Name, item.Name, oldSubTableItems, subTableItems));
                        }

                        if (item.ItemType == "SubTable" && oldItem.ItemType != "SubTable")
                        {
                            var subTableItems = getSubTableItem(item.Settings);
                            sb.Append(CreateSubTable(tableName + "_" + item.Code, item.Name, subTableItems));
                        }

                        if (item.ItemType != "SubTable" && oldItem.ItemType == "SubTable")
                        {
                            sb.Append(DeleteSubTable(tableName + "_" + oldItem.Code));
                        }

                        #endregion

                        #endregion
                    }
                }
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
            sqlHelper.ExecuteNonQuery(sb.ToString());

            #region //更新数据字典

            MetaFO metaFO = FormulaHelper.CreateFO<MetaFO>();
            metaFO.ImportTable(entity.ConnName, false);
            metaFO.ImportField(entity.ConnName, entity.TableName);
            foreach (var item in items)
            {
                if (item.ItemType == "SubTable")
                {
                    metaFO.ImportField(entity.ConnName, entity.TableName + "_" + item.Code);
                }
            }

            #endregion

            var releasedData = new { ConnName = entity.ConnName, TableName = entity.TableName, Name = entity.Name, Items = entity.Items };
            entity.ReleasedData = JsonHelper.ToJson(releasedData);
            entity.ReleaseTime = DateTime.Now;
            entities.SaveChanges();
            return Json("");
        }

        #region 子表创建

        private string CreateSubTable(string tableName, string tableDesc, List<FormItem> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateTable(tableName, tableDesc));

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.FieldType))
                    continue;
                sb.Append(CreateField(tableName, item.Code, item.FieldType, item.Name));
                if (item.ItemType == "ButtonEdit")
                    sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
            }

            return sb.ToString();
        }

        #endregion

        #region 子表修改

        private string ModifySubTable(string oldTableName, string tableName, string oldTableDesc, string tableDesc, List<FormItem> oldItems, List<FormItem> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ModifyTable(oldTableName, tableName, oldTableDesc, tableDesc));

            foreach (var oldItem in oldItems)
            {
                if (items.Where(c => c.ID == oldItem.ID).Count() == 0)
                {
                    sb.Append(DeleteField(tableName, oldItem.Code));
                    #region //同时删除ButtonEdit的冗余字段
                    if (oldItem.ItemType == "ButtonEdit")
                        sb.Append(DeleteField(tableName, oldItem.Code + "Name"));
                    #endregion
                }
            }
            foreach (var item in items)
            {
                var oldItem = oldItems.SingleOrDefault(c => c.ID == item.ID);
                if (oldItem == null)
                {
                    sb.Append(CreateField(tableName, item.Code, item.FieldType, item.Name));
                    #region //同时创建ButtonEdit冗余字段
                    if (item.ItemType == "ButtonEdit")
                        sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                    #endregion
                }
                else
                {
                    sb.Append(ModifyField(tableName
                         , oldItem.Code, oldItem.FieldType, oldItem.Name
                         , item.Code, item.FieldType, item.Name));

                    #region //同时修改ButtonEdit冗余字段
                    if (item.ItemType == "ButtonEdit" && oldItem.ItemType == "ButtonEdit")
                    {
                        sb.Append(ModifyField(tableName
                          , oldItem.Code + "Name", oldItem.FieldType, oldItem.Name
                          , item.Code + "Name", item.FieldType, item.Name + "名称"));
                    }
                    if (item.ItemType == "ButtonEdit" && oldItem.ItemType != "ButtonEdit")
                    {
                        sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                    }
                    if (item.ItemType != "ButtonEdit" && oldItem.ItemType == "ButtonEdit")
                    {
                        sb.Append(DeleteField(tableName, oldItem.Code + "Name"));
                    }
                    #endregion
                }
            }

            return sb.ToString();
        }

        #endregion

        #region 子表删除

        public string DeleteSubTable(string oldTableName)
        {
            return DeleteTable(oldTableName);
        }

        #endregion

        #region 数据库操作方法

        #region 创建表

        private string CreateTable(string tableName, string tableDesc)
        {
            string sql = @" 
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
BEGIN
CREATE TABLE [{0}](
 [ID] [nvarchar](50) NOT NULL,
 [CreateDate] [datetime] NULL,
 [ModifyDate] [datetime] NULL,
 [CreateUserID] [nvarchar](50) NULL, 
 [CreateUser] [nvarchar](50) NULL, 
 [ModifyUserID] [nvarchar](50) NULL, 
 [ModifyUser] [nvarchar](50) NULL, 
 [OrgID] [nvarchar](50) NULL, 
 [PrjID] [nvarchar](50) NULL, 
 [FlowPhase] [nvarchar](50) NULL, 
 [StepName] [nvarchar](500) NULL,
 CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
EXECUTE sp_addextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL
EXECUTE sp_addextendedproperty 'MS_Description', 'ID', 'user', dbo, 'table', '{0}', 'column', 'ID'
END";

            sql = string.Format(sql, tableName, tableDesc);
            if (tableName.Contains('_'))
            {
                var arr = tableName.Split('_').ToList();
                arr.Remove(arr.Last());
                string mainTableName = string.Join("_", arr);

                sql = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
begin
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{1}]') AND type in (N'U'))
    begin
        CREATE TABLE [{0}](
         [ID] [nvarchar](50) NOT NULL,
         [CreateDate] [datetime] NULL,
         [ModifyDate] [datetime] NULL,
         [CreateUserID] [nvarchar](50) NULL,
         [CreateUser] [nvarchar](50) NULL,
         [ModifyUserID] [nvarchar](50) NULL, 
         [ModifyUser] [nvarchar](50) NULL, 
         [OrgID] [nvarchar](50) NULL,        
         [PrjID] [nvarchar](50) NULL,        
         [FlowPhase] [nvarchar](50) NULL,        
         [StepName] [nvarchar](500) NULL,
         CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
        (
	        [ID] ASC
        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        EXECUTE sp_addextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL
        EXECUTE sp_addextendedproperty 'MS_Description', 'ID', 'user', dbo, 'table', '{0}', 'column', 'ID'
    end
    else
    begin
        CREATE TABLE [{0}](
         [ID] [nvarchar](50) NOT NULL,
         {1}ID [nvarchar](50) NULL,     
         SortIndex [float] NULL,
         [IsReleased] [char](1) NULL,
         CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
        (
	        [ID] ASC
        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        EXECUTE sp_addextendedproperty 'MS_Description','{2}','user','dbo','table','{0}',NULL,NULL
        EXECUTE sp_addextendedproperty 'MS_Description', 'ID', 'user', dbo, 'table', '{0}', 'column', 'ID'
        EXECUTE sp_addextendedproperty 'MS_Description', '{1}ID', 'user', dbo, 'table', '{0}', 'column', '{1}ID' 
        EXECUTE sp_addextendedproperty 'MS_Description', '排序号', 'user', dbo, 'table', '{0}', 'column', 'SortIndex'       
        EXECUTE sp_addextendedproperty 'MS_Description', '已发布', 'user', dbo, 'table', '{0}', 'column', 'IsReleased'

        ALTER TABLE [{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{1}] FOREIGN KEY([{1}ID])
        REFERENCES [{1}] ([ID])
        ON DELETE CASCADE
        ALTER TABLE [{0}] CHECK CONSTRAINT [FK_{0}_{1}]
    end
end
";
                sql = string.Format(sql, tableName, mainTableName, tableDesc);
            }


            return sql;

            //代码备注-以供参考：列出表TestTable中列TestCol的描述属性
            //SELECT * FROM ::fn_ListExtendedProperty ( 'MS_Description' , 'User' , 'dbo' , 'Table' , 'TestTable' , 'Column' , 'TestCol' )
        }

        #endregion

        #region 修改表

        private string ModifyTable(string oldTableName, string tableName, string oldTableDesc, string tableDesc)
        {
            //防止没有旧表
            string sql = CreateTable(oldTableName, oldTableDesc);

            if (oldTableName != tableName)
                sql += string.Format("\n EXEC sp_rename '{0}', '{1}'", oldTableName, tableName);
            if (oldTableDesc != tableDesc)
                sql += string.Format("\n EXECUTE sp_updateextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL", tableName, tableDesc);

            return sql;
        }

        #endregion

        #region 删除表

        private string DeleteTable(string tableName)
        {
            string sql = @"
select fk.name,fk.object_id,OBJECT_NAME(fk.parent_object_id) as referenceTableName from sys.foreign_keys as fk join sys.objects as o on fk.referenced_object_id=o.object_id where o.name='{0}'";



            sql = string.Format(@" 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
BEGIN
    IF EXISTS (select fk.name,fk.object_id,OBJECT_NAME(fk.parent_object_id) as referenceTableName from sys.foreign_keys as fk join sys.objects as o on fk.referenced_object_id=o.object_id where o.name='{0}')
        BEGIN
         declare @s nvarchar(1000)
         select @s='DROP TABLE ' + OBJECT_NAME(fk.parent_object_id) from sys.foreign_keys as fk join sys.objects as o on fk.referenced_object_id=o.object_id where o.name='{0}'
         exec(@s)
        END
    DROP TABLE [dbo].[{0}]
END
", tableName);

            return sql;
        }

        #endregion

        #region 创建字段

        private string CreateField(string tableName, string fieldName, string fieldType, string fieldDesc)
        {
            if (string.IsNullOrEmpty(fieldType))
                return "";
            fieldType = fieldType.Replace(' ', ',');//decimal(18,2)

            string sql = string.Format(@"
if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}') begin 
alter table {0} add {1} {2} 
EXEC sp_addextendedproperty 'MS_Description', '{3}', 'user', dbo, 'table', {0}, 'column', {1}
end  ", tableName, fieldName, fieldType, fieldDesc);

            return sql;
        }

        #endregion

        #region 修改字段

        private string ModifyField(string tableName, string oldFieldName, string oldFieldType, string oldFieldDesc, string fieldName, string fieldType, string fieldDesc)
        {

            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(oldFieldType))
                return sb.ToString();
            if (string.IsNullOrEmpty(fieldType) && !string.IsNullOrEmpty(oldFieldType))
            {
                sb.Append(DeleteField(tableName, oldFieldName));
                return sb.ToString();
            }
            if (!string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(oldFieldType))
            {
                sb.Append(CreateField(tableName, fieldName, fieldType, fieldDesc));
                return sb.ToString();
            }

            oldFieldType = oldFieldType.Replace(' ', ',');//decimal(18,2)
            fieldType = fieldType.Replace(' ', ',');//decimal(18,2)


            string modifyField = "";
            string modifyFieldType = "";
            string modifyFieldDesc = "";

            if (oldFieldName != fieldName)
                modifyField = string.Format("\n EXEC sp_rename '[{0}].[{1}]', '{2}', 'COLUMN' ", tableName, oldFieldName, fieldName);
            if (oldFieldType != fieldType)
                modifyFieldType = string.Format("\n ALTER TABLE {0} ALTER COLUMN {1} {2}", tableName, fieldName, fieldType);
            if (oldFieldDesc != fieldDesc)
                modifyFieldDesc = string.Format("\n EXECUTE sp_updateextendedproperty N'MS_Description', '{2}', N'user', N'dbo', N'Table', {0}, N'column' , {1}", tableName, fieldName, fieldDesc);


            string sql = string.Format(@"
if exists(select * from syscolumns where id=object_id('{0}') and name='{1}')
    set @existOldField=1
else 
    set @existOldField=0
if exists(select * from syscolumns where id=object_id('{0}') and name='{2}')
    set @existField=1
else
    set @existField=0

if( @existOldField=1 and @existField=0 )
begin set @tmp=1
    {3}
end
else if( @existOldField=0 and @existField=0 )
begin set @tmp=1
    {4}
end
else if ( @existOldField=0 and @existField=1 )
begin set @tmp=1
    {5}
end
else if ( @existOldField=1 and @existField=1 and '{1}'<>'{2}')
begin set @tmp=1
    {6}
end
else if ( @existOldField=1 and @existField=1 and '{1}'='{2}')
begin set @tmp=1
    {7}
end
", tableName, oldFieldName, fieldName
 , modifyField + modifyFieldType + modifyFieldDesc
 , CreateField(tableName, fieldName, fieldType, fieldDesc)
 , modifyFieldType + modifyFieldDesc
 , DeleteField(tableName, oldFieldName) + modifyFieldType + modifyFieldDesc
 , modifyFieldType + modifyFieldDesc
 );
            sb.Append(sql);

            return sb.ToString();
        }
        #endregion

        #region 删除字段

        private string DeleteField(string tableName, string fieldName)
        {
            string sql = string.Format(@"
if exists(select * from syscolumns where id=object_id('{0}') and name='{1}') begin
ALTER TABLE {0} DROP COLUMN {1}
end
", tableName, fieldName);
            return sql;
        }

        #endregion

        #endregion

        #region 私有方法

        private List<FormItem> getSubTableItem(string settings)
        {
            if (string.IsNullOrEmpty(settings) || !settings.Contains("listData"))
                return new List<FormItem>();
            var dic = JsonHelper.ToObject(settings);
            return JsonHelper.ToObject<List<FormItem>>(dic["listData"].ToString());
        }

        #endregion

        #endregion

        #region 克隆

        public JsonResult Clone(string formID)
        {
            var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            var newFormInfo = new S_UI_Form();
            FormulaHelper.UpdateModel(newFormInfo, formInfo);

            #region 修改ID、字段ID和子表字段ID
            newFormInfo.ID = FormulaHelper.CreateGuid();
            var items = JsonHelper.ToObject<List<FormItem>>(newFormInfo.Items);
            foreach (var item in items)
            {
                item.ID = FormulaHelper.CreateGuid();
                if (item.ItemType == "SubTable")
                {
                    if (string.IsNullOrEmpty(item.Settings)) continue;
                    var itemDic = JsonHelper.ToObject(item.Settings);
                    var subItems = JsonHelper.ToObject<List<FormItem>>(itemDic["listData"].ToString());
                    foreach (var subItem in subItems)
                    {
                        subItem.ID = FormulaHelper.CreateGuid();
                    }
                    //修改完成赋值回去
                    itemDic["listData"] = subItems;
                    item.Settings = JsonHelper.ToJson(itemDic);
                }
            }
            //修改完成赋值回去
            newFormInfo.Items = JsonHelper.ToJson(items);
            #endregion

            newFormInfo.Code += "_copy";
            newFormInfo.TableName += "_copy";
            newFormInfo.Name += "(副本)";
            newFormInfo.ModifyTime = null;
            newFormInfo.ModifyUserID = "";
            newFormInfo.ModifyUserName = "";
            newFormInfo.ReleaseTime = null;
            newFormInfo.ReleasedData = null;
            entities.Set<S_UI_Form>().Add(newFormInfo);
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

    }
}
