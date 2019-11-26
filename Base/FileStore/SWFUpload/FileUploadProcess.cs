using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using FileStore.SlUpload;

namespace FileStore.SWFUpload
{
    public class FileUploadProcess
    {
        /// <summary>
        /// FileUploadProcess 文件上传处理类 - coordinated by rey - 2009.5.20
        /// </summary>
        public event FileUploadCompletedEvent FileUploadCompleted;

        public void ProcessRequest(HttpContext context, string uploadPath, UploadFileInfo fileInfo, UploadConfig uploadConfig)
        {

            // 是否已上传完毕
            bool complete = string.IsNullOrEmpty(context.Request.Form["Complete"]) ? true : bool.Parse(context.Request.Form["Complete"]);
            // 获取上传进度操作标识
            bool getBytes = string.IsNullOrEmpty(context.Request.Form["GetBytes"]) ? false : bool.Parse(context.Request.Form["GetBytes"]);
            // 上传开始位置，用于断点续传
            long startByte = string.IsNullOrEmpty(context.Request.Form["StartByte"]) ? 0 : long.Parse(context.Request.Form["StartByte"]);

            // 服务器端文件路径
            string filePath = Path.Combine(uploadPath, fileInfo.FileString);
            FileHelper.CheckFileFullPath(filePath);

            if (getBytes)
            { 
                FileInfo fi = new FileInfo(HttpContext.Current.Server.UrlDecode(filePath));

                if (!fi.Exists)
                    context.Response.Write("0");
                else
                    context.Response.Write(fi.Length.ToString());

                context.Response.Flush();
                return;
            }
            else
            {
                if (startByte > 0 && File.Exists(filePath))
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Append))
                    {
                        SaveFile(context.Request.Files["Filedata"].InputStream, fs);
                        fs.Close();
                    }
                }
                else
                {
                    using (FileStream fs = File.Create(filePath))
                    {
                        SaveFile(context.Request.Files["Filedata"].InputStream, fs);
                        fs.Close();
                    }
                }
                if (complete)
                {
                    if (FileUploadCompleted != null)
                    {
                        FileUploadCompletedEventArgs args = new FileUploadCompletedEventArgs(fileInfo.FileName, filePath);
                        FileUploadCompleted(this, args);
                    }
                }
            }
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }
    }

    // 文件上传完成事件
    public delegate void FileUploadCompletedEvent(object sender, FileUploadCompletedEventArgs args);

    // 文件上传完成事件参数
    public class FileUploadCompletedEventArgs
    {
        private string _fileName = String.Empty;
        public string FileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }

        private string _filePath = String.Empty;
        public string FilePath
        {
            get { return this._filePath; }
            set { this._filePath = value; }
        }

        public FileUploadCompletedEventArgs() { }

        public FileUploadCompletedEventArgs(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }

}