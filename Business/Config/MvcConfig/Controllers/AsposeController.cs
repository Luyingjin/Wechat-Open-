using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MvcAdapter.ImportExport;
using System.Data;
using System.Web.Security;
using System.IO;
using Newtonsoft.Json.Converters;
using Formula;
using Formula.ImportExport;
using Aspose.Cells;

namespace MvcConfig.Controllers
{
    public class AsposeController : Controller
    {
        // Post  ExportExcel 导出Excel
        [ValidateInput(false)]
        public ActionResult ExportExcel(string dataUrl, string queryFormData, string sortField, string sortOrder, string jsonColumns, string excelKey, string title = "export")
        {
            LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 开始", excelKey));

            #region 收集自动生成模板的列信息
            var columns = JsonConvert.DeserializeObject<List<ColumnInfo>>(jsonColumns);
            // 清空中文名称为空的列
            if (columns != null)
            {
                for (var i = columns.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(columns[i].ChineseName))
                    {
                        columns.RemoveAt(i);
                    }
                }
                HttpContext.Items["__ColumnInfo"] = columns;
            }
            #endregion

            // 导出到Excel的数据源，
            DataTable dt = null;
            if (dataUrl.IndexOf("[]") >= 0 || dataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                dt = JsonConvert.DeserializeObject<DataTable>(dataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(queryFormData))
                    dic.Add("queryFormData", queryFormData);
                if (!string.IsNullOrWhiteSpace(sortField))
                    dic.Add("sortField", sortField);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    dic.Add("sortOrder", sortOrder);
                dic.Add("pageSize", "0");
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                
                dt = Get<DataTable>(serverUrl, dataUrl, dic) ?? new DataTable();
            }
            dt.TableName = excelKey;

            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = null;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            if (System.IO.File.Exists(templatePath))
            {
                LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, title);
            }

            var buffer = exporter.Export(dt, templateBuffer);

            LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 结束", excelKey));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
            }

            LogWriter.Info(string.Format("ExportExcel - 导出数据失败，参数: dataUrl={0}<br>queryFormData={1}<br>jsonColumns={2}<br>excelKey={3}<br>title={4}<br> ", dataUrl, queryFormData, jsonColumns, excelKey, title));
            return Content("导出数据失败，请检查相关配置！");
        }

        [ValidateInput(false)]
        public ActionResult ExportInlineExcel(string jsonColumns, string excelKey, string title, string masterDataUrl, string masterColumn, string queryFormData, string sortField, string sortOrder, string detailDataUrl, string relateColumn)
        {
            LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 开始", excelKey));

            #region 收集自动生成模板的列信息
            var columns = JsonConvert.DeserializeObject<List<ColumnInfo>>(jsonColumns);
            // 清空中文名称为空的列
            if (columns != null)
            {
                for (var i = columns.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(columns[i].ChineseName))
                    {
                        columns.RemoveAt(i);
                    }
                }
                HttpContext.Items["__ColumnInfo"] = columns;
            }
            #endregion

            // 导出到Excel的数据源，
            DataTable dtMaster = null;
            if (masterDataUrl.IndexOf("[]") >= 0 || masterDataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                dtMaster = JsonConvert.DeserializeObject<DataTable>(masterDataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(queryFormData))
                    dic.Add("queryFormData", queryFormData);
                if (!string.IsNullOrWhiteSpace(sortField))
                    dic.Add("sortField", sortField);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    dic.Add("sortOrder", sortOrder);
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                dtMaster = Get<DataTable>(serverUrl, masterDataUrl, dic) ?? new DataTable();
            }

            DataTable dtDetail = null;
            if (detailDataUrl.IndexOf("[]") >= 0 || detailDataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                dtDetail = JsonConvert.DeserializeObject<DataTable>(detailDataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                string[] arrID = new string[dtMaster.Rows.Count];
                for (int i = 0; i < dtMaster.Rows.Count; i++)
                {
                    arrID[i] = Convert.ToString(dtMaster.Rows[i][masterColumn]);
                }
                string ids = string.Join(",", arrID);
                dic.Add("queryFormData", "{\"$IN$" + relateColumn + "\": \"" + ids + "\"}");
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                dtDetail = Get<DataTable>(serverUrl, detailDataUrl, dic) ?? new DataTable();
            }
            DataTable dt = new DataTable();
            //增加从表列到主表中
            foreach (DataColumn dc in dtMaster.Columns)
            {
                if (!dt.Columns.Contains(dc.ColumnName))
                {
                    dt.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
            foreach (DataColumn dc in dtDetail.Columns)
            {
                if (!dt.Columns.Contains(dc.ColumnName))
                {
                    dt.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
            //合并数据
            foreach (DataRow dr in dtMaster.Rows)
            {
                DataRow[] details = dtDetail.Select(relateColumn + " = '" + Convert.ToString(dr[masterColumn]) + "'");
                if (details.Count() > 0)
                {
                    foreach (DataRow detail in details)
                    {
                        DataRow drNew = dt.NewRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dr.Table.Columns.Contains(dc.ColumnName))
                                drNew[dc.ColumnName] = dr[dc.ColumnName];
                            else if (detail.Table.Columns.Contains(dc.ColumnName))
                                drNew[dc.ColumnName] = detail[dc.ColumnName];
                        }
                        dt.Rows.Add(drNew);
                    }
                }
                else
                {
                    DataRow drNew = dt.NewRow();
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        drNew[dc.ColumnName] = dr[dc.ColumnName];
                    }
                    dt.Rows.Add(drNew);
                }
            }
            

            dt.TableName = excelKey;
            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = null;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            if (System.IO.File.Exists(templatePath))
            {
                LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, title);
            }

            var buffer = exporter.Export(dt, templateBuffer);

            LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 结束", excelKey));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
            }

            LogWriter.Info(string.Format("ExportExcel - 导出数据失败，参数: masterDataUrl={0}<br>queryFormData={1}<br>jsonColumns={2}<br>excelKey={3}<br>title={4}<br> ", masterDataUrl, queryFormData, jsonColumns, excelKey, title));
            return Content("导出数据失败，请检查相关配置！");
        }

        /// <summary>
        /// 获取指定key的Excel模板的文件流
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual byte[] GetExcelTemplateFile(string key)
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.xls", path, key);
            if (!System.IO.File.Exists(tmplPath))
            {
                throw new Exception("找不到指点key的模板文件，key为：" + key);
            }
            var fileBuffer = FileHelper.GetFileBuffer(tmplPath);
            return fileBuffer;
        }

        // Get ImportExcel 导入Excel的界面
        [ValidateInput(false)]
        public ActionResult ImportExcel(string excelkey, string vaildURL, string saveURL, string ErrorFilePath, string ErrorCount, string result, string errorMsg)
        {
            ViewBag.ExcelKey = excelkey;
            ViewBag.VaildURL = vaildURL;
            ViewBag.SaveURL = saveURL;
            ViewBag.ErrorFilePath = ErrorFilePath;
            ViewBag.ErrorCount = ErrorCount;
            ViewBag.ErrorMsg = errorMsg;
            ViewBag.IsSuccess = !string.IsNullOrWhiteSpace(result) ? bool.TrueString : bool.FalseString;
            ViewBag.SuccessCount = result;
            return View();
        }

        // Get DownloadExcelTemplate 下载Excel模板
        public ActionResult DownloadExcelTemplate(string excelkey)
        {
            var importer = new AsposeExcelImporter();
            var buffer = importer.GetExcelTemplate(excelkey);
            return File(buffer, "application/vnd.ms-excel", excelkey + ".xls");
        }

        //文件上传
        public ActionResult UploadExcel(HttpPostedFileBase Fdata, string excelKey, string vaildURL, string saveURL)
        {
            LogWriter.Info(string.Format("UploadExcel - Excel导入的Key：{0} - 开始", excelKey));

            if (Fdata == null || Fdata.InputStream == null)
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = "数据文件没有上传，请上传要导入数据文件！" });

            var fileSize = Fdata.InputStream.Length;
            byte[] fileBuffer = new byte[fileSize];
            Fdata.InputStream.Read(fileBuffer, 0, (int)fileSize);
            Fdata.InputStream.Close();

            ExcelData data = null;
            try
            {
                IImporter importer = new AsposeExcelImporter();
                data = importer.Import(fileBuffer, excelKey);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = ex.Message });
            }
            var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

            // 验证数据
            LogWriter.Info(string.Format("UploadExcel - 进入校验数据逻辑，校验地址：{0} - 开始", vaildURL));
            var errors = Post<List<CellErrorInfo>>(serverUrl, vaildURL, new { data = JsonConvert.SerializeObject(data) });
            LogWriter.Info(string.Format("UploadExcel - 进入校验数据逻辑，校验地址：{0} - 结束", vaildURL));
            if (errors.Count > 0)
            {
                var errorFilePath = string.Format(@"~/ErrorExcels/{0}_有错误.xls", excelKey);
                WriteErrorInfoToExcel(errors, fileBuffer, errorFilePath);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorCount = errors.Count, ErrorFilePath = Url.Content(errorFilePath) });
            }

            // 保存数据
            var dataTable = data.GetDataTable();
            LogWriter.Info(string.Format("UploadExcel - 进入数据保存逻辑，保存地址：{0} - 开始", saveURL));
            var strResult = Post(serverUrl, saveURL, new { data = JsonConvert.SerializeObject(dataTable, new DataTableConverter()) });
            LogWriter.Info(string.Format("UploadExcel - 进入数据保存逻辑，保存地址：{0} - 结束", saveURL));
            if (strResult != "Success")
            {
                LogWriter.Error(strResult);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = "数据无法保存，请联系管理员查看错误日志！" });
            }
            // 清空数据，释放内存
            data = null;

            LogWriter.Info(string.Format("UploadExcel - Excel导入的Key：{0} - 结束", excelKey));
            return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, result = dataTable.Rows.Count.ToString() });
            //return Content(strResult);
        }

        private void WriteErrorInfoToExcel(IList<CellErrorInfo> errors, byte[] fileBuffer, string errorFilePath)
        {
            var workbook = new Workbook(new MemoryStream(fileBuffer));
            foreach (var info in errors)
            {
                var worksheet = workbook.Worksheets[0];

                //Add comment to cell 
                int commentIndex = worksheet.Comments.Add(info.RowIndex, info.ColIndex);

                //Access the newly added comment
                Comment comment = worksheet.Comments[commentIndex];

                //Set the comment note
                comment.Note = info.ErrorText;

                //Set the font of a comment
                comment.Font.Size = 12;
                comment.Font.IsBold = true;
                comment.HeightCM = 5;
                comment.WidthCM = 5;

                //为单元格添加样式    
                var cell = worksheet.Cells[info.RowIndex, info.ColIndex];
                var style = cell.GetStyle();
                //设置背景颜色
                style.ForegroundColor = System.Drawing.Color.Red;
                style.Pattern = BackgroundType.Solid;
                style.Font.IsBold = true;
                style.Font.Color = System.Drawing.Color.White;

                cell.SetStyle(style);
            }

            // 保存错误文件到临时目录
            workbook.Save(Server.MapPath(errorFilePath));
        }

        private void CellValidation(CellValidationArgs e)
        {
            if (e.FieldName == "Code" && e.Value.EndsWith("1"))
            {
                e.IsValid = false;
                e.ErrorText = "内容重复！";
            }
        }

        public class CellValidationArgs
        {
            public CellValidationArgs()
            {
                this.IsValid = true;
            }

            /// <summary>
            /// 行索引值
            /// </summary>
            public int RowIndex { get; set; }

            /// <summary>
            /// 列索引值
            /// </summary>
            public int ColIndex { get; set; }

            /// <summary>
            /// 对应的字段名称
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// 字段值
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// 枚举Key
            /// </summary>
            public string EnumKey { get; set; }

            /// <summary>
            ///  是否验证通过
            /// </summary>
            public bool IsValid { get; set; }

            /// <summary>
            /// 错误提示文本
            /// </summary>
            public string ErrorText { get; set; }

            /// <summary>
            /// 所在数据行
            /// </summary>
            public DataRow Record { get; set; }

        }

        public class CellErrorInfo
        {
            /// <summary>
            /// 行索引值
            /// </summary>
            public int RowIndex { get; set; }

            /// <summary>
            /// 列索引值
            /// </summary>
            public int ColIndex { get; set; }

            /// <summary>
            /// 错误提示文本
            /// </summary>
            public string ErrorText { get; set; }
        }

        private T Get<T>(string serverUrl, string requestUrl, IDictionary<string, object> data = null)
        {
            T result = default(T);

            var restClient = new RestSharp.RestClient(serverUrl);
            
            var request = new RestSharp.RestRequest(requestUrl, RestSharp.Method.POST);
            if (data != null)
            {
                foreach (var key in data.Keys)
                {
                    request.AddParameter(key, data[key]);
                }
            }

            // 追加登录Cookie
            var cookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, true);
            request.AddCookie(FormsAuthentication.FormsCookieName, cookie.Value);
            var response = restClient.Execute(request);

            var json = response.Content;
            if (json.IndexOf("\"total\":0") < 0)
            {
                var dtStartIndex = json.IndexOf("[{");
                var dtEndIndex = json.LastIndexOf("}]");
                var dtJson = json.Substring(dtStartIndex, dtEndIndex - dtStartIndex + 2);
                result = JsonConvert.DeserializeObject<T>(dtJson);
            }
            return result;
        }

        private string Post(string apiBaseUri, string requestUrl, object data = null)
        {
            var restClient = new RestSharp.RestClient(apiBaseUri);
            var request = new RestSharp.RestRequest(requestUrl, RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            //if (data != null)
            //{
            //    foreach (var key in data.Keys)
            //    {
            //        request.AddParameter(key, data[key]);
            //    }
            //}
            request.AddBody(data);

            // 追加登录Cookie
            var cookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, true);
            request.AddCookie(FormsAuthentication.FormsCookieName, cookie.Value);
            request.Timeout = 36000;
            var response = restClient.Execute(request);

            return response.Content;
        }

        private T Post<T>(string serverUrl, string requestUrl, object data = null) where T : class
        {
            var content = Post(serverUrl, requestUrl, data);
            var result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }
    }
}
