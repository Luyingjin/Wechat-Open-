using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using System.Data;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using Aspose.Words;
using System.Text.RegularExpressions;
using Base.Logic.BusinessFacade;
using Formula.Exceptions;
using Base.Logic.Model.UI.Form;
using System.Text;

namespace Base.Areas.UI.Controllers
{
    public class WordController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name from S_M_Category where FullID like '{0}%'", "0"));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWordList(MvcAdapter.QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "select * from S_UI_Word";
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                sql += string.Format(" where CategoryID='{0}'", Request["CategoryID"]);


            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            dt.Columns.Add("HasWord");
            foreach (DataRow row in dt.Rows)
            {
                string tmplName = row["Code"].ToString() + ".docx";
                if (!string.IsNullOrEmpty(tmplName))
                {
                    string path = HttpContext.Server.MapPath("/") + "WordTemplate/" + tmplName;
                    if (System.IO.File.Exists(path))
                        row["HasWord"] = "1";
                }
            }
            GridData data = new GridData(dt);
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
            return JsonGetModel<S_UI_Word>(id);
        }

        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Word>();
            if (entities.Set<S_UI_Word>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("Word导出编号重复，Word导出名称“{0}”，Word导出编号：“{1}”", entity.Name, entity.Code));

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.Items = "[]";
            }
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Delete(string listIDs)
        {
            var ids = listIDs.Split(',');
            entities.Set<S_UI_Word>().Delete(c => ids.Contains(c.ID));
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 字段信息

        public ActionResult SettingsSubTable()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().Where(c => !string.IsNullOrEmpty(c.ParentID)).Select(c => new { value = c.ID, text = c.Name }));
            return View();
        }

        public JsonResult GetItemList(string wordID)
        {
            return Json(entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID).Items);
        }

        public JsonResult SaveItemList(string wordID, string itemList)
        {
            var word = entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID);
            word.Items = itemList;
            var user = FormulaHelper.GetUserInfo();
            word.ModifyUserID = user.UserID;
            word.ModifyUserName = user.UserName;
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

        public JsonResult UploadWord()
        {
            if (Request.Files.Count > 0)
            {
                string code = Request["TmplCode"];
                var define = entities.Set<S_UI_Word>().Where(c => c.Code == code).SingleOrDefault();

                string filePath = HttpContext.Server.MapPath("/") + "WordTemplate/";


                if (System.IO.File.Exists(filePath + define.Code + ".docx"))
                    System.IO.File.Delete(filePath + define.Code + ".docx");

                string fileFullName = HttpContext.Server.MapPath("/") + "WordTemplate/" + define.Code + ".docx";
                Request.Files[0].SaveAs(fileFullName);

                var user = FormulaHelper.GetUserInfo();
                define.ModifyUserID = user.UserID;
                define.ModifyUserName = user.UserName;
                define.ModifyTime = DateTime.Now;
                entities.SaveChanges();
            }
            return Json("");
        }

        public FileResult DownloadWord(string tmplCode)
        {
            var define = entities.Set<S_UI_Word>().Where(c => c.Code == tmplCode).SingleOrDefault();
            string filePath = HttpContext.Server.MapPath("/") + "WordTemplate/" + define.Code + ".docx";
            return File(filePath, "application/msword", define.Code + ".docx");
        }

        public JsonResult CreateWordTmpl(string tmplCode)
        {
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            if (form == null)
                throw new BusinessException(string.Format("表单定义不存在：{0}", tmplCode));
            var word = entities.Set<S_UI_Word>().SingleOrDefault(c => c.Code == tmplCode);
            if (word != null)
                throw new BusinessException(string.Format("Word导出定义已经存在：{0}", tmplCode));


            word = new S_UI_Word();
            word.ID = FormulaHelper.CreateGuid();
            word.Code = form.Code;
            word.Name = form.Name;
            word.ConnName = form.ConnName;
            word.CategoryID = form.CategoryID;
            word.Description = "FormWord"; //根据此值判断是否由表单定义生成的Word导出
            word.SQL = string.Format("select * from {0}", form.TableName);
            entities.Set<S_UI_Word>().Add(word);

            #region 创建Word导出模板

            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var uiForm = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            var formItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(uiForm.Items);

            Aspose.Words.Document doc = new Aspose.Words.Document();
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            string layout = form.Layout;

            Regex reg = new Regex("\\{[^\\{]*\\}", RegexOptions.IgnorePatternWhitespace);

            #region 处理layout中的子表

            layout = reg.Replace(layout, (Match m) =>
            {
                string text = m.Value.Trim('{', '}', ' ');
                var item = formItems.SingleOrDefault(c => c["Name"] == text);

                if (item == null)
                {
                    return m.Value;
                }
                else if (item["ItemType"] == "AuditSign")
                {
                    string tmplName = "auditSignTmpl";
                    string signTitle = "签字";
                    string width = "100%";
                    if (item.ContainsKey("Settings") && item["Settings"] != "")
                    {
                        var _dic = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                        if (_dic.ContainsKey("tmplName") && _dic["tmplName"] != "")
                            tmplName = _dic["tmplName"];
                        if (_dic.ContainsKey("signTitle") && _dic["signTitle"] != "")
                            signTitle = _dic["signTitle"];
                        if (_dic.ContainsKey("width") && _dic["width"] != "")
                            width = _dic["width"];
                    }

                    if (tmplName == "auditSignTmpl")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{TableStart:" + item["Code"] + "}");
                        sb.Append("<span>意见：{Field:SignComment}</span>");
                        sb.Append("<div>" + signTitle + "：{Field:ExecUserID}日期：{Field:SignTime}</div>");
                        sb.Append("{TableEnd:" + item["Code"] + "}");
                        return sb.ToString();
                    }
                    else if (tmplName == "auditSignSingleTmpl")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<table style=\"width:" + width + "\" cellspacing=\"0\" cellpadding=\"2\" border=\"0\">");
                        sb.AppendLine("<tr><td>");
                        sb.AppendLine(signTitle + "{" + item["Code"] + ":ExecUserID}");
                        sb.AppendLine("</td></tr><tr><td>");
                        sb.AppendLine("日期：{" + item["Code"] + ":SignTime}");
                        sb.AppendLine("</td></tr></table>");
                        return sb.ToString();
                    }
                    else
                    {
                        return "";
                    }

                }
                else if (item["ItemType"] == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item["Settings"]);
                    var subItemList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    if (subItemList.Count == 0)
                        return m.Value;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<table style=\"width:100%\" bordercolor=\"#000000\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\">");
                    sb.AppendLine("<tr>");
                    foreach (var subItem in subItemList)
                    {
                        sb.AppendFormat("<td width=\"{1}\">{0}</td>", subItem.Name, subItem.width);
                    }
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = 0; i < subItemList.Count; i++)
                    {
                        var subItem = subItemList[i];

                        sb.Append("<td>");

                        if (i == 0)//子表开始
                            sb.Append("{TableStart:" + item["Code"] + "}");

                        sb.Append("{Field:" + subItem.Code + "}");

                        if (i == subItemList.Count - 1) //子表结束
                            sb.Append("{TableEnd:" + item["Code"] + "}");

                        sb.Append("</td>");
                    }
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");
                    return sb.ToString();
                }
                else
                {
                    return m.Value;
                }
            });


            #endregion

            layout = reg.Replace(layout, (match) =>
            {
                if (match.Value.Contains("TableStart:") || match.Value.Contains("TableEnd:"))
                    return match.Value;
                else
                    return "<span>" + match.Value + "</span>";
            });

            builder.InsertHtml(layout);

            doc.Range.Replace(reg, new ReplaceToField(formItems), false);

            string path = HttpContext.Server.MapPath("/WordTemplate");
            path += "\\" + tmplCode + ".docx";
            doc.Save(path, Aspose.Words.SaveFormat.Docx);

            #endregion

            entities.SaveChanges();

            return Json("");
        }
    }


    public class ReplaceToField : IReplacingCallback
    {
        public List<Dictionary<string, string>> formItems = null;
        public ReplaceToField(List<Dictionary<string, string>> formItems)
        {
            this.formItems = formItems;
        }
        public ReplaceAction Replacing(ReplacingArgs e)
        {
            //获取当前节点            
            var node = e.MatchNode;
            //获取当前文档           
            Document doc = node.Document as Document;
            DocumentBuilder builder = new DocumentBuilder(doc);
            //将光标移动到指定节点      
            builder.MoveTo(node);
            var text = node.GetText().Trim('{', '}', ' ');

            var item = formItems.SingleOrDefault(c => c["Name"] == text);

            if (text.StartsWith("S:"))   //子表开始
            {
                builder.InsertField(@"MERGEFIELD " + "TableStart:" + text.Split(':')[1] + @" \* MERGEFORMAT");
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[2] + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("E:"))//子表结束
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[2] + @" \* MERGEFORMAT");
                builder.InsertField(@"MERGEFIELD " + "TableEnd:" + text.Split(':')[1] + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("F:")) //子表字段
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[1] + @" \* MERGEFORMAT");

            }
            else if (text.StartsWith("TableStart:") || text.StartsWith("TableEnd:")) //子表开头和结束
            {
                builder.InsertField(@"MERGEFIELD " + text + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("Field:"))//子表字段
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[1] + @" \* MERGEFORMAT");
            }
            else if (item != null) //表单字段
            {
                builder.InsertField(@"MERGEFIELD " + item["Code"] + @" \* MERGEFORMAT");
            }
            else
            {
                builder.InsertField(@"MERGEFIELD " + text + @" \* MERGEFORMAT");
            }

            return ReplaceAction.Replace;
        }
    }
}
