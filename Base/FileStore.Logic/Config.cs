using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileStore
{
    public class FileStoreConfig
    {
        public static string FileServerName
        {
            get
            {
                if (!System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FS_ServerName"))
                    throw new Exception("请在配置文件中配置文件服务的名称:FS_ServerName");

                return System.Configuration.ConfigurationManager.AppSettings["FS_ServerName"];
            }
        }

        public static string MasterServerUrl
        {
            get
            {
                if (!System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FS_MasterServerUrl"))
                    throw new Exception("请在配置文件中配置文件服务的名称:FS_MasterServerUrl");

                return System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
            }
        }

        public static bool IsMaster
        {
            get
            {
                if (!System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FS_IsMaster"))
                    throw new Exception("请在配置文件中配置文件服务的名称:FS_IsMaster");

                return System.Configuration.ConfigurationManager.AppSettings["FS_IsMaster"].ToLower() == "true";
            }
        }

    }
}