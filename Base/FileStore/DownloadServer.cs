using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileStore.Logic.BusinessFacade;
using System.IO;
using FileStore.Logic.Domain;

namespace FileStore
{
    public class DownloadServer
    {
        OuterServiceFO fsFO = new OuterServiceFO();       

        public string GetResultFileName(string fileIds)
        {

            string fileName = fsFO.GetFileName(fileIds.Split(',').First());

            if (fileIds.Contains(','))
            {
                if (Path.HasExtension(fileName))
                    fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName += ".zip";
            }

            string realFileName = fileName.IndexOf('_') > 0 ? fileName.Remove(0, fileName.IndexOf('_') + 1) : fileName;

            var explorerName = HttpContext.Current.Request.Browser.Browser.ToUpper();
            if (explorerName == "IE" || explorerName == "INTERNETEXPLORER" || HttpContext.Current.Request.UserAgent.ToString().IndexOf("rv:11") > 0)
            {
                realFileName = HttpUtility.UrlEncode(realFileName, System.Text.Encoding.UTF8); //HttpContext.Current.Server.UrlEncode(realFileName);
                realFileName = realFileName.Replace("+", "%20");
            }

            return realFileName;
        }

        public void ValidateFileSize(string fileIds)
        {
            if (!fileIds.Contains(','))
                return;


            //验证文件是否过大
            int totalSize = 0;
            foreach (string id in fileIds.Split(','))
            {
                totalSize += (int)fsFO.GetFileSize(id);
            }
            if (totalSize > 1024 * 1024 * 300)
            {
                HttpContext.Current.Server.Transfer("FileBigErr.htm");
                return;
            }
        }

        public void ExportFile(string fileIds)
        {
            //单文件不压缩,直接输出
            if (!fileIds.Contains(','))
            {
                int pos = 0;
                int length = 1024 * 1024;

                byte[] bytes = fsFO.GetFileBytes(fileIds, pos, length);
                while (bytes != null && bytes.Length > 0 && HttpContext.Current.Response.IsClientConnected)
                {
                    HttpContext.Current.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    HttpContext.Current.Response.Flush();
                    pos += length;
                    bytes = fsFO.GetFileBytes(fileIds, pos, length);
                }


                return;
            }




            ZipHelper zipHelper = new ZipHelper();

            foreach (string id in fileIds.Split(','))
            {      
                string fileName = fsFO.GetFileName(id);

                fileName = fileName.Remove(0, fileName.IndexOf('_') + 1);

                long fileSize = fsFO.GetFileSize(id);
                zipHelper.AddFile(fileName, fileSize);

                int pos = 0;
                int length = 1024 * 1024;
                byte[] bytes = fsFO.GetFileBytes(id, pos, length);
                while (bytes != null && bytes.Length > 0 && HttpContext.Current.Response.IsClientConnected)
                {
                    zipHelper.AppentFileBytes(bytes, bytes.Length);
                    pos += length;
                    bytes = fsFO.GetFileBytes(id, pos, length);
                }

            }

            zipHelper.Complete();

        }


    }

}