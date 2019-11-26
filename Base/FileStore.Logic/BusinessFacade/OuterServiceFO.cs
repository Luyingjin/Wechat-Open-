using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileStore.Logic.Domain;
using System.IO;

namespace FileStore.Logic.BusinessFacade
{
    public class OuterServiceFO
    {

        #region MasterService

        private MasterService.MasterService _masterService = null;
        private MasterService.MasterService masterService
        {
            get
            {
                if (_masterService == null)
                {
                    _masterService = new MasterService.MasterService();
                    _masterService.Url = FileStoreConfig.MasterServerUrl;
                }
                return _masterService;
            }
        }

        #endregion

        #region 检测根目录

        /// <summary>
        /// 检测本服务器的跟目录
        /// </summary>
        public void CheckLocalServerRootFolders()
        {
            var query = masterService.GetAllRootFolderInfo(FileStoreConfig.FileServerName);

            foreach (var root in query)
            {
                if (Directory.Exists(root.RootFolderPath))
                    continue;
                WNetHelper.WNetAddConnection(root.UserName, root.Pwd, root.RootFolderPath);
            }
        }

        #endregion

        #region LocalizeFile

        /// <summary>
        /// 本地化文件，仅供主服务器调用
        /// </summary>
        /// <param name="fileName"></param>
        public MasterService.FsFileInfo LocalizeFile(string fileName)
        {
            MasterService.FsFileInfo fileInfo = masterService.GetFileInfo(fileName, FileStoreConfig.FileServerName);

            string localFilePath = GetLocalFilePath(fileInfo.FileFullPathList);

            if (localFilePath != "")
            {
                fileInfo.FileFullPath = localFilePath;
                return fileInfo;
            }

            FileService.InnerService innerService = new FileService.InnerService();
            innerService.Url = fileInfo.SrcFileServiceUrl;

            int pos = 0;
            int length = 1024 * 1024;
            byte[] bytes = innerService.GetFileBytes(fileInfo.SrcFileFullPath, pos, length);
            while (bytes.Length > 0)
            {
                FileHelper.SaveFile(fileInfo.FileFullPath, bytes, fileInfo.FileSize, pos);
                pos += length;
                bytes = innerService.GetFileBytes(fileInfo.SrcFileFullPath, pos, length);
            }

            masterService.UpdateFileServerNames(fileName, FileStoreConfig.FileServerName);

            return fileInfo;
        }

        #endregion 

        #region CopyFile

        public string CopyFile(string fileName, string relateId, string fileCode, string version, string src)
        {
            var fileInfo = LocalizeFile(fileName);

            string destFileName = fileName.Substring(fileName.IndexOf('_') + 1);

            var newFileInfo = masterService.AddFile(FileStoreConfig.FileServerName, destFileName, fileInfo.FileSize, relateId, fileCode, version, src);

            FileHelper.CopyFile(fileInfo.FileFullPath, newFileInfo.FileFullPath);

            return newFileInfo.FileName;
        }

        #endregion

        #region OverwriteFile

        public void OverwriteFile(string fileName, string relateId, string fileCode, string version, byte[] bytes, long fileSize, string src)
        {
            masterService.DeletePhysicalFile(fileName);
            masterService.UpdateFile(FileStoreConfig.FileServerName, fileName, fileSize, relateId, fileCode, version, src);

            var fileInfo = masterService.GetFileInfo(fileName, FileStoreConfig.FileServerName);
            FileHelper.SaveFile(fileInfo.FileFullPath, bytes, fileInfo.FileSize, 0);
        }

        #endregion

        #region DelFileByRelateId

        public void DelFileByRelateId(string relateId, string delReason)
        {
            masterService.DelFileByRelateId(relateId, delReason);             
        }

        #endregion

        #region AddLocalFile

        public string AddLocalFile(string localFileFullPath, string destFileName, string relateId, string code, string version, string src, bool moveFile = false)
        {
            long fileSize = FileHelper.GetFileSize(localFileFullPath);

            var fileInfo = masterService.AddFile(FileStoreConfig.FileServerName, destFileName, fileSize, relateId, code, version, src);

            if (moveFile)
                FileHelper.MoveFile(localFileFullPath, fileInfo.FileFullPath);
            else
                FileHelper.CopyFile(localFileFullPath, fileInfo.FileFullPath);

            return fileInfo.FileName;
        }

        #endregion

        #region SaveFile

        public string SaveFile(string fileName, string relateId, string code, string version, byte[] bytes, long fileSize, string src)
        {
            var fileInfo = masterService.AddFile(FileStoreConfig.FileServerName, fileName, fileSize, relateId, code, version, src);

            FileHelper.SaveFile(fileInfo.FileFullPath, bytes, fileSize, 0);

            return fileInfo.FileName;
        }

        #endregion

        #region AppendFile

        public string AppendFile(string fileId, byte[] bytes)
        {
            var fileInfo = masterService.GetFileInfo(fileId, FileStoreConfig.FileServerName);

            FileHelper.AppendFile(fileInfo.FileFullPath, bytes, fileInfo.FileSize);

            return fileInfo.FileName;
        }

        #endregion

        #region GetFileSize

        public long GetFileSize(string fileName)
        {
            return masterService.GetFileSize(fileName);
        }

        #endregion

        #region GetFileName

        public string GetFileName(string fileID)
        {
            return masterService.GetFileName(fileID);
        }

        #endregion

        #region GetFileNamesByRelateId

        public string GetFileNamesByRelateId(string relateId)
        {
            return masterService.GetFileNamesByRelateId(relateId);
        }

        #endregion

        #region GetFileBytes

        public byte[] GetFileBytes(string fileName, int pos, int length)
        {
            var fileInfo = LocalizeFile(fileName);
            return FileHelper.GetFileBytes(fileInfo.FileFullPath, pos, length);
        }

        #endregion

        #region DelFiles

        public void DelFiles(string fileName, string delReasion)
        {
            masterService.DelFile(fileName, delReasion);
        }

        #endregion

        #region GetAvailableRootFolder

        public string GetAvailableRootFolder(string src, string extName)
        {
            return masterService.GetAvailableRootFolder(FileStoreConfig.FileServerName, src, extName);
        }

        #endregion

        #region 私有方法

        public string GetLocalFilePath(string[] fileFullPaths)
        {
            foreach (string path in fileFullPaths)
            {              
                if (FileHelper.FileExist(path))
                    return path;
            }
            return "";
        }

        #endregion
    }
}
