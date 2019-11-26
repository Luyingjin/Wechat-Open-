using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using WeChatService.wechatfile;
using WeChat.Logic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace WeChatService.Controllers
{
    public class ImageController : ApiController
    {
        private MasterService _masterService = null;
        private MasterService masterService
        {
            get
            {
                if (_masterService == null)
                {
                    _masterService = new MasterService();
                    _masterService.Url = WeChatConfig.MasterServerUrl;
                }
                return _masterService;
            }
        }

        // GET api/<controller>/5
        public virtual HttpResponseMessage Get(string id)
        {
            FsFileInfo fileInfo = masterService.GetFileInfo(id, WeChatConfig.FileServerName);
            if (fileInfo != null)
            {

                //从图片中读取byte
                var imgByte = File.ReadAllBytes(fileInfo.FileFullPath);
                //从图片中读取流
                var imgStream = new MemoryStream(File.ReadAllBytes(fileInfo.FileFullPath));
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(imgByte)
                    //或者
                    //Content = new StreamContent(stream)
                };
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
