using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Config;
using System.Data;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using System.Text;
using Formula.ImportExport;
using Base.Logic.BusinessFacade;

namespace MvcConfig.Areas.UI.Controllers
{
    public class WordController : Controller
    {
        public FileResult Export(string tmplCode, string id)
        {
            if (string.IsNullOrEmpty(tmplCode))
                throw new Exception("缺少参数TmplCode");      

            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dtWordTmpl = sqlHeper.ExecuteDataTable(string.Format("select * from S_UI_Word where Code='{0}'", tmplCode));
            if (dtWordTmpl.Rows.Count==0)
                throw new Exception("Word导出定义不存在");


            #region 预览时的自动ID
            if (Request["TopID"] == "true" && string.IsNullOrEmpty(id))
            {
                SQLHelper sqlHelperWord = SQLHelper.CreateSqlHelper(dtWordTmpl.Rows[0]["ConnName"].ToString());
                string sql = string.Format("select top 1 ID from ({0}) a", dtWordTmpl.Rows[0]["SQL"]);

                var obj = sqlHelperWord.ExecuteScalar(sql);
                if (obj != null)
                    id = obj.ToString();
            }

            #endregion

            if (string.IsNullOrEmpty(id))
                throw new Exception("缺少参数ID");

            string tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + ".docx";

            string tempPath = Server.MapPath("/") + "WordTemplate/" + tmplName;

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            DataSet ds = uiFO.GetWordDataSource(tmplCode, id);

            AsposeWordExporter export = new AsposeWordExporter();
            byte[] result = export.ExportWord(ds, tempPath);
            MemoryStream docStream = new MemoryStream(result);

            string realFileName = dtWordTmpl.Rows[0]["Name"].ToString();
            var explorerName = HttpContext.Request.Browser.Browser.ToUpper();
            if (explorerName == "IE" || explorerName == "INTERNETEXPLORER" || HttpContext.Request.UserAgent.ToString().IndexOf("rv:11") > 0)
            {
                realFileName = HttpUtility.UrlEncode(realFileName, System.Text.Encoding.UTF8); 
                realFileName = realFileName.Replace("+", "%20");
            }

            return base.File(docStream.ToArray(), "application/msword", realFileName + ".doc");

        }


    }
}
