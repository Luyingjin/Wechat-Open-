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
    /// OuterService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class OuterService : System.Web.Services.WebService
    {
        OuterServiceFO outerServiceFO = new OuterServiceFO();

        [WebMethod]
        public string LocalizeFile(string fileName)
        {
            return outerServiceFO.LocalizeFile(fileName).FileFullPath;
        }

        [WebMethod]
        public string GetFileFullPath(string fileName)
        {
            return outerServiceFO.LocalizeFile(fileName).FileFullPath;
        }

        [WebMethod]
        public string CopyFile(string fileName, string relateId, string fileCode, string version, string src)
        {
            return outerServiceFO.CopyFile(fileName, relateId, fileCode, version, src);
        }

        [WebMethod]
        public bool DelFileByRelateId(string relateId, string delReason)
        {
            outerServiceFO.DelFileByRelateId(relateId, delReason);
            return true;
        }

        [WebMethod]
        public bool OverwriteFile(string fileId, string relateId, string fileCode, string version, byte[] bytes, long fileSize, string src)
        {
            outerServiceFO.OverwriteFile(fileId, relateId, fileCode, version, bytes, fileSize, src);      
            return true;
        }

        [WebMethod]
        public bool DelFiles(string fileIds, string delReason)
        {
            outerServiceFO.DelFiles(fileIds, delReason);
            return true;
        }

        [WebMethod]
        public string GetFileNamesByRelateId(string relateId)
        {
            return outerServiceFO.GetFileNamesByRelateId(relateId);
        }
                
        [WebMethod]
        public string SaveFile(string name, string relateId, string code, string version, byte[] bytes, long fileSize, string src)
        {
            string result = outerServiceFO.SaveFile(name, relateId, code, version, bytes, fileSize, src);
            
            return result;
        }

        [WebMethod]
        public string AppendFile(string fileId, byte[] bytes)
        {
            return outerServiceFO.AppendFile(fileId, bytes);
        }

        [WebMethod]
        public int GetFileSize(string fileId)
        {
            return (int)outerServiceFO.GetFileSize(fileId);
        }

        [WebMethod]
        public byte[] GetFileBytes(string fileId, int pos, int length)
        {
            return outerServiceFO.GetFileBytes(fileId, pos, length);
        }
    }
}
