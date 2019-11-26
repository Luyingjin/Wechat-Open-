using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace FileStore.SlUpload
{
    public sealed class UploadHelper
    {

        //// 获取上传得临时文件夹路径, 也即原Ftp目录路径
        //public static string GetTemparatoryDirectory()
        //{
        //    return  Config.File_TemparatoryDirectory;
        //}


        // 获取上传信息配置s
        public static UploadConfig RetrieveConfig()
        {
            UploadConfig _config = new UploadConfig();

            return _config;
        }
    }

    // 上传文件信息类
    public class UploadFileInfo
    {
        #region 枚举

        // 文件状态
        public enum FileUploadState
        {
            FINISHED,	// 上传完毕
            UNFINISHED,	// 未上传完毕
            UNKNOWN		// 不确定
        }

        #endregion

        #region 属性

        // 文件的上传是否已经完毕
        private FileUploadState _uploadState = FileUploadState.UNKNOWN;
        public FileUploadState UploadState
        {
            get { return this._uploadState; }
        }

        // 文件名
        private string _fileName = String.Empty;
        public string FileName
        {
            get { return this._fileName; }
        }

        // 文件大小
        private long _fileLength = 0;
        public long FileLength
        {
            get { return this._fileLength; }
        }

        // 文件在客户端最后修改时间, 如果为上传完毕文件, 则为DateTime.MinValue
        //private DateTime _lastClientWriteDateTime = DateTime.MinValue;
        //public DateTime LastClientWriteDateTime
        //{
        //	get{ return this._lastClientWriteDateTime; }
        //}

        // 文件上传完毕时间, 默认为DateTime.MinValue
        private DateTime _uploadCompletedDateTime = DateTime.MinValue;
        public DateTime UploadCompletedDateTime
        {
            get { return this._uploadCompletedDateTime; }
        }

        // 文件上传者UserId
        private string _userId = String.Empty;
        public string UserId
        {
            get { return this._userId; }
        }

        // 文件唯一识别符(编码规范(SHA1, SHA256, MD5) + 编码)
        private string _hashCode = String.Empty;
        public string HashCode
        {
            get { return this._hashCode; }
        }

        // 返回文件String
        public string FileString
        {
            get
            {
                string rtnStr = String.Empty;

                switch (this.UploadState)
                {
                    case FileUploadState.FINISHED:
                        rtnStr = UploadFileInfo.GetAppCompletedFileName(this.FileName, this.UploadCompletedDateTime);
                        break;
                    case FileUploadState.UNFINISHED:
                        rtnStr = UploadFileInfo.GetAppTemporaryFileName(this);
                        break;
                    default:
                        rtnStr = this.FileName;
                        break;
                }

                return rtnStr;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构建已上传文件信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="uploadDateTime">上传时间</param>
        public UploadFileInfo(string fileName, DateTime uploadDateTime)
        {
            this._fileName = fileName.Replace(',', '，');
            Init(UploadFileInfo.GetAppCompletedFileName(fileName, uploadDateTime));
        }

        /// <summary>
        /// 构建未上传文件信息
        /// </summary>
        /// <param name="userId">文件userId</param>
        /// <param name="fileName">文件名</param>
        /// <param name="lastClientWriteFileTime">客户端最后修改时间(FileTime)</param>
        ///public UploadFileInfo(string userId, string fileName, string lastClientWriteFileTime)
        public UploadFileInfo(string userId, string fileName, long fileLength)
        {
            this._fileName = fileName.Replace(',', '，');
            //Init(UploadFileInfo.GetAppTemporaryFileName(userId, fileName, lastClientWriteFileTime));
            Init(UploadFileInfo.GetAppTemporaryFileName(userId, fileName, fileLength));
        }

        /// <summary>
        /// 由文件串（通常作为服务器端文件名）得到文件信息
        /// </summary>
        /// <param name="fileString">文件串</param>
        //public UploadFileInfo(string fileString)
        //{
        //    Init(fileString);
        //}

        protected void Init(string fileString)
        {
            if (fileString.IndexOf(UPLOAD_FINISHED_FLAG + "+") == 0)
            {
                try
                {
                    string _fileString = fileString.Substring(UPLOAD_FINISHED_FLAG.Length, (fileString.Length - UPLOAD_FINISHED_FLAG.Length - 1));
                    string[] _fileStringArr = _fileString.Split('+');
                    string _timeString = _fileStringArr[0];

                    this._uploadCompletedDateTime = new DateTime(
                        int.Parse(_timeString.Substring(0, 4)),
                        int.Parse(_timeString.Substring(3, 2)),
                        int.Parse(_timeString.Substring(5, 2)),
                        int.Parse(_timeString.Substring(7, 2)),
                        int.Parse(_timeString.Substring(9, 2)),
                        int.Parse(_timeString.Substring(11, 2)),
                        int.Parse(_timeString.Substring(13, 1))
                        );

                    //this._fileName = _fileStringArr[2];
                    this._uploadState = FileUploadState.FINISHED;
                }
                catch
                {
                    this._uploadState = FileUploadState.UNKNOWN;
                    //this._fileName = fileString;
                }
            }
            else if (fileString.IndexOf(UPLOAD_UNFINISHED_FLAG + "+") == 0)
            {
                try
                {
                    string[] _fileStringArr = fileString.Split('+');
                    this._userId = _fileStringArr[1];
                    //this._lastClientWriteDateTime = DateTime.FromFileTime(long.Parse(_fileStringArr[2]));
                    this._fileLength = long.Parse(_fileStringArr[2]);
                    //this._fileName = _fileStringArr[3];

                    this._uploadState = FileUploadState.UNFINISHED;
                }
                catch
                {
                    this._uploadState = FileUploadState.UNKNOWN;
                    //this._fileName = fileString;
                }
            }
            else
            {
                this._uploadState = FileUploadState.UNKNOWN;
                //this._fileName = fileString;
            }

        }

        #endregion

        #region 静态方法

        // 生成临时文件名(文件已上传完毕)
        public static string GetAppCompletedFileName(string fileName, DateTime uploadDateTime)
        {

            //return FileHelper.GetAppCompletedFileName(fileName, uploadDateTime);


            return fileName;


        }

        // 生成临时文件名(文件未上传完毕, "T" + UserId + 文件名 + 客户端最后修改时间, 若文件在客户端有任何修改则不能实现断点上传功能.)
        public static string GetAppTemporaryFileName(UploadFileInfo uploadFileInfo)
        {
            //return GetAppTemporaryFileName(uploadFileInfo.UserId, uploadFileInfo.FileName, uploadFileInfo.LastClientWriteDateTime.ToFileTime().ToString());
            return GetAppTemporaryFileName(uploadFileInfo.UserId, uploadFileInfo.FileName, uploadFileInfo.FileLength);
        }

        // 生成临时文件名(文件未上传完毕, "T" + UserId + 文件名 + 客户端最后修改时间, 若文件在客户端有任何修改则不能实现断点上传功能.)
        //public static string GetAppTemporaryFileName(string userId, string fileName, string lastClientWriteFileTime)
        public static string GetAppTemporaryFileName(string userId, string fileName, long FileLength)
        {
            //fileName = fileName.Replace(" ", "_").Replace("+", "_");

            string fname = "T+" + userId + "+"
                //+ lastClientWriteFileTime + "+"
                + FileLength + "+"
                + Path.GetFileName(fileName.Replace(" ", "_").Replace("+", "_"));

            return fname;
        }

        #endregion

        #region 私有函数


        public const string UPLOAD_FINISHED_FLAG = "NNN";	// 上传完毕文件前缀(按照传统规则)
        public const string UPLOAD_UNFINISHED_FLAG = "T";	// 未上传完毕文件前缀
        public const string UPLOAD_UNKNOWN_FLAG = "T";	// 未知状态文件前缀

        // 由文件状态获得文件Flag
        private static string GetFlagByState(FileUploadState fileState)
        {
            string flagStr = UploadFileInfo.UPLOAD_UNKNOWN_FLAG;

            switch (fileState)
            {
                case FileUploadState.FINISHED:
                    flagStr = UploadFileInfo.UPLOAD_FINISHED_FLAG;
                    break;
                case FileUploadState.UNFINISHED:
                    flagStr = UploadFileInfo.UPLOAD_UNFINISHED_FLAG;
                    break;
                default:
                    flagStr = UploadFileInfo.UPLOAD_UNKNOWN_FLAG;
                    break;
            }

            return flagStr;
        }

        #endregion
    }

}