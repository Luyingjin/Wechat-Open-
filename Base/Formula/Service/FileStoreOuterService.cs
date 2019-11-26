using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStoreReference
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "OuterServiceSoap", Namespace = "http://tempuri.org/")]
    public partial class OuterService : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback LocalizeFileOperationCompleted;

        private System.Threading.SendOrPostCallback GetFileFullPathOperationCompleted;

        private System.Threading.SendOrPostCallback CopyFileOperationCompleted;

        private System.Threading.SendOrPostCallback DelFileByRelateIdOperationCompleted;

        private System.Threading.SendOrPostCallback OverwriteFileOperationCompleted;

        private System.Threading.SendOrPostCallback DelFilesOperationCompleted;

        private System.Threading.SendOrPostCallback GetFileNamesByRelateIdOperationCompleted;

        private System.Threading.SendOrPostCallback SaveFileOperationCompleted;

        private System.Threading.SendOrPostCallback AppendFileOperationCompleted;

        private System.Threading.SendOrPostCallback GetFileSizeOperationCompleted;

        private System.Threading.SendOrPostCallback GetFileBytesOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public OuterService()
        {
            string str = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
            str = str.Replace("MasterService.asmx", "OuterService.asmx");
            this.Url = str;
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event LocalizeFileCompletedEventHandler LocalizeFileCompleted;

        /// <remarks/>
        public event GetFileFullPathCompletedEventHandler GetFileFullPathCompleted;

        /// <remarks/>
        public event CopyFileCompletedEventHandler CopyFileCompleted;

        /// <remarks/>
        public event DelFileByRelateIdCompletedEventHandler DelFileByRelateIdCompleted;

        /// <remarks/>
        public event OverwriteFileCompletedEventHandler OverwriteFileCompleted;

        /// <remarks/>
        public event DelFilesCompletedEventHandler DelFilesCompleted;

        /// <remarks/>
        public event GetFileNamesByRelateIdCompletedEventHandler GetFileNamesByRelateIdCompleted;

        /// <remarks/>
        public event SaveFileCompletedEventHandler SaveFileCompleted;

        /// <remarks/>
        public event AppendFileCompletedEventHandler AppendFileCompleted;

        /// <remarks/>
        public event GetFileSizeCompletedEventHandler GetFileSizeCompleted;

        /// <remarks/>
        public event GetFileBytesCompletedEventHandler GetFileBytesCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/LocalizeFile", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string LocalizeFile(string fileName)
        {
            object[] results = this.Invoke("LocalizeFile", new object[] {
                        fileName});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void LocalizeFileAsync(string fileName)
        {
            this.LocalizeFileAsync(fileName, null);
        }

        /// <remarks/>
        public void LocalizeFileAsync(string fileName, object userState)
        {
            if ((this.LocalizeFileOperationCompleted == null))
            {
                this.LocalizeFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLocalizeFileOperationCompleted);
            }
            this.InvokeAsync("LocalizeFile", new object[] {
                        fileName}, this.LocalizeFileOperationCompleted, userState);
        }

        private void OnLocalizeFileOperationCompleted(object arg)
        {
            if ((this.LocalizeFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LocalizeFileCompleted(this, new LocalizeFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFileFullPath", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetFileFullPath(string fileName)
        {
            object[] results = this.Invoke("GetFileFullPath", new object[] {
                        fileName});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetFileFullPathAsync(string fileName)
        {
            this.GetFileFullPathAsync(fileName, null);
        }

        /// <remarks/>
        public void GetFileFullPathAsync(string fileName, object userState)
        {
            if ((this.GetFileFullPathOperationCompleted == null))
            {
                this.GetFileFullPathOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileFullPathOperationCompleted);
            }
            this.InvokeAsync("GetFileFullPath", new object[] {
                        fileName}, this.GetFileFullPathOperationCompleted, userState);
        }

        private void OnGetFileFullPathOperationCompleted(object arg)
        {
            if ((this.GetFileFullPathCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileFullPathCompleted(this, new GetFileFullPathCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CopyFile", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CopyFile(string fileName, string relateId, string fileCode, string version, string src)
        {
            object[] results = this.Invoke("CopyFile", new object[] {
                        fileName,
                        relateId,
                        fileCode,
                        version,
                        src});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void CopyFileAsync(string fileName, string relateId, string fileCode, string version, string src)
        {
            this.CopyFileAsync(fileName, relateId, fileCode, version, src, null);
        }

        /// <remarks/>
        public void CopyFileAsync(string fileName, string relateId, string fileCode, string version, string src, object userState)
        {
            if ((this.CopyFileOperationCompleted == null))
            {
                this.CopyFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCopyFileOperationCompleted);
            }
            this.InvokeAsync("CopyFile", new object[] {
                        fileName,
                        relateId,
                        fileCode,
                        version,
                        src}, this.CopyFileOperationCompleted, userState);
        }

        private void OnCopyFileOperationCompleted(object arg)
        {
            if ((this.CopyFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CopyFileCompleted(this, new CopyFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DelFileByRelateId", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DelFileByRelateId(string relateId, string delReason)
        {
            object[] results = this.Invoke("DelFileByRelateId", new object[] {
                        relateId,
                        delReason});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DelFileByRelateIdAsync(string relateId, string delReason)
        {
            this.DelFileByRelateIdAsync(relateId, delReason, null);
        }

        /// <remarks/>
        public void DelFileByRelateIdAsync(string relateId, string delReason, object userState)
        {
            if ((this.DelFileByRelateIdOperationCompleted == null))
            {
                this.DelFileByRelateIdOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDelFileByRelateIdOperationCompleted);
            }
            this.InvokeAsync("DelFileByRelateId", new object[] {
                        relateId,
                        delReason}, this.DelFileByRelateIdOperationCompleted, userState);
        }

        private void OnDelFileByRelateIdOperationCompleted(object arg)
        {
            if ((this.DelFileByRelateIdCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DelFileByRelateIdCompleted(this, new DelFileByRelateIdCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/OverwriteFile", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool OverwriteFile(string fileId, string relateId, string fileCode, string version, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] bytes, long fileSize, string src)
        {
            object[] results = this.Invoke("OverwriteFile", new object[] {
                        fileId,
                        relateId,
                        fileCode,
                        version,
                        bytes,
                        fileSize,
                        src});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void OverwriteFileAsync(string fileId, string relateId, string fileCode, string version, byte[] bytes, long fileSize, string src)
        {
            this.OverwriteFileAsync(fileId, relateId, fileCode, version, bytes, fileSize, src, null);
        }

        /// <remarks/>
        public void OverwriteFileAsync(string fileId, string relateId, string fileCode, string version, byte[] bytes, long fileSize, string src, object userState)
        {
            if ((this.OverwriteFileOperationCompleted == null))
            {
                this.OverwriteFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnOverwriteFileOperationCompleted);
            }
            this.InvokeAsync("OverwriteFile", new object[] {
                        fileId,
                        relateId,
                        fileCode,
                        version,
                        bytes,
                        fileSize,
                        src}, this.OverwriteFileOperationCompleted, userState);
        }

        private void OnOverwriteFileOperationCompleted(object arg)
        {
            if ((this.OverwriteFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.OverwriteFileCompleted(this, new OverwriteFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DelFiles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DelFiles(string fileIds, string delReason)
        {
            object[] results = this.Invoke("DelFiles", new object[] {
                        fileIds,
                        delReason});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DelFilesAsync(string fileIds, string delReason)
        {
            this.DelFilesAsync(fileIds, delReason, null);
        }

        /// <remarks/>
        public void DelFilesAsync(string fileIds, string delReason, object userState)
        {
            if ((this.DelFilesOperationCompleted == null))
            {
                this.DelFilesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDelFilesOperationCompleted);
            }
            this.InvokeAsync("DelFiles", new object[] {
                        fileIds,
                        delReason}, this.DelFilesOperationCompleted, userState);
        }

        private void OnDelFilesOperationCompleted(object arg)
        {
            if ((this.DelFilesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DelFilesCompleted(this, new DelFilesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFileNamesByRelateId", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetFileNamesByRelateId(string relateId)
        {
            object[] results = this.Invoke("GetFileNamesByRelateId", new object[] {
                        relateId});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetFileNamesByRelateIdAsync(string relateId)
        {
            this.GetFileNamesByRelateIdAsync(relateId, null);
        }

        /// <remarks/>
        public void GetFileNamesByRelateIdAsync(string relateId, object userState)
        {
            if ((this.GetFileNamesByRelateIdOperationCompleted == null))
            {
                this.GetFileNamesByRelateIdOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileNamesByRelateIdOperationCompleted);
            }
            this.InvokeAsync("GetFileNamesByRelateId", new object[] {
                        relateId}, this.GetFileNamesByRelateIdOperationCompleted, userState);
        }

        private void OnGetFileNamesByRelateIdOperationCompleted(object arg)
        {
            if ((this.GetFileNamesByRelateIdCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileNamesByRelateIdCompleted(this, new GetFileNamesByRelateIdCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SaveFile", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SaveFile(string name, string relateId, string code, string version, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] bytes, long fileSize, string src)
        {
            object[] results = this.Invoke("SaveFile", new object[] {
                        name,
                        relateId,
                        code,
                        version,
                        bytes,
                        fileSize,
                        src});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void SaveFileAsync(string name, string relateId, string code, string version, byte[] bytes, long fileSize, string src)
        {
            this.SaveFileAsync(name, relateId, code, version, bytes, fileSize, src, null);
        }

        /// <remarks/>
        public void SaveFileAsync(string name, string relateId, string code, string version, byte[] bytes, long fileSize, string src, object userState)
        {
            if ((this.SaveFileOperationCompleted == null))
            {
                this.SaveFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveFileOperationCompleted);
            }
            this.InvokeAsync("SaveFile", new object[] {
                        name,
                        relateId,
                        code,
                        version,
                        bytes,
                        fileSize,
                        src}, this.SaveFileOperationCompleted, userState);
        }

        private void OnSaveFileOperationCompleted(object arg)
        {
            if ((this.SaveFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveFileCompleted(this, new SaveFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/AppendFile", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string AppendFile(string fileId, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] bytes)
        {
            object[] results = this.Invoke("AppendFile", new object[] {
                        fileId,
                        bytes});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void AppendFileAsync(string fileId, byte[] bytes)
        {
            this.AppendFileAsync(fileId, bytes, null);
        }

        /// <remarks/>
        public void AppendFileAsync(string fileId, byte[] bytes, object userState)
        {
            if ((this.AppendFileOperationCompleted == null))
            {
                this.AppendFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAppendFileOperationCompleted);
            }
            this.InvokeAsync("AppendFile", new object[] {
                        fileId,
                        bytes}, this.AppendFileOperationCompleted, userState);
        }

        private void OnAppendFileOperationCompleted(object arg)
        {
            if ((this.AppendFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AppendFileCompleted(this, new AppendFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFileSize", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int GetFileSize(string fileId)
        {
            object[] results = this.Invoke("GetFileSize", new object[] {
                        fileId});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void GetFileSizeAsync(string fileId)
        {
            this.GetFileSizeAsync(fileId, null);
        }

        /// <remarks/>
        public void GetFileSizeAsync(string fileId, object userState)
        {
            if ((this.GetFileSizeOperationCompleted == null))
            {
                this.GetFileSizeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileSizeOperationCompleted);
            }
            this.InvokeAsync("GetFileSize", new object[] {
                        fileId}, this.GetFileSizeOperationCompleted, userState);
        }

        private void OnGetFileSizeOperationCompleted(object arg)
        {
            if ((this.GetFileSizeCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileSizeCompleted(this, new GetFileSizeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFileBytes", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] GetFileBytes(string fileId, int pos, int length)
        {
            object[] results = this.Invoke("GetFileBytes", new object[] {
                        fileId,
                        pos,
                        length});
            return ((byte[])(results[0]));
        }

        /// <remarks/>
        public void GetFileBytesAsync(string fileId, int pos, int length)
        {
            this.GetFileBytesAsync(fileId, pos, length, null);
        }

        /// <remarks/>
        public void GetFileBytesAsync(string fileId, int pos, int length, object userState)
        {
            if ((this.GetFileBytesOperationCompleted == null))
            {
                this.GetFileBytesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileBytesOperationCompleted);
            }
            this.InvokeAsync("GetFileBytes", new object[] {
                        fileId,
                        pos,
                        length}, this.GetFileBytesOperationCompleted, userState);
        }

        private void OnGetFileBytesOperationCompleted(object arg)
        {
            if ((this.GetFileBytesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileBytesCompleted(this, new GetFileBytesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void LocalizeFileCompletedEventHandler(object sender, LocalizeFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LocalizeFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal LocalizeFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetFileFullPathCompletedEventHandler(object sender, GetFileFullPathCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFileFullPathCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetFileFullPathCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void CopyFileCompletedEventHandler(object sender, CopyFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CopyFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CopyFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void DelFileByRelateIdCompletedEventHandler(object sender, DelFileByRelateIdCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DelFileByRelateIdCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DelFileByRelateIdCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void OverwriteFileCompletedEventHandler(object sender, OverwriteFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class OverwriteFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal OverwriteFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void DelFilesCompletedEventHandler(object sender, DelFilesCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DelFilesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DelFilesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetFileNamesByRelateIdCompletedEventHandler(object sender, GetFileNamesByRelateIdCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFileNamesByRelateIdCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetFileNamesByRelateIdCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void SaveFileCompletedEventHandler(object sender, SaveFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SaveFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SaveFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void AppendFileCompletedEventHandler(object sender, AppendFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AppendFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal AppendFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetFileSizeCompletedEventHandler(object sender, GetFileSizeCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFileSizeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetFileSizeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetFileBytesCompletedEventHandler(object sender, GetFileBytesCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFileBytesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetFileBytesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public byte[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
}

