using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileStore.Logic.Domain;
using System.IO;

namespace FileStore.Logic.BusinessFacade
{
    public class MasterServiceFO
    {
        FileStoreEntities entities = new FileStoreEntities("FileStore");

        LogFO logFO = new LogFO();

        #region 检测根目录

        /// <summary>
        /// 检测本服务器的跟目录
        /// </summary>
        public void CheckLocalServerRootFolders()
        {
            var query = entities.FsRootFolder.Where(c => c.FsServer.ServerName == FileStoreConfig.FileServerName);

            foreach (var root in query)
            {
                if (Directory.Exists(root.RootFolderPath))
                    continue;
                WNetHelper.WNetAddConnection(root.UserName, root.Pwd, root.RootFolderPath);
            }
        }

        #endregion

        #region GetFileInfo

        public FsFileInfo GetFileInfo(string fileName, string serverName)
        {
            int fileID = FileHelper.GetFileId(fileName);
            var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();



            string srcFileFullPath = GetFileFullPath(fileName);

            var rootFolders = entities.FsRootFolder.Where(c => c.FsServer.ServerName == serverName).ToArray();
            string[] fileFullPathList = new string[rootFolders.Count()];
            for (int i = 0; i < rootFolders.Length; i++)
            {
                fileFullPathList[i] = FileHelper.GetFileFullPath(rootFolders[i].RootFolderPath, file.Name, file.Guid, rootFolders[i].AllowEncrypt == "1");
            }


            var fsRoot = GetAvailableRootFolder(serverName, file.Src, file.ExtName);
            string fileFullPath = FileHelper.GetFileFullPath(fsRoot.RootFolderPath, file.Name, file.Guid, fsRoot.AllowEncrypt == "1");

            return new FsFileInfo()
            {
                FileName = file.Name,
                FileSize = (long)file.FileSize,
                FileFullPath = fileFullPath,
                FileFullPathList = fileFullPathList,
                SrcFileFullPath = srcFileFullPath.Substring(srcFileFullPath.IndexOf('@') + 1),
                SrcFileServiceUrl = srcFileFullPath.Substring(0, srcFileFullPath.IndexOf('@'))
            };
        }

        #endregion

        #region AddFile

        public FsFileInfo AddFile(string serverName, string destfileName, long fileSize, string relateId, string fileCode, string version, string src)
        {
            //文件名中不能包含半角逗号
            destfileName = destfileName.Replace(',', '，');

            FsFile file = new FsFile();
            file.Name = "";
            file.ExtName = "";
            if (destfileName.Contains('.'))
                file.ExtName = destfileName.Split('.').Last();
            file.IsDeleted = "0";
            file.FileSize = fileSize;
            file.RelateId = relateId;
            file.Code = fileCode;
            file.Version = version;
            file.Src = src;
            file.CreateTime = DateTime.Now;
            file.Guid = Guid.NewGuid().ToString();

            var fileServer = entities.FsServer.Where(c => c.ServerName == serverName).SingleOrDefault();
            file.UploadServerName = fileServer.ServerName;
            file.FileServerNames = fileServer.ServerName;
            file.OnMaster = fileServer.IsMaster;
            file.Status = "";


            if (Config.Constant.IsOracleDb)
            {
                var sqlHelper = Config.SQLHelper.CreateSqlHelper("FileStore");

                var idvalue = sqlHelper.ExecuteScalar("select FSFILE_SEQUENCE.nextval from dual").ToString();
                file.Name = idvalue + "_" + destfileName;
                var sql = "INSERT INTO FSFILE( ID ,\"NAME\",EXTNAME,FILESIZE,RELATEID,CODE,VERSION,CREATETIME,ISDELETED,DELETETIME,DELETEREASON,SRC,ONMASTER,UPLOADSERVERNAME,FILESERVERNAMES,GUID,STATUS,ERRORCOUNT,ERRORTIME) VALUES ({18},'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy/MM/dd hh24:mi:ss'),'{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')";
                sql = string.Format(sql, file.Name, file.ExtName, file.FileSize, file.RelateId, file.Code, file.Version,
                    file.CreateTime, file.IsDeleted, file.DeleteTime, file.DeleteReason, file.Src, file.OnMaster,
                    file.UploadServerName, file.FileServerNames, file.Guid, file.Status, file.ErrorCount, file.ErrorTime,idvalue.ToString());

                sqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                entities.FsFile.Add(file);
                entities.SaveChanges();
                file.Name = string.Format("{0}_{1}", file.Id, destfileName);
            }
            entities.SaveChanges();

            var fsRoot = GetAvailableRootFolder(serverName, src, file.ExtName);
            string fileFullPath = FileHelper.GetFileFullPath(fsRoot.RootFolderPath, file.Name, file.Guid, fsRoot.AllowEncrypt == "1");


            //记录日志
            logFO.AddLog(file.Name, "AddFile", serverName);

            return new FsFileInfo()
            {
                FileName = file.Name,
                FileSize = (long)file.FileSize,
                FileFullPath = fileFullPath,
                SrcFileFullPath = "",
                SrcFileServiceUrl = ""
            };
        }

