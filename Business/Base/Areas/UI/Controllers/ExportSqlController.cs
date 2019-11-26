using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using System.IO;

namespace Base.Areas.UI.Controllers
{
    public class ExportSqlController : Controller
    {
        public FileResult SqlFile(string defID, string fileCode, string tableName)
        {
            string sql = string.Format("select * from {1} where ID='{0}'", defID, tableName);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            string result = string.Format("delete from {1} where ID='{0}' \n", defID, tableName);
            result += SQLHelper.CreateInsertSql(tableName, dt);
            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(result));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", fileCode + ".sql");
        }
    }
}
