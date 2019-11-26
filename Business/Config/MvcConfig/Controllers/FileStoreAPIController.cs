using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Config;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Win32;

namespace MvcConfig.Controllers
{
    public class FileStoreAPIController : ApiController
    {
        private static string fsMasterServerUrl = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];

        [HttpGet]
        public HttpResponseMessage _GetFileByRelateID(string id)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.FileStore);
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from FsFile where RelateID = '{0}'", id));
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("关联ID为" + id + "不存在对应文件");

            string fileID = dt.Rows[0]["Id"].ToString();
            string fileName = dt.Rows[0]["Name"].ToString();
            string fileExt = dt.Rows[0]["ExtName"].ToString();
            byte[] bytes = GetService().GetFileBytes(fileID, 0, int.MaxValue);

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(fileExt));
            result.Content.Headers.ContentLength = bytes.Length;

            return result;
        }

        private FileService.OuterService GetService()
        {
            var fileService = new FileService.OuterService();
            fileService.Url = fsMasterServerUrl.Replace("MasterService.asmx", "OuterService.asmx");
            return fileService;
        }

        private string GetContentType(string fileExt)
        {
            string contentType = "application/unknown";
            if (!string.IsNullOrEmpty(fileExt))
            {
                //look in HKCR
                RegistryKey regkey = Registry.ClassesRoot;

                //look for extension
                RegistryKey fileextkey = regkey.OpenSubKey("." + fileExt);

                //retrieve Content Type value
                if (fileextkey != null)
                {
                    contentType = fileextkey.GetValue("Content Type", contentType).ToString();
                }
            }

            return contentType;
        }
    }
}
