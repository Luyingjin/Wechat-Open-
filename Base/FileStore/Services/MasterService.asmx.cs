using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using FileStore.Logic.BusinessFacade;
using FileStore.Logic.Domain;

namespace FileStore.Services
{
    /// <summary>
    /// MasterService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class MasterService : System.Web.Services.WebService
    {
        MasterServiceFO masterFO = new MasterServiceFO();

        [WebMethod]
        public FsFileInfo GetFileInfo(string fileName, string serverName)
        {
            return masterFO.GetFileInfo(fileName, serverName);
        }

        [WebMethod]
        public FsFileInfo AddFile(string serverName, string destfileName, long fileSize, string relateId, string fileCode, string version, string src)
        {
            return masterFO.AddFile(serverName, destfileName, fileSize, relateId, fileCode, version, src);
        }

        [WebMethod]
        public FsFileInfo UpdateFile(string serverName, string destfileName, long fileSize, string relateId, string fileCode, string version, string src)
        {
            return masterFO.UpdateFile(serverName, destfileName, fileSize, relateId, fileCode, version, src);
        }

        [WebMethod]
        public void SetFileStatus(string fileName, string status)
        {
            masterFO.SetFileStatus(fileName, status);
        }

        [WebMethod]
        public void UpdateFileServerNames(string fileName, string serverName)
        {
            masterFO.UpdateFileServerNames(fileName, serverName);
        }

        [WebMethod]
        public void DelFile(string fileName, string delReason)
        {
            masterFO.DelFile(fileName, delReason);
        }

        [WebMethod]
        public void DelFileByRelateId(string relateId, string delReason)
        {
            masterFO.DelFileByRelateId(relateId, delReason);           
        }
        [WebMethod]
        public void DeletePhysicalFile(string fileName)
        {
            masterFO.DeletePhysicalFile(fileName);
        }

        [WebMethod]
        public string GetFileNamesByRelateId(string relateId)
        {
            return masterFO.GetFileNamesByRelateId(relateId);
        }

        [WebMethod]
        public void SetFileBaseAttr(string fileId, string relateId, string fileCode, string version)
        {
            masterFO.SetFileBaseAttr(fileId, relateId, fileCode, version);
        }

        [WebMethod]
        public int GetFileSize(string fileId)
        {
            return (int)masterFO.GetFileSize(fileId);
        }
        [WebMethod]
        public string GetFileName(string fileID)
        {
            return masterFO.GetFileName(fileID);
        }
        [WebMethod]
        public string GetAvailableRootFolder(string serverName, string src, string extName)
        {
            return masterFO.GetAvailableRootFolder(serverName, src, extName).RootFolderPath;
        }
        [WebMethod]
        public RootFolderInfo[] GetAllRootFolderInfo(string serverName)
        {
            return masterFO.GetAllRootFolderInfo(serverName);
        }
    }

}
