using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcConfig.Areas.UI.Controllers;

using System.IO;
using Config;
using System.Data;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using System.Text;
using Formula.ImportExport;
using Base.Logic.BusinessFacade;
using System.Net.Http.Headers;
namespace MvcConfig.Controllers
{
    public class FormToWordAPIController : ApiController
    {
        /// <summary>
        /// Form to Word
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Export(string id)
        {
            string tmplCode = Request.RequestUri.ParseQueryString().Get("tmplCode");
            if (string.IsNullOrEmpty(tmplCode))
                throw new Exception("缺少参数TmplCode");

            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dtWordTmpl = sqlHeper.ExecuteDataTable(string.Format("select * from S_UI_Word where Code='{0}'", tmplCode));
            if (dtWordTmpl.Rows.Count == 0)
                throw new Exception("Word导出定义不存在");

            if (string.IsNullOrEmpty(id))
                throw new Exception("缺少参数ID");

            string tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + ".docx";

            var path = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string tempPath = path.Substring(0, path.LastIndexOf('\\') + 1) + "WordTemplate/" + tmplName;// Server.MapPath("/") +

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            DataSet ds = uiFO.GetWordDataSource(tmplCode, id);

            AsposeWordExporter export = new AsposeWordExporter();
            byte[] bytesArray = export.ExportWord(ds, tempPath);
            string fileName = dtWordTmpl.Rows[0]["Name"].ToString();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytesArray);
            result.Content.Headers.ContentLength = bytesArray.Length;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;

            return result;
        }
    }
}