        #endregion

        #region UpdateFile

        public FsFileInfo UpdateFile(string serverName, string destfileName, long fileSize, string relateId, string fileCode, string version, string src)
        {
            //记录日志
            logFO.AddLog(destfileName, "UpdateFile", serverName);

            int fileID = FileHelper.GetFileId(destfileName);

            FsFile file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();
            file.IsDeleted = "0";
            file.FileSize = fileSize;
            file.RelateId = relateId;
            file.Code = fileCode;
            file.Version = version;
            file.Src = src;


            var fileServer = entities.FsServer.Where(c => c.ServerName == serverName).SingleOrDefault();
            file.UploadServerName = fileServer.ServerName;
            file.FileServerNames = fileServer.ServerName;
            file.OnMaster = fileServer.IsMaster;
            file.Status = "";


            entities.SaveChanges();
            var fsRoot = GetAvailableRootFolder(serverName, src, file.ExtName);

            return new FsFileInfo()
            {
                FileName = file.Name,
                FileSize = (long)file.FileSize,
                FileFullPath = FileHelper.GetFileFullPath(fsRoot.RootFolderPath, file.Name, file.Guid, fsRoot.AllowEncrypt == "1"),
                SrcFileFullPath = "",
                SrcFileServiceUrl = ""
            };
        }

        #endregion

        #region SetFileStatus

        public void SetFileStatus(string fileName, string status)
        {
            int fileID = FileHelper.GetFileId(fileName);
            var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();
            file.Status = status;
            entities.SaveChanges();
        }

        #endregion

        #region UpdateFileServerNames

        public void UpdateFileServerNames(string fileName, string serverName)
        {
            int fileID = FileHelper.GetFileId(fileName);
            var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();

            file.FileServerNames += "," + serverName;
            file.FileServerNames = string.Join(",", file.FileServerNames.Split(',').Distinct());
            entities.SaveChanges();
        }

        #endregion

        #region DelFile

        public void DelFile(string fileNames, string delReason)
        {
            foreach (string fileName in fileNames.Split(','))
            {
                //记录日志
                logFO.AddLog(fileName, "DelFile", "");

                int fileID = FileHelper.GetFileId(fileName);
                var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();
                file.IsDeleted = "1";
                file.DeleteTime = DateTime.Now;
                file.DeleteReason = delReason;
            }
            entities.SaveChanges();
        }

        #endregion

        #region DelFileByRelateId

        public void DelFileByRelateId(string relateId, string delReason)
        {
            var files = entities.FsFile.Where(c => c.RelateId == relateId);
            foreach (var item in files)
            {
                //记录日志
                logFO.AddLog(item.Name, "DelFileByRelateId", "");

                item.IsDeleted = "1";
                item.DeleteTime = DateTime.Now;
                item.DeleteReason = delReason;
            }
            entities.SaveChanges();
        }

        #endregion

        #region DeletePhysicalFile

        public void DeletePhysicalFile(string fileName)
        {
            int fileID = FileHelper.GetFileId(fileName);
            var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();

            FileService.InnerService innerService = new FileService.InnerService();
            foreach (var item in entities.FsServer.ToList())
            {
                //记录日志
                logFO.AddLog(fileName, "DeletePhysicalFile", "");

                innerService.Url = item.HttpUrl;

                var rootFolder = GetAvailableRootFolder(item.ServerName, file.Src, file.ExtName);



                innerService.DeletePhysicalFile(FileHelper.GetFileFullPath(rootFolder.RootFolderPath, file.Name, file.Guid, rootFolder.AllowEncrypt == "1"));
            }

        }

        #endregion

        #region GetFileNamesByRelateId

        public string GetFileNamesByRelateId(string relateId)
        {
            var query = entities.FsFile.Where(c => c.RelateId == relateId);
            StringBuilder sb = new StringBuilder();

            foreach (FsFile file in query)
            {
                sb.AppendFormat("{0}_{1},", file.Name, file.FileSize);
            }

            return sb.ToString().Trim(',');
        }

        #endregion

        #region SetFileBaseAttr

        public void SetFileBaseAttr(string fileName, string relateId, string fileCode, string version)
        {
            int fileID = FileHelper.GetFileId(fileName);

            FsFile file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();
            file.RelateId = relateId;
            file.Code = fileCode;
            file.Version = version;
            entities.SaveChanges();
        }

        #endregion

        #region GetFileSize

        public long GetFileSize(string fileName)
        {
            int fileID = FileHelper.GetFileId(fileName);
            return (long)entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault().FileSize;
        }

        #endregion

        #region GetFileName

        public string GetFileName(string fileID)
        {
            int id = FileHelper.GetFileId(fileID);
            return entities.FsFile.Where(c => c.Id == id).SingleOrDefault().Name;
        }

        #endregion

        #region GetAvailableRootFolder

