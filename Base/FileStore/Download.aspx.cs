using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FileStore.Logic;
using FileStore.Logic.BusinessFacade;
using Config;

namespace FileStore
{
    public partial class Download : System.Web.UI.Page
    {
        DownloadServer downloadServer = new DownloadServer();

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileIds = GetFileIds();

            fileIds = Server.UrlDecode(fileIds);

            if (string.IsNullOrEmpty(fileIds))
            {
                Response.Write("需要参数FileId");
                return;
            }

            if (System.Configuration.ConfigurationManager.AppSettings["FS_Distributed"] != "True")
            {
                #region 非分布式文件系统，需要跳转到文件所在的服务器下载文件
                string fID = fileIds.Split(',')[0].Split('_')[0];
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("FileStore");
                object src = sqlHelper.ExecuteScalar(string.Format("select Src from FsFile where ID='{0}'", fID));
                if (src == null)
                    return;
                MasterServiceFO masterFO = new MasterServiceFO();
                string url = masterFO.GetAvailableRootFolder(src.ToString()).FsServer.HttpUrl;
                if (string.IsNullOrEmpty(url))
                    url = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
                url = url.Split(new string[] { "Services" }, StringSplitOptions.RemoveEmptyEntries).First();

                Uri u = new Uri(url);
                //if (!Request.Url.OriginalString.StartsWith(url))
                if (Request.Url.Host != u.Host || !Request.Url.LocalPath.StartsWith(u.LocalPath))
                {
                    url = string.Format("{0}Download.aspx", url);
                    url += Request.Url.Query;
                    Response.Redirect(url);
                    return;
                }
                #endregion
            }
            else
            {
                #region 分布式的文件系统，需要跳转到个人使用的文件服务器

                string url = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                {
                    string sql = "select HttpUrl from FsServer join UserFileServer on UserName='{0}' and FsServer.ServerName= UserFileServer.ServerName";
                    sql = string.Format(sql, HttpContext.Current.User.Identity.Name);
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("FileStore");
                    object result = sqlHelper.ExecuteScalar(sql);
                    if (result != null)
                        url = result.ToString();
                }

                url = url.Split(new string[] { "Services" }, StringSplitOptions.RemoveEmptyEntries).First();

                Uri u = new Uri(url);
                //if (!Request.Url.OriginalString.StartsWith(url))

                #region 由于使用了Apache的重定向功能，会跟这段代码冲突，引发无线循环，因此取消这段代码(本系统暂时不能启用分布式文件部署方案) edit by bob.peng 20150304
                //if (Request.Url.Host != u.Host || !Request.Url.LocalPath.StartsWith(u.LocalPath))
                //{
                //    url = string.Format("{0}Download.aspx", url);
                //    url += Request.Url.Query;
                //    Response.Redirect(url);
                //    return;
                //}
                #endregion
                #endregion
            }

            downloadServer.ValidateFileSize(fileIds);

            string realFileName = downloadServer.GetResultFileName(fileIds);

            realFileName.Replace(" ", "%20");


            //long fileSize = bytes.Length;
            Response.ContentType = "application/octet-stream";
            Response.ContentType = "application/octet-stream ; Charset=UTF8";
            Response.CacheControl = "public";
            Response.AddHeader("Content-Disposition", "attachment;filename=\"" + realFileName + "\"");
            //Response.AddHeader("Content-Length", fileSize.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");

            downloadServer.ExportFile(fileIds);

            Response.End();

            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }


        private string GetFileIds()
        {
            if (!string.IsNullOrEmpty(Request["FileID"]))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["FS_FreeDownload"] == "True")
                {
                    return Request["FileID"];
                }
                else
                {
                    Response.Write("不能直接通过输入地址下载文件！");
                    Response.End();
                    return null;
                }

            }
            else if (!string.IsNullOrEmpty(Request["auth"]))
            {
                string fileId = Request["auth"];                
                fileId = System.Text.Encoding.Default.GetString(Convert.FromBase64String(fileId.Replace("%2B", "+")));

                DateTime t = DateTime.Parse(fileId.Split('_').LastOrDefault());
                if (DateTime.Now > t)
                {
                    Response.Write("下载权限已超时，请重新获取下载权限！");
                    Response.End();
                    return null;
                }
                //return fileId.Split('_').FirstOrDefault();
                return fileId.Remove(fileId.LastIndexOf('_'));
            }
            else
            {
                string id = Request["id"];
                //id = id.Split('_').FirstOrDefault();
                id += DateTime.Now.AddMinutes(1).ToString("_yyyy-MM-dd HH:mm:ss");
                string fileID = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(id)).Replace("+", "%2B");
                Response.Write(fileID);
                Response.End();
                return null;
            }           
        }
    }
}