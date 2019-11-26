using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Formula.ImportExport;
using Config.Logic;
using System.Data;
using Formula;
using MvcAdapter.ImportExport;
using System.Reflection;
using System.Web;

namespace MvcAdapter
{
    public class AsposeExcelController : Controller
    {
        // Post  ExportISO 导出ISO表单
        public ActionResult ExportForm(string Id, string tmplKey, string title, string typeName)
        {
            Formula.LogWriter.Info(string.Format("ExportISO - 导出ISO单的Key：{0} - 开始", tmplKey));

            // 获取导出Excel的数据源
            var dic = new Dictionary<string, object>();
            string mapPath = Server.MapPath("..") + "\\bin\\{0}.dll";
            string root = Request.ApplicationPath.Split('/')[1];
            Type t = null;
            if (System.IO.File.Exists(string.Format(mapPath, root + ".Logic")))
            {
                Assembly assLogic = Assembly.LoadFrom(string.Format(mapPath, root + ".Logic"));
                t = assLogic.GetType(typeName);
            }
            if (t == null)
            {
                if (System.IO.File.Exists(string.Format(mapPath, root)))
                {
                    Assembly assWeb = Assembly.LoadFrom(string.Format(mapPath, root));
                    t = assWeb.GetType(typeName);
                }
                if (t == null)
                    throw new Exception("不存在对应的类名(包括命名空间)");
            }
            if (Array.IndexOf(t.GetInterfaces(), typeof(IExporterForm)) == -1)
                throw new Exception(typeName + "没有继承" + typeof(IExporterForm).Name);
            IExporterForm exporterForm = (IExporterForm)t.Assembly.CreateInstance(t.FullName);
            var row = exporterForm.GetMasterData(tmplKey, title, Id);
            foreach (DataColumn c in row.Table.Columns)
            {
                dic.Add(c.ColumnName, row[c.ColumnName]);
            }
            var ds = exporterForm.GetDetailData(tmplKey, title, Id);

            // 查找模板
            byte[] templateBuffer = null;
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, tmplKey) : string.Format("{0}\\{1}.xls", path, tmplKey);
            if (System.IO.File.Exists(templatePath))
            {
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                throw new Exception("没有找到对应的模板，ISO表单名称为：{0}");
            }

            // 导出Excel文件
            var exporter = new AsposeExcelExporter();
            var buffer = exporter.Export(dic, ds, templateBuffer);

            Formula.LogWriter.Info(string.Format("ExportISO - 导出ISO单的Key：{0} - 结束", tmplKey));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
            }

            return Content("导出数据失败，请检查相关配置！");
        }
    }
}
