using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using FileStore.Logic.BusinessFacade;
using FileStore.Logic.Domain;

namespace FileStore.SlUpload
{
    /// <summary>
    /// FileUploadHandler 的摘要说明
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {
        private HttpContext ctx;
        //private UserStateClient userState=null;	// 用户状态信息
        private UploadFileInfo fileInfo = null;	// 文件信息
        private UploadConfig uploadConfig = null;	// 配置信息
        private bool isLog = false;	// 是否写日志

        string uploadPath = "";
        public void ProcessRequest(HttpContext context)
        {
            this.Init(context);

            if (uploadPath == "")
            {
                long fileLength = string.IsNullOrEmpty(ctx.Request.QueryString["FileLength"]) ? 0 : long.Parse(ctx.Request.QueryString["FileLength"]);
                string src = ctx.Request.QueryString["src"];
                string filename = ctx.Request.QueryString["filename"];
                uploadPath = GetTempFolder(filename, src, fileLength);
            }

            FileUploadProcess fileUpload = new FileUploadProcess();

            fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
            fileUpload.ProcessRequest(context, uploadPath, fileInfo, uploadConfig);
        }

        // 文件上传完毕时操作
        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
            string fileName = "";

            OuterServiceFO fsFO = new OuterServiceFO();
            try
            {
                FileInfo fi = new FileInfo(args.FilePath);
                string completedFileName = UploadFileInfo.GetAppCompletedFileName(fileInfo.FileName, DateTime.Now);
                string targetFile = Path.Combine(fi.Directory.FullName, completedFileName);


                string relateId = HttpContext.Current.Request.QueryString["RelateId"];
                string src = HttpContext.Current.Request.QueryString["Src"];

                long fileSize = fi.Length;

                fileName = fsFO.AddLocalFile(fi.FullName, completedFileName, relateId, "", "", src, true);

                ctx.Response.ContentEncoding = Encoding.Unicode;
                ctx.Response.Write(fileName + "_" + fileSize.ToString());
                ctx.Response.Flush();

            }
            catch (Exception exp)
            {
                if (fileName != "")
                {


                    fsFO.DelFiles(fileName, string.Format("Upload页面错误,{0}", exp.Message).Substring(0, 200));
                    FileInfo file = new FileInfo(args.FilePath);
                    file.Delete();
                }

                ctx.Response.ContentEncoding = Encoding.Unicode;
                ctx.Response.Write("error:" + exp.Message);
                ctx.Response.Flush();
            }


            this.WriteLog("Upload End");
        }

        string GetTempFolder(string filename, string src, long fileLength)
        {
            string cacheKey = string.Format("{0}_{1}_{2}", filename, src, fileLength);
            cacheKey = cacheKey.GetHashCode().ToString();
            object obj = HttpContext.Current.Cache.Get(cacheKey);
            if (obj == null)
            {

                OuterServiceFO fsFO = new OuterServiceFO();
                string rootPath = fsFO.GetAvailableRootFolder(src, Path.GetExtension(filename).Trim('.'));
                string tempFolder = Path.Combine(rootPath, "TempFolder");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                HttpContext.Current.Cache.Insert(cacheKey, tempFolder);

                return tempFolder;
            }
            return obj.ToString();
        }
        // 初始化信息
        void Init(HttpContext context)
        {
            this.ctx = context;
            isLog = string.IsNullOrEmpty(ctx.Request.QueryString["IsLog"]) ? false : bool.Parse(ctx.Request.QueryString["IsLog"]);

            long fileLength = string.IsNullOrEmpty(ctx.Request.QueryString["FileLength"]) ? 0 : long.Parse(ctx.Request.QueryString["FileLength"]);
            string src = ctx.Request.QueryString["src"];
            string filename = ctx.Request.QueryString["filename"];




            // 客户端最后写入时间，考虑到效率，据此简单判断客户端文件是否改变，断点续传时使用(如用HashCode对大文件影响效率)
            //string clientLastWriteFileTime = Func.AttrIsNull(ctx.Request.QueryString["LastWriteFileTime"]) ? String.Empty : ctx.Request.QueryString["LastWriteFileTime"];

            // 获取文件信息状态
            //fileInfo = new UploadFileInfo(userState.UserId, filename, clientLastWriteFileTime, fileSize);
            uploadConfig = UploadHelper.RetrieveConfig();
            fileInfo = new UploadFileInfo("USERID", filename, fileLength);

            // 开始上传
            long startByte = string.IsNullOrEmpty(ctx.Request.QueryString["StartByte"]) ? 0 : long.Parse(ctx.Request.QueryString["StartByte"]);
            // 并不是获取上传字节
            bool getBytes = string.IsNullOrEmpty(context.Request.QueryString["GetBytes"]) ? false : bool.Parse(context.Request.QueryString["GetBytes"]);

            if (startByte == 0 && !getBytes)
            {
                this.WriteLog("Upload Begin");
            }

        }

        // 写日志
        void WriteLog(string actionKey)
        {
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}