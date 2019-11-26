using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FileStore.Logic;
using FileStore.Logic.BusinessFacade;
using Config;

namespace FileStore.SlUpload
{
    public partial class Upload : System.Web.UI.Page
    {
        // 初始化参数
        protected string InitParams = "";

        // 禁止文件格式
        protected string FilterExts = "";

        private void Page_Load(object sender, System.EventArgs e)
        {
            this.RetrieveInitParams();

            FilterExts = System.Configuration.ConfigurationManager.AppSettings["FS_FileExtFilter"] ?? "";
        }

        #region 私有函数

        private void RetrieveInitParams()
        {

            UploadConfig config = new UploadConfig();

            string fileMode = string.IsNullOrEmpty(Request["FileMode"]) ? "Multi" : Request["FileMode"];

            string bMaximumUpload = Request["MaximumUpload"];
            string maximumUpload = String.Empty;

            if (string.IsNullOrEmpty(bMaximumUpload))
            {
                maximumUpload = ConverterM2B(config.MaxFileSize);
            }
            else
            {
                maximumUpload = ConverterM2B(float.Parse(bMaximumUpload));
            }

            string filter = HttpUtility.UrlDecode(string.IsNullOrEmpty(Request["Filter"]) ? "" : Request["Filter"]);
            if (string.IsNullOrEmpty(filter))
            {
                filter = config.FileFilter;
            }

            string maxNumberToUpload = Request["MaxNumberToUpload"];
            if (string.IsNullOrEmpty(maxNumberToUpload))
            {
                maxNumberToUpload = config.MaxFileNumber.ToString();
            }

            string allowThumbnail = string.IsNullOrEmpty(Request["AllowThumbnail"]) ? "" : Request["AllowThumbnail"].ToLower();
            if (string.IsNullOrEmpty(allowThumbnail))
            {
                allowThumbnail = config.AllowThumbnail.ToString();
            }

            string isLog = string.IsNullOrEmpty(Request["IsLog"]) ? "" : Request["IsLog"].ToLower();
            if (string.IsNullOrEmpty(isLog))
            {
                isLog = config.IsLog.ToString();
            }

            string _initParams = String.Empty;

            //增加RelateId   by dingx_new 2011-7-25
            //string uploadPage = Server.UrlPathEncode(config.HandlerPage);

            string uploadPage = Server.UrlPathEncode(GetUploadHandlerUrl());    //动态上传地址  2013-11 by dingx_new


            if (uploadPage.Contains('?'))
                uploadPage += "&RelateId=" + this.Request.QueryString["RelateId"] + "&Src=" + this.Request.QueryString["Src"];
            else
                uploadPage += "?RelateId=" + this.Request.QueryString["RelateId"] + "&Src=" + this.Request.QueryString["Src"];


            _initParams += //"UploadPage=" + Server.UrlPathEncode(config.HandlerPage + "?PassCode=" + this.PassCode) + ","
                "UploadPage=" + uploadPage + ","
                + "FileMode=" + fileMode + ","
                + "MaximumUpload=" + maximumUpload + ","
                + "Filter=" + filter + ","
                + "MaxNumberToUpload=" + maxNumberToUpload + ","
                + "AllowThumbnail=" + allowThumbnail + ","
                + "IsLog=" + isLog + ","
                + "ContinueSize=" + config.ContinueSize + ","
                + "MaximumTotalUpload=-1,UploadChunkSize=4194304,MaxConcurrentUploads=1,ResizeImage=False,ImageSize=300,"
                + "Multiselect=True,JsCompleteFunction=OnComplete,JsCancelFunction=OnCancel,JsCheckFunction=OnCheck";

            this.InitParams = _initParams;
        }

        // 将比特转换为M比特
        private string ConverterM2B(float val)
        {
            long _mval = (long)(val * 1024 * 1024);
            return _mval.ToString();
        }

        #endregion

        private string GetUploadHandlerUrl()
        {
            string url = "";

            if (System.Configuration.ConfigurationManager.AppSettings["FS_Distributed"] == "True")
            {

                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                {
                    string sql = "select HttpUrl from FsServer join UserFileServer on UserName='{0}' and FsServer.ServerName= UserFileServer.ServerName";
                    sql = string.Format(sql, HttpContext.Current.User.Identity.Name);
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("FileStore");
                    object result = sqlHelper.ExecuteScalar(sql);
                    url = result == null ? "" : result.ToString();
                }
            }
            else
            {
                string src = Request.QueryString["src"];              
                MasterServiceFO masterFO = new MasterServiceFO();
                var rootFolder = masterFO.GetAvailableRootFolder(src);
                url = rootFolder.FsServer.HttpUrl;                          
            }

            if (string.IsNullOrEmpty(url))
                url = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];

            url = url.Split(new string[] { "Services" }, StringSplitOptions.RemoveEmptyEntries).First();

            return string.Format("{0}SlUpload/FileUploadHandler.ashx", url);

        }
    }
}