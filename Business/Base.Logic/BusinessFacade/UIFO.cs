using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula.Helper;
using Formula;
using Base.Logic.Domain;
using Base.Logic.Model.UI.Form;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using Config;

namespace Base.Logic.BusinessFacade
{
    public class UIFO
    {
        public static string uiRegStr = "\\{[()（），。、；,.;0-9a-zA-Z_\u4e00-\u9faf]*\\}";//，。、；,.;

        #region 表单Html及脚本

        #region CreateFormHiddenHtml

        public string CreateFormHiddenHtml(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == code);
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));
            StringBuilder sb = new StringBuilder();
            var items = JsonHelper.ToObject<List<FormItem>>(form.Items).Where(c => c.ItemType == "");

            foreach (var item in items)
            {
                sb.AppendFormat("\n<input name=\"{0}\" class=\"mini-hidden\" />", item.Code);
            }
            return sb.ToString();
        }

        #endregion

        #region CreateFormHtml

        public string CreateFormHtml(string code, DataRow formDataRow)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == code);
            if (formInfo == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));

            var defaultValueRows = GetDefaultValueRows(formInfo.DefaultValueSettings);

            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
            Regex reg = new Regex(uiRegStr);

            string result = reg.Replace(formInfo.Layout ?? "", (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                FormItem item = items.SingleOrDefault(c => c.Name == value);

                #region 没有控件时

                if (item == null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                        return HttpContext.Current.Request[value];
                    else
                        return m.Value;
                }

                #endregion

                #region 控件类型为空时

                if (string.IsNullOrEmpty(item.ItemType))
                {
                    if (formDataRow != null && formDataRow.Table.Columns.Contains(item.Code))
                        return string.Format("<span id='{0}'>{1}</span>", item.Code, formDataRow[item.Code].ToString());
                    else
                        return string.Format("<span id='{0}'></span>", item.Code);
                }

                #endregion

                #region 控件类型为子表时

                if (item.ItemType == "SubTable")
                {
                    return CreateSubTableHtml(item);
                }

                #endregion

                string miniuiSettings = GetMiniuiSettings(item.Settings);
                if (miniuiSettings == "")
                    miniuiSettings = "style='width:100%'";

                if (item.ItemType == "TextBox" | item.ItemType == "TextArea")
                {
                    if (!miniuiSettings.Contains("maxLength"))
                    {
                        if (item.FieldType == "nvarchar(50)")
                            miniuiSettings += " maxLength='50'";
                        if (item.FieldType == "nvarchar(200)")
                            miniuiSettings += " maxLength='200'";
                        if (item.FieldType == "nvarchar(500)")
                            miniuiSettings += " maxLength='500'";
                        if (item.FieldType == "nvarchar(2000)")
                            miniuiSettings += " maxLength='2000'";
                    }
                }


                return string.Format("<input name='{0}' {5} class='mini-{1}' {2} {3} {4}/>"
                    , item.Code, item.ItemType.ToLower(), miniuiSettings
                    , item.Enabled == "true" ? "" : "enabled='false'"
                    , item.Visible == "true" ? "" : "visible='false'"
                    , item.ItemType == "ButtonEdit" ? string.Format("textName='{0}Name'", item.Code) : ""
                    );

            });

            return result;
        }

        #endregion

        #region CreateFormScript

        public string CreateFormScript(string code, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == code);
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));
            StringBuilder sb = new StringBuilder("\n");
            var list = JsonHelper.ToObject<List<FormItem>>(form.Items);

            #region 添加选择器脚本

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Settings))
                    continue;

                if (item.ItemType == "ButtonEdit")
                {
                    var dic = JsonHelper.ToObject<Dictionary<string, string>>(item.Settings);

                    string returnParams = "value:ID,text:Name";
                    if (dic.ContainsKey("ReturnParams"))
                        returnParams = dic["ReturnParams"];

                    if (dic["SelectorKey"].ToString() == "SystemUser")
                    {
                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                        else
                            sb.AppendFormat("addSingleUserSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                    }
                    else if (dic["SelectorKey"].ToString() == "SystemOrg")
                    {
                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiOrgSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                        else
                            sb.AppendFormat("addSingleOrgSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                    }
                    else
                    {
                        string selectorKey = dic["SelectorKey"];
                        var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                        if (selector == null)
                            throw new Exception(string.Format("弹出选择框控件“{0}”尚未配置选择页面", item.Name));
                        var url = selector.URLSingle;
                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            url = selector.URLMulti;

                        sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}',returnParams:'{5}' }});\n"
                            , item.Code, url, selector.Title, selector.Width, selector.Height, returnParams);
                    }
                }
                else if (item.ItemType == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item.Settings);
                    var subList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    foreach (var subItem in subList)
                    {
                        if (string.IsNullOrEmpty(subItem.Settings))
                            continue;

                        if (subItem.ItemType == "ButtonEdit")
                        {
                            string selectorName = item.Code + "_" + subItem.Code;

                            var dic = JsonHelper.ToObject<Dictionary<string, string>>(subItem.Settings);

                            string returnParams = "value:ID,text:Name";
                            if (dic.ContainsKey("ReturnParams"))
                                returnParams = dic["ReturnParams"];

                            if (dic["SelectorKey"].ToString() == "SystemUser")
                            {
                                if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                    sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                                else
                                    sb.AppendFormat("addSingleUserSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                            }
                            else if (dic["SelectorKey"].ToString() == "SystemOrg")
                            {
                                if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                    sb.AppendFormat("addMultiOrgSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                                else
                                    sb.AppendFormat("addSingleOrgSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                            }
                            else
                            {
                                string selectorKey = dic["SelectorKey"];
                                var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                                if (selector == null)
                                    throw new Exception(string.Format("弹出选择框控件“{0}”尚未配置选择页面", subItem.Name));
                                var url = selector.URLSingle;
                                if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                    url = selector.URLMulti;

                                sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}',returnParams:'{5}' }});\n"
                                    , selectorName, url, selector.Title, selector.Width, selector.Height, returnParams);
                            }
                        }
                    }
                }

            }

            #endregion

            #region 添加系统枚举

            var enumNameList = new List<string>();

            IEnumService enumService = FormulaHelper.GetService<IEnumService>();

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Settings))
                    continue;
                if (item.ItemType == "CheckBoxList" || item.ItemType == "RadioButtonList" || item.ItemType == "ComboBox")
                {
                    var dic = JsonHelper.ToObject(item.Settings);
                    var data = dic["data"].ToString().Trim();
                    if (!data.StartsWith("["))
                    {
                        if (!enumNameList.Contains(data))
                        {
                            if (isOutput == false)
                            {
                                if (!string.IsNullOrEmpty(data))
                                {
                                    var enumItems = enumService.GetEnumTable(data);
                                    sb.AppendFormat("var {0} = {1};\n", data.Split('.').Last(), JsonHelper.ToJson(enumItems));
                                    enumNameList.Add(data);
                                }
                            }
                            else
                            {
                                sb.AppendFormat("@Html.GetEnum('{0}')", data);
                            }
                        }
                    }
                }

                if (item.ItemType == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item.Settings);
                    var subTableItems = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    foreach (var subItem in subTableItems)
                    {
                        if (string.IsNullOrEmpty(subItem.Settings))
                            continue;
                        if (subItem.ItemType == "ComboBox")
                        {
                            var dic = JsonHelper.ToObject(subItem.Settings);
                            var data = dic["data"].ToString().Trim();
                            if (!data.StartsWith("["))
                            {
                                if (!enumNameList.Contains(data))
                                {
                                    if (isOutput == false)
                                    {
                                        if (!string.IsNullOrEmpty(data))
                                        {
                                            var enumItems = enumService.GetEnumTable(data);
                                            sb.AppendFormat("var {0} = {1};\n", data.Split('.').Last(), JsonHelper.ToJson(enumItems));
                                            enumNameList.Add(data);
                                        }
                                    }
                                    else
                                    {
                                        sb.AppendFormat("@Html.GetEnum('{0}')", data);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            sb.Append(@"
function commitGridEdit(gridId) { 
    var grid = mini.get(gridId);        
    grid.commitEdit();    
}
function moveUp(gridId) {
    var dataGrid = mini.get(gridId);
    var rows = dataGrid.getSelecteds();
    dataGrid.moveUp(rows);
}
function moveDown(gridId) {
    var dataGrid = mini.get(gridId);
    var rows = dataGrid.getSelecteds();
    dataGrid.moveDown(rows);
}    
");
            sb.AppendLine();
            sb.Append(HttpContext.Current.Server.HtmlDecode(form.ScriptText));

            return sb.ToString();
        }

        #endregion

        #region GetDefaultValue

        public string GetDefaultValue(string code, string defaultValueTmpl, List<DataRow> rows)
        {
            if (string.IsNullOrEmpty(defaultValueTmpl))
                return "";

            string result = "";
            if (rows.Count == 0)
            {
                result = ReplaceString(defaultValueTmpl, null);
            }
            else
            {

                foreach (var row in rows)
                {
                    result = ReplaceString(defaultValueTmpl, row);
                    if (result != defaultValueTmpl)
                        break;
                }
            }


            return result;
        }

        #endregion

        #region GetDefaultValueRows

        public List<DataRow> GetDefaultValueRows(string DefaultValueSettings)
        {
            if (string.IsNullOrEmpty(DefaultValueSettings))
                return new List<DataRow>();

            List<DataRow> defaultValueRows = new List<DataRow>();
            foreach (var item in JsonHelper.ToList(DefaultValueSettings))
            {
                SQLHelper tmpSQLHelper = SQLHelper.CreateSqlHelper(item["ConnName"].ToString());
                string defaultSQL = ReplaceString(item["SQL"].ToString());
                DataTable tmpDT = tmpSQLHelper.ExecuteDataTable(defaultSQL);
                if (tmpDT.Rows.Count > 0)
                    defaultValueRows.Add(tmpDT.Rows[0]);
                else
                    defaultValueRows.Add(tmpDT.NewRow());
            }

            return defaultValueRows;
        }

        #endregion

        #region 私有方法

        #region CreateSubTableHtml

        private string CreateSubTableHtml(FormItem formItem)
        {
            if (string.IsNullOrEmpty(formItem.Settings))
                return "";
            var dic = JsonHelper.ToObject(formItem.Settings);
            var list = JsonHelper.ToObject<List<FormItem>>(dic["listData"].ToString());
            if (list.Count == 0)
                return "";

            //默认值Dic
            var defaultDic = new Dictionary<string, string>();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.DefaultValue))
                    continue;
                defaultDic.Add(item.Code, GetDefaultValue(item.Code, item.DefaultValue, new List<DataRow>()));
            }


            string miniuiSettings = GetMiniuiSettings(dic["formData"].ToString());
            if (miniuiSettings == "")
                miniuiSettings = "style='width:100%;height:100px;'";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
<div class='mini-toolbar'  style='border-bottom: 0px;'>
    <table>
        <tr>
            <td>
                <a class='mini-button' id='btn{3}Add' iconcls='icon-add' onclick='addRow({0});'>添加</a>
                <a class='mini-button' id='btn{3}Delete' iconcls='icon-remove' onclick='delRow({1});'>移除</a>
                <a class='mini-button' iconcls='icon-up' onclick='moveUp({2});'>上移</a>
                <a class='mini-button' iconcls='icon-down' onclick='moveDown({2});'>下移</a>
            </td>
            <td>
            </td>
        </tr>
    </table>
</div>
", JsonHelper.ToJson(defaultDic) + ",{gridId:\"" + formItem.Code + "\",isLast:true}"
 , "{gridId:\"" + formItem.Code + "\"}"
 , "\"" + formItem.Code + "\""
 , formItem.Code
 );



            sb.AppendFormat(@" 
        <div id='{0}' {1} {2} {3} class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true' allowcellselect='true' showpager='false' allowUnselect='false'>
 ", formItem.Code, miniuiSettings
  , formItem.Enabled == "true" ? "" : "enabled='false'"
  , formItem.Visible == "true" ? "" : "visible='false'");

            sb.AppendFormat(@"
        <div property='columns'>
            <div type='checkcolumn'></div>
");

            foreach (var item in list)
            {
                #region 控件类型为复选框时

                if (item.ItemType == "CheckBox")
                {
                    sb.AppendFormat("\n<div type='checkboxcolumn' field='{0}' header='{1}' {2} {3}></div>"
                        , item.Code
                        , item.Name
                        , GetMiniuiSettings(JsonHelper.ToJson(item))
                        , GetMiniuiSettings(item.Settings)
                        );
                    continue;
                }

                #endregion

                miniuiSettings = GetMiniuiSettings(item.Settings ?? "");
                if (miniuiSettings == "")
                    miniuiSettings = "style='width:100%'";

                string ColumnSettings = GetMiniuiSettings(item.ColumnSettings ?? "");//列格式

                #region 获取vtype
                string vtype = "";
                if (!string.IsNullOrEmpty(item.Settings))
                {
                    var _dic = JsonHelper.ToObject<Dictionary<string, string>>(item.Settings);
                    if (_dic.ContainsKey("required") && _dic["required"] == "true")
                        vtype += "required;";
                    if (_dic.ContainsKey("vtype"))
                        vtype += _dic["vtype"];
                }
                #endregion


                sb.AppendFormat(@"
        <div field='{3}' {8} header='{4}' {5} {6} {7} {0} autoShowPopup='true' {12} {13}>
                <input {9} property='editor' class='mini-{1}' {2} {10} {11}/>
        </div>
"
                    , GetMiniuiSettings(JsonHelper.ToJson(item))
                    , item.ItemType.ToLower()
                    , miniuiSettings
                    , item.Code
                    , item.Name
                    , item.ItemType == "DatePicker" ? "dateFormat='yyyy-MM-dd'" : ""
                    , item.ItemType == "ComboBox" ? "type='comboboxcolumn'" : ""
                    , vtype == "" ? "" : string.Format("vtype='{0}'", vtype)
                    , item.ItemType == "ButtonEdit" ? "displayfield='" + item.Code + "Name'" : ""
                    , item.ItemType == "ButtonEdit" ? "allowInput='false' name='" + formItem.Code + "_" + item.Code + "'" : ""  //列表上选择暂时不支持智能感知，因此先加allowInput
                    , item.ItemType == "ComboBox" ? "onitemclick=\"commitGridEdit('" + formItem.Code + "');\"" : ""
                    , item.ItemType == "DatePicker" ? "onhidepopup=\"commitGridEdit('" + formItem.Code + "');\"" : ""
                    , string.IsNullOrEmpty(item.SummaryType) ? "" : string.Format("summaryType='{0}' summaryRenderer='onSummaryRenderer'", item.SummaryType)
                    , ColumnSettings
                    );
            }

            sb.AppendFormat(@"
        
    </div>
</div>
");
            return sb.ToString();
        }

        #endregion


        #endregion

        #endregion

        #region 列表Html及脚本

        public string CreateListHtml(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));
            var fields = JsonHelper.ToList(listDef.LayoutField);
            var buttons = JsonHelper.ToList(listDef.LayoutButton);

            //详细查询字段
            var queryFields = fields.Where(c => c.ContainsKey("ItemType") && c["ItemType"].ToString() != "");
            //快速查询字段
            var quickQueryFields = fields.Where(c => c.ContainsKey("AllowSearch") && c["AllowSearch"].ToString() == "true");

            #region QueryForm

            StringBuilder sbQuery = new StringBuilder();

            int i = 0;

            foreach (var item in queryFields)
            {
                if (i % 2 == 0)
                    sbQuery.Append("<tr>");

                if (i + 1 % 2 == 0)
                    sbQuery.Append("<td width=\"5%\" /></tr>");
                else
                {
                    sbQuery.Append(GetQueryItemHtml(item));
                }

                i++;
            }

            string queryHtml = @"
<div id='queryWindow' class='mini-window' title='详细查询' style='width: 690px; height: @{1}px;'>
    <div class='queryDiv'>
        <form id='queryForm' method='post'>
        <table>
           {0}
        </table>
        </form>
        <div>
            <a class='mini-button' onclick='search()' iconcls='icon-find' style='margin-right: 20px;'>查询</a>
            <a class='mini-button' onclick='clearQueryForm()' iconcls='icon-undo'>清空</a>
        </div>
    </div>
</div>";
            string strQueryForm = string.Format(queryHtml, sbQuery, 100 + 22 * (queryFields.Count() / 2));

            #endregion

            #region Bar条

            StringBuilder sbButtons = new StringBuilder();
            foreach (var item in buttons)
            {
                string onclick = "";
                if (item.ContainsKey("URL") && !string.IsNullOrEmpty(item["URL"].ToString()))
                {
                    onclick = "onclick='openWindow(\"" + item["URL"] + "\"";
                    if (item.ContainsKey("Settings"))
                    {
                        var sets = JsonHelper.ToObject(item["Settings"].ToString());
                        if (sets.ContainsKey("Field") && !string.IsNullOrEmpty(sets["Field"].ToString()))
                            continue;

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        if (sets.ContainsKey("PopupWidth") && sets["PopupWidth"].ToString() != "")
                            dic.Add("width", sets["PopupWidth"].ToString());
                        else
                            dic.Add("width", "1000");
                        if (sets.ContainsKey("PopupHeight") && sets["PopupHeight"].ToString() != "")
                            dic.Add("height", sets["PopupHeight"].ToString());
                        if (sets.ContainsKey("PopupTitle") && sets["PopupTitle"].ToString() != "")
                            dic.Add("title", sets["PopupTitle"].ToString());
                        if (sets.ContainsKey("Confirm") && sets["Confirm"].ToString() == "true")
                            dic.Add("mustConfirm", "true");
                        if (sets.ContainsKey("SelectMode") && sets["SelectMode"].ToString() != "")
                            dic.Add(sets["SelectMode"].ToString(), "true");
                        onclick += "," + JsonHelper.ToJson(dic);
                    }
                    else
                    {
                        onclick += ",{width:1000}";
                    }

                    onclick += ");'";
                }

                sbButtons.AppendFormat("\n<a class='mini-button' plain='true' {0} {1} {2}></a>"
                    , GetMiniuiSettings(item)
                    , onclick
                    , item.ContainsKey("Settings") ? GetMiniuiSettings(item["Settings"].ToString()) : "");
            }

            StringBuilder sb = new StringBuilder();

            string strBar = @"
<div class='mini-toolbar gw-grid-toolbar'>
    <table>
        <tr>
            <td>
                {0}
            </td>
            <td class='gw-toolbar-right'>
            {1}
            {2}
            </td>
        </tr>
    </table>
</div>";

            string strQuickQueryBox = string.Format("<input id='key' class='mini-buttonedit gw-searchbox' emptytext='请输入{0}' onenter=\"quickSearch('{1}');\" onbuttonclick=\"quickSearch('{1}');\" />"
                , string.Join("或", quickQueryFields.Select(c => c["header"].ToString()).ToArray())
                , string.Join(",", quickQueryFields.Select(c => c["field"].ToString()).ToArray())
                );
            string strSearchButton = "<a class='mini-button' onclick=\"showWindow('queryWindow');\" iconcls='icon-find'>详细查询</a>";

            strBar = string.Format(strBar
                , sbButtons
                , quickQueryFields.Count() > 0 ? strQuickQueryBox : ""
                , queryFields.Count() > 0 ? strSearchButton : ""
                );

            #endregion

            #region Grid

            StringBuilder sbField = new StringBuilder();

            foreach (var field in fields)
            {
                sbField.AppendFormat("<div {0} {1}></div>", GetMiniuiSettings(field)
                    , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : "");
            }

            string strGrid = @"
<div class='mini-fit'>
    <div id='dataGrid' class='mini-datagrid' style='width: 100%; height: 100%;' url='GetList' {0} onDrawSummaryCell='onDrawSummaryCell'>
        <div property='columns'>           
            <div type='checkcolumn'></div>
            {1}
        </div>
    </div>
</div>";
            strGrid = string.Format(strGrid
                , GetMiniuiSettings(listDef.LayoutGrid)
                , sbField
                );

            #endregion

            return strBar + "\n" + strGrid + "\n" + strQueryForm + "\n" + createExportExcelbtn(code);
        }

        public string CreateListScript(string code, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("document.title='{0}';", listDef.Name);

            var fields = JsonHelper.ToList(listDef.LayoutField);

            #region 字段详细
            foreach (var field in fields)
            {
                if (!field.ContainsKey("Settings"))
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());

                if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                    continue;

                string enumKey = settings["EnumKey"].ToString();
                if (!enumKey.StartsWith("["))
                {
                    if (isOutput == false)
                    {
                        IList<DicItem> dt = new List<DicItem>();
                        if (!string.IsNullOrEmpty(enumKey))
                            dt = FormulaHelper.GetService<IEnumService>().GetEnumDataSource(enumKey, "", "");
                        enumKey = enumKey.Split('.').Last();
                        var result = string.Format("\n var {0} = {1};", enumKey, JsonHelper.ToJson(dt));
                        sb.Append(result);
                        sb.AppendFormat("\n addGridEnum('dataGrid', '{0}', '{1}');", field["field"], enumKey);
                    }
                    else
                    {
                        sb.AppendFormat("\n @Html.GetEnum('{0}')", enumKey);
                        sb.AppendFormat("\n addGridEnum('dataGrid', '{0}', '{1}');", field["field"], enumKey.Split('.').LastOrDefault());
                    }


                }
                else
                {
                    var result = string.Format("\n var {0} = {1};", "enum_" + field["field"], enumKey);
                    sb.Append(result);
                    sb.AppendFormat("\n addGridEnum('dataGrid', '{0}', '{1}');", field["field"], "enum_" + field["field"]);
                }

            }

            #endregion

            #region 按钮
            sb.AppendLine();
            var buttons = JsonHelper.ToList(listDef.LayoutButton);
            foreach (var item in buttons)
            {

                if (item.ContainsKey("URL") && !string.IsNullOrEmpty(item["URL"].ToString()))
                {
                    if (item.ContainsKey("Settings"))
                    {
                        var sets = JsonHelper.ToObject(item["Settings"].ToString());
                        if (!sets.ContainsKey("Field") || string.IsNullOrEmpty(sets["Field"].ToString()))
                            continue;

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        if (sets.ContainsKey("PopupWidth") && sets["PopupWidth"].ToString() != "")
                            dic.Add("width", sets["PopupWidth"].ToString());
                        else
                            dic.Add("width", "1000");
                        if (sets.ContainsKey("PopupHeight") && sets["PopupHeight"].ToString() != "")
                            dic.Add("height", sets["PopupHeight"].ToString());
                        if (sets.ContainsKey("PopupTitle") && sets["PopupTitle"].ToString() != "")
                            dic.Add("title", sets["PopupTitle"].ToString());
                        if (sets.ContainsKey("Confirm") && sets["Confirm"].ToString() == "true")
                            dic.Add("mustConfirm", "true");
                        if (sets.ContainsKey("SelectMode") && sets["SelectMode"].ToString() != "")
                            dic.Add(sets["SelectMode"].ToString(), "true");

                        var displayContent = "";
                        if (sets.ContainsKey("DisplayContent") && sets["DisplayContent"].ToString() != "")
                            displayContent = sets["DisplayContent"].ToString();
                        if (displayContent == "buttonIcon")
                            dic.Add("buttonClass", item["iconcls"]);
                        else if (displayContent == "buttonName")
                            dic.Add("linkText", item["text"]);
                        string addGridLink = string.Format("\n addGridLink('dataGrid','{0}','{1}',{2});", sets["Field"], item["URL"], JsonHelper.ToJson(dic));

                        sb.Append(addGridLink);
                    }

                }
            }

            #endregion

            #region 详细查询
            sb.AppendLine();
            foreach (var field in fields)
            {
                if (!field.ContainsKey("ItemType") || field["ItemType"].ToString() != "ButtonEdit")
                    continue;

                if (!field.ContainsKey("QuerySettings"))
                    continue;
                string mode = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
                string queryMode = getQueryMode(mode);
                string name = string.Format("${0}${1}", queryMode, field["field"]);

                var dic = JsonHelper.ToObject<Dictionary<string, string>>(field["QuerySettings"].ToString());
                if (dic["SelectorKey"].ToString() == "SystemUser")
                {
                    if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                        sb.AppendFormat("addMultiUserSelector('{0}');\n", name);
                    else
                        sb.AppendFormat("addSingleUserSelector('{0}');\n", name);
                }
                else if (dic["SelectorKey"].ToString() == "SystemOrg")
                {
                    if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                        sb.AppendFormat("addMultiOrgSelector('{0}');\n", name);
                    else
                        sb.AppendFormat("addSingleOrgSelector('{0}');\n", name);
                }
                else
                {
                    sb.AppendFormat("addSelector('{0}', '/MvcConfig/UI/Selector/PageView?TmplCode={1}', {2});\n"
                        , name, dic["SelectorKey"], "{ allowResize: true,title:'请选择' }");
                }


            }

            #endregion


            return sb.ToString() + "\n" + HttpContext.Current.Server.HtmlDecode(listDef.ScriptText);

        }

        private string GetQueryItemHtml(Dictionary<string, object> field)
        {
            string mode = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
            string queryMode = getQueryMode(mode);


            string miniCls = field["ItemType"].ToString().ToLower();
            string enumKey = "";
            if (field.ContainsKey("Settings"))
            {
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("EnumKey"))
                {
                    string s = settings["EnumKey"].ToString();
                    if (s.StartsWith("["))
                        enumKey = string.Format("data='{0}'", settings["EnumKey"]);
                    else
                        enumKey = string.Format("data='{0}'", s.Split('.').Last());
                }
            }

            string code = field["field"].ToString();
            string name = field["header"].ToString();

            string html = "";

            string miniuiSettings = GetMiniuiSettings(field.ContainsKey("QuerySettings") ? field["QuerySettings"].ToString() : "{}");

            html = string.Format("<input name=\"${0}${1}\" class=\"mini-{2}\" {3} {4} style='width:100%' />"
   , queryMode, code, miniCls, miniuiSettings, enumKey);


            if (field["QueryMode"].ToString() == "Between")
            {
                html = string.Format("<input name=\"${0}${1}\" class=\"mini-{2}\" {4} style='width:45%'/>到<input name=\"${3}${1}\" class=\"mini-{2}\" {4} style='width:45%'/>"
                  , "FR", code, miniCls, "LT", miniuiSettings);
            }


            return string.Format("<td width=\"15%\">{0}</td><td width=\"35%\" nowrap=\"nowrap\">{1}</td>", name, html);
        }

        private string getQueryMode(string mode)
        {
            string queryMode = "";
            switch (mode)
            {
                case "Equal":
                    queryMode = "EQ";
                    break;
                case "NotEqual":
                    queryMode = "UQ";
                    break;
                case "GreaterThan":
                    queryMode = "GT";
                    break;
                case "LessThan":
                    queryMode = "LT";
                    break;
                case "In":
                    queryMode = "IN";
                    break;
                case "GreaterThanOrEqual":
                    queryMode = "FR";
                    break;
                case "LessThanOrEqual":
                    queryMode = "TO";
                    break;
                case "Like":
                    queryMode = "LK";
                    break;
                case "StartsWith":
                    queryMode = "SW";
                    break;
                case "EndsWith":
                    queryMode = "EW";
                    break;
                case "IGNORE":
                    queryMode = "";
                    break;
                default:
                    queryMode = "LK";
                    break;
            }
            return queryMode;
        }

        private string createExportExcelbtn(string code)
        {
            StringBuilder sb = new StringBuilder();

            string includeColumns = "";
            string excelKey = "";
            string gridId = "dataGrid";
            //string text = "导出";

            includeColumns = !string.IsNullOrWhiteSpace(includeColumns) && !includeColumns.EndsWith(",") ? includeColumns + "," : includeColumns;
            excelKey = code;
            //var btnExportHTML = "<a id='btnExport' class='mini-button' iconcls='icon-excel-export' plain='true' onclick=\"ExportExcel('{0}', '{1}', '{2}')\">{3}</a>";

            var strFormHTML = @"    
    <!--导出Excel——模拟异步ajax提交表单 -->
    <form id='excelForm{0}' style='display:none;' action='/MvcConfig/Aspose/ExportExcel' method='post' target='excelIFrame{0}'>
        <input type='hidden' name='jsonColumns' />
        <input type='hidden' name='title' />
        <input type='hidden' name='excelKey' />
        <input type='hidden' name='queryFormData' />
        <input type='hidden' name='sortOrder' />
        <input type='hidden' name='sortField' />
        <input type='hidden' name='dataUrl' />
    </form>
    <iframe id='excelIFrame{0}' name='excelIFrame{0}' style='display:none;'></iframe>";

            sb.AppendLine(string.Format(strFormHTML, excelKey));

            var strExcelWindowHTML = @"
<!--导出Excel——自定义删选字段-->
<div id='excelWindow{2}' class='mini-window' title='导出数据' style='width: 262px; height: 270px; display:none;'
    showmodal='true' allowresize='false' allowdrag='true'>
    <div id='gridColumns{2}' class='mini-listbox' style='width: 250px; height: 200px;' showcheckbox='true'
        multiselect='true' textfield='ChineseName' valuefield='FieldName'>
    </div>
    <div style='float: right; padding-top: 6px;'>
        <a class='mini-button' iconcls='icon-excel' plain='false' onclick='{0}'>
            导出</a>
        <a class='mini-button' iconcls='icon-cancel' plain='false' onclick='{1}'>
            取消</a>
    </div>
</div>";

            sb.AppendLine(string.Format(strExcelWindowHTML,
                string.Format("downloadExcelData(\"{0}\",\"{1}\");", excelKey, gridId), string.Format("closeExcelWindow(\"{0}\")", excelKey), excelKey));

            //sb.AppendLine(string.Format(btnExportHTML, excelKey, gridId, includeColumns.ToLower(), text));

            return sb.ToString();
        }

        #endregion

        #region Settings处理

        public string GetMiniuiSettings(string settings)
        {
            if (string.IsNullOrEmpty(settings))
                return "";
            var dic = JsonHelper.ToObject(settings);
            return GetMiniuiSettings(dic);
        }

        public string GetMiniuiSettings(Dictionary<string, object> dic)
        {
            if (dic.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            string style = "";
            foreach (string key in dic.Keys)
            {
                if (key != "Enabled" && key != "Visible" && key.StartsWith(key.Substring(0, 1).ToUpper())) //以大写字符开头的不处理
                    continue;

                if (dic[key] != null && dic[key].ToString() != "")
                {
                    if (key == "data")
                    {
                        if (dic[key].ToString().StartsWith("["))
                            sb.AppendFormat(" {0}='{1}'", key, dic[key]);
                        else
                            sb.AppendFormat(" {0}='{1}'", key, dic[key].ToString().Split('.').Last()); //以逗号隔开的枚举名
                    }
                    else if (key == "readonly")
                    {
                        if (dic[key].ToString() == "true")
                            sb.AppendFormat(" {0}='{1}'", key, dic[key]);
                    }
                    else
                    {
                        if (key.StartsWith("style_"))
                            style += string.Format("{0}:{1};", key.Split('_')[1], dic[key]);
                        else
                            sb.AppendFormat(" {0}='{1}'", key.ToLower(), dic[key]);
                    }
                }
            }

            string result = sb.ToString();
            if (!string.IsNullOrEmpty(style))
                result += " style='" + style + "'";
            return result;
        }


        #endregion

        #region 获取拼音首字符

        public string GetHanZiPinYinString(string name)
        {
            List<string> list = new List<string>();
            var arr = name.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                if (c >= '0' && c <= '9')
                {
                    list.Add(c.ToString());
                }
                else if (c >= 'a' && c <= 'z')
                {
                    list.Add(c.ToString());
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    list.Add(c.ToString());
                }
                else if (c == '_')
                {
                    list.Add(c.ToString());
                }
                else
                {
                    list.Add(getHanZiPin(c.ToString()));
                }
            }

            string result = string.Join("", list);
            result = result.Replace(" ", "");
            return result;
        }

        private string getHanZiPin(string c)
        {
            if ("()（），。、；,.;".Contains(c))
                return "";

            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            if (array.Length == 1)
                return "";
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

            if (i < 0xB0A1) return "$";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "J";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "$";
        }


        //private string getHanZiPin(string str)
        //{
        //    if (str.CompareTo("吖") < 0) return "";
        //    if (str.CompareTo("八") < 0) return "A";
        //    if (str.CompareTo("嚓") < 0) return "B";
        //    if (str.CompareTo("咑") < 0) return "C";
        //    if (str.CompareTo("妸") < 0) return "D";
        //    if (str.CompareTo("发") < 0) return "E";
        //    if (str.CompareTo("旮") < 0) return "F";
        //    if (str.CompareTo("铪") < 0) return "G";
        //    if (str.CompareTo("讥") < 0) return "H";
        //    if (str.CompareTo("咔") < 0) return "J";
        //    if (str.CompareTo("垃") < 0) return "K";
        //    if (str.CompareTo("嘸") < 0) return "L";
        //    if (str.CompareTo("拏") < 0) return "M";
        //    if (str.CompareTo("噢") < 0) return "N";
        //    if (str.CompareTo("妑") < 0) return "O";
        //    if (str.CompareTo("七") < 0) return "P";
        //    if (str.CompareTo("亽") < 0) return "Q";
        //    if (str.CompareTo("仨") < 0) return "R";
        //    if (str.CompareTo("他") < 0) return "S";
        //    if (str.CompareTo("哇") < 0) return "T";
        //    if (str.CompareTo("夕") < 0) return "W";
        //    if (str.CompareTo("丫") < 0) return "X";
        //    if (str.CompareTo("帀") < 0) return "Y";
        //    if (str.CompareTo("咗") < 0) return "Z";
        //    return "";

        //    //byte[] arrCN = Encoding.Default.GetBytes(cnChar);
        //    //if (arrCN.Length > 1)
        //    //{
        //    //    int area = (short)arrCN[0];
        //    //    int pos = (short)arrCN[1];
        //    //    int code = (area << 8) + pos;
        //    //    int[] areacode = new int[] { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

        //    //    for (int i = 0; i < 26; i++)
        //    //    {
        //    //        int max = 55290;
        //    //        if (i != 25) max = areacode[i + 1];
        //    //        if (areacode[i] <= code && code < max)
        //    //        {
        //    //            return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
        //    //        }
        //    //    }
        //    //    return "";
        //    //}

        //    //else return cnChar;

        //}

        #endregion

        #region String替换

        /// <summary>
        /// 替换{}内容为当前地址栏参数或当前人信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string ReplaceString(string sql, DataRow row = null, Dictionary<string, string> dic = null)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];

                if (row != null && row.Table.Columns.Contains(value))
                    return row[value].ToString();
                if (dic != null && dic.ContainsKey(value))
                    return dic[value];

                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    default:
                        return m.Value;
                }
            });

            return result;
        }


        #endregion

        #region Word导出

        public DataSet GetWordDataSource(string code, string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var word = entities.Set<S_UI_Word>().SingleOrDefault(c => c.Code == code);
            if (word == null)
                throw new Exception(string.Format("编号为：“{0}”的Word导出不存在", code));

            if (word.Description == "FormWord")
                return GetFormWordDataSource(code, id); //表单定义的Word导出数据

            var enumService = FormulaHelper.GetService<IEnumService>();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(word.ConnName);
            string sql = string.Format("select * from ({0}) a where ID='{1}'", word.SQL, id);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            dtForm.TableName = "dtForm";
            if (dtForm.Rows.Count == 0)
                throw new Exception("不存在ID为：" + id + "的记录！");

            DataSet ds = new DataSet("dataSet1");
            ds.Tables.Add(dtForm);


            var items = JsonHelper.ToObject<List<Dictionary<string, string>>>(word.Items);
            foreach (var item in items)
            {
                string fieldCode = item["Code"];


                if (item["ItemType"] == "Datetime")
                {
                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        string format = "yyyy-MM-dd";
                        var settings = new Dictionary<string, string>();
                        if (item.ContainsKey("Settings"))
                            settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                        if (settings.ContainsKey("format"))
                            format = settings["format"];

                        dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString(format);
                    }

                }
                else if (item["ItemType"] == "Float")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);

                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        dtForm.Rows[0][fieldCode] = Convert.ToDouble(fieldValue).ToString(settings["format"]);
                    }
                }
                else if (item["ItemType"] == "Enum")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                    #region 处理枚举

                    IList<DicItem> enumData = GetEnum(settings["data"]);

                    if (settings["isCheckBox"] == "true")
                    {
                        StringBuilder sb = new StringBuilder();
                        var value = dtForm.Rows[0][fieldCode].ToString();

                        int repeatItems = 0;
                        if (settings.ContainsKey("repeatItems"))
                            repeatItems = int.Parse(settings["repeatItems"]);
                        for (int i = 0; i < enumData.Count; i++)
                        {
                            var d = enumData[i];
                            if (value.Split(',').Contains(d.Value))
                                sb.AppendFormat(" √{0}", d.Text);
                            else
                                sb.AppendFormat(" □{0}", d.Text);

                            if (repeatItems > 0 && (i + 1) % repeatItems == 0 && i + 1 < enumData.Count)
                                sb.AppendLine();

                        }
                        dtForm.Rows[0][fieldCode] = sb.ToString();
                    }
                    else
                    {
                        var value = dtForm.Rows[0][fieldCode].ToString();
                        foreach (var d in enumData)
                        {
                            if (d.Value == value)
                                dtForm.Rows[0][fieldCode] = d.Text;
                        }
                    }
                    #endregion
                }
                else if (item["ItemType"] == "SubTable" || item["ItemType"] == "FieldTable")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, object>>(item["Settings"]);
                    var formData = JsonHelper.ToObject<Dictionary<string, string>>(settings["formData"].ToString());
                    var listData = JsonHelper.ToObject<List<Dictionary<string, string>>>(settings["listData"].ToString());

                    sql = ReplaceString(formData["SQL"], dtForm.Rows[0]);
                    DataTable subDT = sqlHelper.ExecuteDataTable(sql);

                    #region 大字段表数据

                    if (item["ItemType"] == "FieldTable")
                    {
                        if (subDT.Rows.Count == 1)
                        {
                            string json = subDT.Rows[0][0].ToString();
                            if (json == "")
                                json = "[]";
                            var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(json);
                            subDT.Columns.Clear();
                            subDT.Rows.Clear();
                            if (list.Count > 0)
                            {
                                foreach (var c in list[0].Keys)
                                {
                                    subDT.Columns.Add(c);
                                }
                                foreach (var r in list)
                                {
                                    var row = subDT.NewRow();
                                    foreach (var c in r.Keys)
                                    {
                                        row[c] = r[c];
                                    }
                                    subDT.Rows.Add(row);
                                }
                            }
                        }
                    }

                    #endregion

                    if (!subDT.Columns.Contains("RowNumber"))
                    {
                        subDT.Columns.Add("RowNumber");
                        for (int i = 0; i < subDT.Rows.Count; i++)
                            subDT.Rows[i]["RowNumber"] = i + 1;
                    }

                    subDT.TableName = fieldCode;
                    ds.Tables.Add(subDT);


                    foreach (var subItem in listData)
                    {
                        string subFieldCode = subItem["Code"];
                        if (subDT.Columns.Contains(subFieldCode) == false)
                            continue;

                        var subSettings = JsonHelper.ToObject<Dictionary<string, string>>(subItem["Settings"]);

                        if (subItem["ItemType"] == "Enum")
                        {
                            #region 处理枚举

                            if (subSettings["isCheckBox"] == "true")
                            {
                                foreach (DataRow subRow in subDT.Rows)
                                {
                                    string subValue = subRow[subFieldCode].ToString();
                                    if (subValue == "1" || subValue == "T" || subValue.ToLower() == "true")
                                        subRow[subFieldCode] = "√";
                                }
                            }
                            else
                            {
                                IList<DicItem> SubEnumData = GetEnum(subSettings["data"]);

                                foreach (DataRow subRow in subDT.Rows)
                                {
                                    string[] subValues = subRow[subFieldCode].ToString().Split(',');
                                    var v = string.Join(",", SubEnumData.Where(c => subValues.Contains(c.Value)).Select(c => c.Text).ToArray());
                                    if (v != "")
                                        subRow[subFieldCode] = v;
                                }
                            }
                            #endregion
                        }
                        else if (subItem["ItemType"] == "Datetime")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDateTime(values[i]).ToString(subSettings["format"]);
                            }
                        }
                        else if (subItem["ItemType"] == "Float")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDouble(values[i]).ToString(subSettings["format"]);
                            }
                        }
                    }

                }
            }

            return ds;
        }

        /// <summary>
        /// 根据配置数据获取枚举
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IList<DicItem> GetEnum(string data)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();
            IList<DicItem> enumData = null;
            if (data.StartsWith("["))
                enumData = JsonHelper.ToObject<IList<DicItem>>(data);
            else
            {
                enumData = enumService.GetEnumDataSource(data);
            }
            return enumData;
        }


        /// <summary>
        /// 表单定义的Word导出配置
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataSet GetFormWordDataSource(string code, string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == code);
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));

            var enumService = FormulaHelper.GetService<IEnumService>();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(form.ConnName);
            string sql = string.Format("select * from {0} a where ID='{1}'", form.TableName, id);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            dtForm.TableName = "dtForm";
            if (dtForm.Rows.Count == 0)
                throw new Exception("不存在ID为：" + id + "的记录！");

            DataSet ds = new DataSet("dataSet1");
            ds.Tables.Add(dtForm);


            var items = JsonHelper.ToObject<List<Dictionary<string, string>>>(form.Items);
            foreach (var item in items)
            {
                string fieldCode = item["Code"];

                if (item["ItemType"] == "ButtonEdit")
                {
                    dtForm.Rows[0][fieldCode] = dtForm.Rows[0][fieldCode + "Name"];
                }
                else if (item["ItemType"] == "AuditSign")
                {
                    string json = dtForm.Rows[0][fieldCode].ToString();
                    if (json == "")
                        json = "[]";
                    var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(json);
                    DataTable subDT = new DataTable(fieldCode);
                    ds.Tables.Add(subDT);
                    if (list.Count > 0)
                    {
                        foreach (var c in list[0].Keys)
                        {
                            subDT.Columns.Add(c);
                        }
                        foreach (var r in list)
                        {
                            var row = subDT.NewRow();
                            foreach (var c in r.Keys)
                            {
                                row[c] = r[c];
                            }
                            subDT.Rows.Add(row);
                        }
                    }
                }
                else if (item["ItemType"] == "SingleFile" || item["ItemType"] == "MultiFile")
                {
                    string fileName = dtForm.Rows[0][fieldCode].ToString();
                    string result = "";
                    foreach (var fName in fileName.Split(','))
                    {
                        result += "\n" + fName.Substring(fName.IndexOf('_') + 1);
                    }
                    dtForm.Rows[0][fieldCode] = result.Trim('\n', ' ');
                }
                else if (item["ItemType"] == "DatePicker") //日期框
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);

                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        if (settings != null)
                            dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString(settings["format"]);
                        else
                            dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString("yyyy-MM-dd");
                    }
                }
                else if (item["ItemType"] == "CheckBox") //复选框
                {
                    var value = dtForm.Rows[0][fieldCode].ToString();
                    if (value == "1")
                        dtForm.Rows[0][fieldCode] = string.Format(" √{0}", item["Name"]);
                    else
                        dtForm.Rows[0][fieldCode] = string.Format(" □{0}", item["Name"]);
                }
                else if (item["ItemType"] == "CheckBoxList" || item["ItemType"] == "RadioButtonList")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                    #region 处理枚举

                    IList<DicItem> enumData = GetEnum(settings["data"]);

                    StringBuilder sb = new StringBuilder();
                    var value = dtForm.Rows[0][fieldCode].ToString();
                    int repeatItems = 0;
                    if (settings.ContainsKey("repeatItems") && settings["repeatItems"] != "")
                        repeatItems = int.Parse(settings["repeatItems"]);
                    for (int i = 0; i < enumData.Count; i++)
                    {
                        var d = enumData[i];
                        if (value.Split(',').Contains(d.Value))
                            sb.AppendFormat(" √{0}", d.Text);
                        else
                            sb.AppendFormat(" □{0}", d.Text);

                        if (repeatItems > 0 && (i + 1) % repeatItems == 0 && i + 1 < enumData.Count)
                            sb.AppendLine();

                    }
                    dtForm.Rows[0][fieldCode] = sb.ToString();

                    #endregion
                }
                else if (item["ItemType"] == "ComboBox")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                    #region 处理枚举

                    IList<DicItem> enumData = GetEnum(settings["data"]);


                    var value = dtForm.Rows[0][fieldCode].ToString();
                    foreach (var d in enumData)
                    {
                        if (d.Value == value)
                            dtForm.Rows[0][fieldCode] = d.Text;
                    }

                    #endregion
                }
                else if (item["ItemType"] == "SubTable")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, object>>(item["Settings"]);
                    var formData = JsonHelper.ToObject<Dictionary<string, string>>(settings["formData"].ToString());
                    var listData = JsonHelper.ToObject<List<Dictionary<string, string>>>(settings["listData"].ToString());

                    sql = string.Format("select * from {0}_{1} where {0}ID = '{2}'", form.TableName, fieldCode, id);
                    DataTable subDT = sqlHelper.ExecuteDataTable(sql);

                    if (!subDT.Columns.Contains("RowNumber"))
                    {
                        subDT.Columns.Add("RowNumber");
                        for (int i = 0; i < subDT.Rows.Count; i++)
                            subDT.Rows[i]["RowNumber"] = i + 1;
                    }

                    subDT.TableName = fieldCode;
                    ds.Tables.Add(subDT);


                    foreach (var subItem in listData)
                    {
                        string subFieldCode = subItem["Code"];
                        if (subDT.Columns.Contains(subFieldCode) == false)
                            continue;

                        var subSettings = JsonHelper.ToObject<Dictionary<string, string>>(subItem["Settings"]);

                        if (subItem["ItemType"] == "ComboBox")
                        {
                            #region 处理枚举

                            IList<DicItem> SubEnumData = GetEnum(subSettings["data"]);

                            foreach (DataRow subRow in subDT.Rows)
                            {
                                string[] subValues = subRow[subFieldCode].ToString().Split(',');
                                var v = string.Join(",", SubEnumData.Where(c => subValues.Contains(c.Value)).Select(c => c.Text).ToArray());
                                if (v != "")
                                    subRow[subFieldCode] = v;
                            }

                            #endregion
                        }
                        else if (subItem["ItemType"] == "DatePicker")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDateTime(values[i]).ToString(subSettings["format"]);
                            }
                        }
                        else if (subItem["ItemType"] == "CheckBox")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode].ToString()).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] == "1" ? "√" : "";
                            }
                        }
                        else if (subItem["ItemType"] == "ButtonEdit")
                        {
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = subDT.Rows[i][subFieldCode + "Name"];
                            }
                        }
                    }

                }
            }

            return ds;
        }

        #endregion
    }
}
