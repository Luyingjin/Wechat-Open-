using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.IO;
using System.Web;

namespace Formula.ImportExport
{
    public class DefaultExcelMetadataStorage : IExcelMetadataStorage
    {
        public virtual ExcelMetadata GetMetadataByKey(string key)
        {
            var fileBuffer = GetExcelTemplateFile(key);
            var workbook = new Workbook(new MemoryStream(fileBuffer));
            var property = workbook.BuiltInDocumentProperties;
            var metadata = new ExcelMetadata
            {
                Author = property.Author,
                Company = property.Company,
                Keywords = property.Keywords,
                RevisionNumber = property.RevisionNumber,
                FileBuffer = fileBuffer,
            };
            workbook = null;
            return metadata;
        }

        public virtual bool IsValidExcel(byte[] xlsBuffer, ExcelMetadata metadata)
        {
            var workbook = new Workbook(new MemoryStream(xlsBuffer));
            var property = workbook.BuiltInDocumentProperties;

            return metadata.Keywords == property.Keywords
                && metadata.RevisionNumber == metadata.RevisionNumber
                && metadata.Author == property.Author
                && metadata.Company == property.Company;
        }

        public virtual void AddMetadata(string excelKey, byte[] tmplBuffer, string keywords)
        {
            var workbook = new Workbook(new MemoryStream(tmplBuffer));

            workbook.BuiltInDocumentProperties.Keywords = keywords;
            workbook.BuiltInDocumentProperties.Company = "goodwaysoft";
            workbook.BuiltInDocumentProperties.RevisionNumber = int.Parse(DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString());
            workbook.BuiltInDocumentProperties.Author = "E2Home";

            // 保存到指定目录下
            var excelStream = new MemoryStream();
            workbook.Save(excelStream, SaveFormat.Excel97To2003);
            var buffer = excelStream.ToArray();
            excelStream.Close();
            SaveExcelTemplateFile(buffer, excelKey);

            workbook = null;
        }

        /// <summary>
        /// 保存指定key的Excel模板的文件流
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual void SaveExcelTemplateFile(byte[] tmplBuffer, string key)
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            if (path.StartsWith("/"))
                path = HttpContext.Current.Server.MapPath(path);
            var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.xls", path, key);
            FileHelper.SaveFileBuffer(tmplBuffer, tmplPath);
        }

        /// <summary>
        /// 获取指定key的Excel模板的文件流
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual byte[] GetExcelTemplateFile(string key)
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            if (path.StartsWith("/"))
                path = HttpContext.Current.Server.MapPath(path);
            var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.xls", path, key);
            if (!System.IO.File.Exists(tmplPath))
            {
                throw new Exception("找不到指点key的模板文件，key为：" + key);
            }
            var fileBuffer = FileHelper.GetFileBuffer(tmplPath);
            return fileBuffer;
        }
    }
}
