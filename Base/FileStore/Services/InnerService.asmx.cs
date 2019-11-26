using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using FileStore.Logic.BusinessFacade;

namespace FileStore.Services
{
    /// <summary>
    /// InnerService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class InnerService : System.Web.Services.WebService
    {      
        
        [WebMethod]
        public byte[] GetFileBytes(string fileFullPath, int pos, int length)
        {
            byte[] bytes = FileHelper.GetFileBytes(fileFullPath, pos, length);
            return bytes;
        }

        [WebMethod]
        public void DeletePhysicalFile(string fileFullPath)
        {
            if (FileHelper.FileExist(fileFullPath))
                FileHelper.DelFile(fileFullPath);
        }

        [WebMethod]
        public bool ExistFile(string fileFullPath)
        {
            if (FileHelper.FileExist(fileFullPath))
                return true;
            else
                return false;
        }

    }
}