        /// <summary>
        /// 获取本服务器可以使用的第一个跟目录
        /// </summary>
        /// <returns>Id,RootFolderPath</returns>
        public FsRootFolder GetAvailableRootFolder(string serverName, string src, string extName)
        {
            extName = extName.Trim('.');
            string errMsg = string.Format("当前文件服务没有可用的根目录，请查看FileStore数据库的FsRootFolder表,Src={0},ExtName={1}", src, extName);

            var query = entities.FsRootFolder.Where(c => c.FsServer.ServerName == serverName && c.IsFull != "1");
            IQueryable<FsRootFolder> result = new List<FsRootFolder>().AsQueryable();


            result = query.Where(c => c.SrcFilter == src && c.ExtNameFilter.Contains(extName));
            if (result.Count() > 0) goto SelectComplete;

            result = query.Where(c => c.SrcFilter == src && (c.ExtNameFilter == null || c.ExtNameFilter == ""));
            if (result.Count() > 0) goto SelectComplete;

            result = query.Where(c => (c.SrcFilter == null || c.SrcFilter == "") && c.ExtNameFilter.Contains(extName));
            if (result.Count() > 0) goto SelectComplete;

            result = query.Where(c => (c.SrcFilter == null || c.SrcFilter == "") && (c.ExtNameFilter == null || c.ExtNameFilter == ""));
            if (result.Count() > 0) goto SelectComplete;

        SelectComplete:
            if (result.Count() == 0)
                throw new Exception(errMsg);
            FsRootFolder rootFolder = result.First();

            return rootFolder;


        }

        /// <summary>
        /// 获取全部服务器可以使用的第一个跟目录
        /// </summary>
        /// <returns>Id,RootFolderPath</returns>
        public FsRootFolder GetAvailableRootFolder(string src)
        {
            string errMsg = string.Format("当前文件服务没有可用的根目录，请查看FileStore数据库的FsRootFolder表,Src={0}", src);

            var query = entities.FsRootFolder.Where(c => c.IsFull != "1");
            IQueryable<FsRootFolder> result = new List<FsRootFolder>().AsQueryable();

            result = query.Where(c => c.SrcFilter == src && (c.ExtNameFilter == null || c.ExtNameFilter == ""));
            if (result.Count() > 0) goto SelectComplete;

            result = query.Where(c => (c.SrcFilter == null || c.SrcFilter == "") && (c.ExtNameFilter == null || c.ExtNameFilter == ""));
            if (result.Count() > 0) goto SelectComplete;

        SelectComplete:
            if (result.Count() == 0)
                throw new Exception(errMsg);
            FsRootFolder rootFolder = result.First();

            return rootFolder;


        }

        #endregion

        #region GetAllRootFolderInfo

        /// <summary>
        /// 获取本服务器可以使用的第一个跟目录
        /// </summary>
        /// <returns>Id,RootFolderPath</returns>
        public RootFolderInfo[] GetAllRootFolderInfo(string serverName)
        {
            return entities.FsRootFolder.Where(c => c.FsServer.ServerName == serverName).Select(c =>
                new RootFolderInfo { RootFolderPath = c.RootFolderPath, UserName = c.UserName, Pwd = c.Pwd }).ToArray();
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 获取文件实际所在服务器的位置
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetFileFullPath(string fileName)
        {
            int fileID = FileHelper.GetFileId(fileName);
            var file = entities.FsFile.Where(c => c.Id == fileID).SingleOrDefault();

            var server = entities.FsServer.Where(c => c.IsMaster == "1").First();


            if (!file.FileServerNames.Split(',').Contains(server.ServerName))
            {
                string serverName = file.FileServerNames.Split(',').First();
                server = entities.FsServer.Where(c => c.ServerName == serverName).SingleOrDefault();
            }


            FileService.InnerService innerService = new FileService.InnerService();
            innerService.Url = server.HttpUrl;

            foreach (var item in entities.FsRootFolder.Where(c => c.ServerId == server.Id).ToArray())
            {
                string fileFullPath = FileHelper.GetFileFullPath(item.RootFolderPath, file.Name, file.Guid, item.AllowEncrypt == "1");
                if (FileHelper.FileExist(fileFullPath))
                {
                    string currentFsServerName = System.Configuration.ConfigurationManager.AppSettings["FS_ServerName"];
                    return entities.FsServer.Where(c => c.ServerName == currentFsServerName).SingleOrDefault().HttpUrl + "@" + fileFullPath;
                }
                else if (innerService.ExistFile(fileFullPath))
                {
                    return server.HttpUrl + "@" + fileFullPath;
                }
            }

            //throw new Exception(string.Format("文件'{0}'在服务器'{1}'上不存在", file.Name, server.ServerName));
            return "@";
        }

        #endregion
    }

    public class FsFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 新增文件时的全路径
        /// </summary>
        public string FileFullPath { get; set; }
        /// <summary>
        /// 同步文件时，文件可能的路径
        /// </summary>
        public string[] FileFullPathList { get; set; }
        /// <summary>
        /// 文件来源服务的文件全路径
        /// </summary>
        public string SrcFileFullPath { get; set; }
        /// <summary>
        /// 文件来源服务器的地址
        /// </summary>
        public string SrcFileServiceUrl { get; set; }
    }

    public class RootFolderInfo
    {
        public string RootFolderPath { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }

    }
}
