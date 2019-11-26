using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Formula.Helper;
using System.Data;
using Config;
using Formula;
using System.Web;
using System.IO;

public static class HtmlHelperExtend
{
    #region GetBasicInfo

    public static MvcHtmlString GetBasicInfo(this HtmlHelper html, bool currentUser = false, bool buttonAuth = false, bool distributeFile = false, bool flow = false)
    {
        StringBuilder sb = new StringBuilder();

        #region 当前用户

        if (currentUser == true && HttpContext.Current.Items["hasCurrentUser"] == null)
        {
            UserInfo user = FormulaHelper.GetUserInfo();
            sb.AppendFormat("var user = {0};\n", JsonHelper.ToJson(user));
            HttpContext.Current.Items["hasCurrentUser"] = true;
        }

        #endregion

        #region 按钮权限

        if (HttpContext.Current.Items["hasBtnAuth"] == null)
        {
            IResService resService = FormulaHelper.GetService<IResService>();

            var listRes = resService.GetRes(HttpContext.Current.Request.Url.PathAndQuery, "Button,Field,FieldEdit");
            List<Res> noneAuthList = new List<Res>();
            List<Res> readonlyList = new List<Res>();
            if (listRes.Count > 0)
            {
                UserInfo user = FormulaHelper.GetUserInfo();
                var userListRes = resService.GetRes(HttpContext.Current.Request.Url.PathAndQuery, "Button,Field,FieldEdit", user.UserID);
                noneAuthList = listRes.Where(c => c.Type == "Button" || c.Type == "Field").Where(c => userListRes.Where(d => d.ID == c.ID).Count() == 0).ToList();
                readonlyList = listRes.Where(c => c.Type == "FieldEdit").Where(c => userListRes.Where(d => d.ID == c.ID).Count() == 0).ToList();

            }

            sb.AppendFormat("var noneAuthControl='{0}';\n", string.Join(",", noneAuthList.Select(c => c.ButtonID).ToArray()));
            sb.AppendFormat("var readonlyControl='{0}';\n", string.Join(",", readonlyList.Select(c => c.ButtonID).ToArray()));

            HttpContext.Current.Items["hasBtnAuth"] = true;
        }

        #endregion

        #region 流程当前环节

        if (flow == true && HttpContext.Current.Items["hasFlowStepCode"] == null)
        {
            sb.AppendFormat("\n var FlowCurrentStepCode = '{0}';", FormulaHelper.GetService<IWorkflowService>().GetFlowCurrentStepCode());
            HttpContext.Current.Items["hasFlowStepCode"] = true;
        }

        #endregion

        return MvcHtmlString.Create(sb.ToString());

    }

    #endregion

    #region GetEnum

    public static MvcHtmlString ClearEnumCache(this HtmlHelper html, string enumKey, string category = "", string subcategory = "")
    {
        string key = string.Format("EnumJson_{0}_{1}_{2}", enumKey, category, subcategory);
        HttpRuntime.Cache.Remove(key);
        return MvcHtmlString.Create("");
    }

    public static MvcHtmlString GetEnum(this HtmlHelper html, string enumKey, string enumName = "", string category = "", string subcategory = "")
    {
        if (string.IsNullOrEmpty(enumName))
            enumName = enumKey;
        enumName = enumName.Split('.').Last();
        var json = FormulaHelper.GetService<IEnumService>().GetEnumJson(enumKey, category, subcategory);
        string result = string.Format("var {0} = {1};", enumName, json);
        return MvcHtmlString.Create(result);
    }

    public static MvcHtmlString GetEnum(this HtmlHelper html, Type type, string enumName = "")
    {
        DataTable dt = EnumBaseHelper.GetEnumTable(type);

        if (string.IsNullOrEmpty(enumName))
            enumName = type.Name;

        string result = string.Format("var {0} = {1};", enumName, JsonHelper.ToJson(dt));
        return MvcHtmlString.Create(result);
    }

    #endregion

    #region GetFlowbar

