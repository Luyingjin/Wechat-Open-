using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileStore.SlUpload
{
    public class UploadConfig
    {
        #region 属性信息

        // 上传者ID
        string _uploaderID = String.Empty;
        public string UploaderID
        {
            get { return this._uploaderID; }
        }

        // 上传者姓名
        string _uploaderName = String.Empty;
        public string UploaderName
        {
            get { return this._uploaderName; }
        }

        // 允许上传文件大小(M)
        float _maxFileSize = 500f;
        public float MaxFileSize
        {
            get { return this._maxFileSize; }
        }

        // 一次上传文件数量控制
        int _maxFileNumber = 20;
        public int MaxFileNumber
        {
            get { return this._maxFileNumber; }
        }

        // 允许上传文件类型
        //string _fileFilter = "*.txt|*.jpg;*.gif";
        string _fileFilter = "所有文件 (*.*)|*.*";
        public string FileFilter
        {
            get { return this._fileFilter; }
        }

        // 处理上传文件的页面
        string _handlerPage = "FileUploadHandler.ashx";
        public string HandlerPage
        {
            get { return this._handlerPage; }
        }

        // 断点续传文件大小(M)
        float _continueSize = 4.096f;	//(默认4M)
        public float ContinueSize
        {
            get { return this._continueSize; }
        }

        // 断点续传文件长度
        public long ContinueLength
        {
            get { return (long)this.ContinueSize * 1024 * 1024; }
        }

        // 是否为上传文件写日志
        bool _isLog = false;
        public bool IsLog
        {
            get { return this._isLog; }
        }

        // 是否为上传文件写日志
        bool _allowThumbnail = false;
        public bool AllowThumbnail
        {
            get { return this._allowThumbnail; }
        }

        #endregion

        #region 构造函数

        public UploadConfig()
        {
            Init();
        }

        // 方便以后扩展，针对每个人设置个性配置时使用
        public UploadConfig(string uploaderID)
        {
            this._uploaderID = uploaderID;

            Init();
        }

        // 初始化类
        private void Init()
        {

            this._maxFileSize = float.Parse(System.Configuration.ConfigurationManager.AppSettings["FS_UploadMaxLength"]);
            this._maxFileNumber = 100;
            this._fileFilter = "所有文件(*.*)|*.*";
            this._handlerPage = "FileUploadHandler.ashx";
            this._continueSize = 50;
            this._isLog = false;
            this._allowThumbnail = false;

        }

        #endregion

        #region 私有方法

        #endregion
    }
}