    private static string _flowbarTmpl = "";
    private static string flowbarTmpl
    {
        get
        {
            if (_flowbarTmpl == "")
            {
                string filePath = HttpContext.Current.Server.MapPath("/") + "/MvcConfig/flowBarTmpl.js";
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs);
                _flowbarTmpl = reader.ReadToEnd();
                reader.Close();
                fs.Close();
            }
            return _flowbarTmpl;
        }
    }

    public static MvcHtmlString GetFlowbar(this HtmlHelper html)
    {
        if (string.IsNullOrEmpty(HttpContext.Current.Request["FlowCode"]) && string.IsNullOrEmpty(HttpContext.Current.Request["TaskExecID"]))
            return MvcHtmlString.Create("");
        return MvcHtmlString.Create(flowbarTmpl);
    }


    #endregion

    public static MvcHtmlString GetGlobalColor(this HtmlHelper html)
    {
        var colorKey = "Global_Color";
        object cacheColor = CacheHelper.Get(colorKey);
        if (cacheColor == null)
        {
            cacheColor = System.Configuration.ConfigurationManager.AppSettings["GlobalSysColor"] ?? "Default";
            CacheHelper.Set(colorKey, cacheColor);

        }
        return MvcHtmlString.Create(cacheColor.ToString());
    }

    #region Excel导入导出
    /// <summary>
    /// 导出Excel的按钮
    /// </summary>
    /// <param name="html"></param>
    /// <param name="includeColumns">需要包含的列，默认为全部</param>
    /// <param name="excelKey">Excel对应的模板Key</param>
    /// <param name="gridId">对应的GridID</param>
    /// <returns></returns>
    public static MvcHtmlString ExportButton(this HtmlHelper html, string text = "导出", string includeColumns = "", string excelKey = null, string gridId = "dataGrid")
    {
        includeColumns = !string.IsNullOrWhiteSpace(includeColumns) && !includeColumns.EndsWith(",") ? includeColumns + "," : includeColumns;
        excelKey = string.IsNullOrWhiteSpace(excelKey) ? GetDefaultExcelTemplateKey(html) : excelKey;
        var btnExportHTML = "<a class='mini-button' iconcls='icon-excel-export' plain='true' onclick=\"ExportExcel('{0}', '{1}', '{2}')\">{3}</a>";

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

        html.ViewContext.Writer.WriteLine(string.Format(strFormHTML, excelKey));

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
        html.ViewContext.Writer.WriteLine(string.Format(strExcelWindowHTML,
            string.Format("downloadExcelData(\"{0}\",\"{1}\");", excelKey, gridId), string.Format("closeExcelWindow(\"{0}\")", excelKey), excelKey));

        return MvcHtmlString.Create(string.Format(btnExportHTML, excelKey, gridId, includeColumns.ToLower(), text));
    }

    /// <summary>
    /// 导出主从Excel的按钮
    /// </summary>
    /// <param name="html"></param>
    /// <param name="detailRelateColumn">从表关联主表的列名</param>
    /// <param name="text">导出</param>
    /// <param name="includeColumns">需要包含的列，默认为全部</param>
    /// <param name="excelKey">Excel对应的模板Key</param>
    /// <param name="masterGridId">主表GridId</param>
    /// <param name="detailGridId">从表GridId</param>
    /// <returns></returns>
    public static MvcHtmlString ExportInlineButton(this HtmlHelper html, string detailRelateColumn = "RelateID", string text = "导出", string includeColumns = "", string excelKey = null, string masterGridId = "dataGrid", string detailGridId = "detailGrid")
    {
        includeColumns = !string.IsNullOrWhiteSpace(includeColumns) && !includeColumns.EndsWith(",") ? includeColumns + "," : includeColumns;
        excelKey = string.IsNullOrWhiteSpace(excelKey) ? GetDefaultExcelTemplateKey(html) : excelKey;
        var btnExportHTML = "<a class='mini-button' iconcls='icon-excel-export' plain='true' onclick=\"ExportExcel('{0}', '{1}', '{2}', '{3}')\">{4}</a>";

        var strFormHTML = @"    
    <!--导出Excel——模拟异步ajax提交表单 -->
    <form id='excelForm{0}' style='display:none;' action='/MvcConfig/Aspose/ExportInlineExcel' method='post' target='excelIFrame{0}'>
        <input type='hidden' name='jsonColumns' />
        <input type='hidden' name='title' />
        <input type='hidden' name='excelKey' />
        <input type='hidden' name='queryFormData' />
        <input type='hidden' name='sortOrder' />
        <input type='hidden' name='sortField' />
        <input type='hidden' name='masterDataUrl' />
        <input type='hidden' name='masterColumn' />
        <input type='hidden' name='detailDataUrl' />
        <input type='hidden' name='relateColumn' />
    </form>
    <iframe id='excelIFrame{0}' name='excelIFrame{0}' style='display:none;'></iframe>";

        html.ViewContext.Writer.WriteLine(string.Format(strFormHTML, excelKey));

        var strExcelWindowHTML = @"
<!--导出Excel——自定义删选字段-->
<div id='excelWindow{2}' class='mini-window' title='导出数据' style='width: 262px; height: 270px;'
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
        html.ViewContext.Writer.WriteLine(string.Format(strExcelWindowHTML,
            string.Format("downloadExcelData(\"{0}\",\"{1}\",\"{2}\",\"{3}\");", excelKey, masterGridId, detailGridId, detailRelateColumn), string.Format("closeExcelWindow(\"{0}\")", excelKey), excelKey));

        return MvcHtmlString.Create(string.Format(btnExportHTML, excelKey, masterGridId, includeColumns.ToLower(), detailGridId, text));
    }

    /// <summary>
    /// Excel导入按钮
    /// </summary>
    /// <param name="html"></param>
    /// <param name="excelKey">Excel模板Key</param>
    /// <returns></returns>
    public static MvcHtmlString ImportButton(this HtmlHelper html, string excelKey = null, string vaildUrl = null, string saveUrl = null)
    {
        excelKey = string.IsNullOrWhiteSpace(excelKey) ? GetDefaultExcelTemplateKey(html) : excelKey;

        UrlHelper urlHelper = new UrlHelper(html.ViewContext.RequestContext);
        vaildUrl = vaildUrl ?? urlHelper.Action("VaildExcelData" + excelKey);
        saveUrl = saveUrl ?? urlHelper.Action("BatchSave" + excelKey);
        var btnExportHTML = "<a class='mini-button' iconcls='icon-excel-import' onclick=\"ImportExcel('{0}','{1}','{2}')\">导入</a>";

        return MvcHtmlString.Create(string.Format(btnExportHTML, excelKey, vaildUrl, saveUrl));
    }

    private static string GetDefaultExcelTemplateKey(HtmlHelper html, string key = "")
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            key = html.ViewContext.RouteData.Values["action"].ToString();
            if (key.ToLower() == "list")
                key = html.ViewContext.RouteData.Values["controller"].ToString();
        }

        return key;
    }
    #endregion
}
