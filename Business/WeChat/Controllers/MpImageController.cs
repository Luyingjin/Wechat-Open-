using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.Logic;
using MvcAdapter;
using WeChat.wechatfile;
using System.Drawing.Imaging;
using Formula.Helper;

namespace WeChat.Controllers
{
    public class MpImageController : BaseController
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

        public ActionResult GetPic()
        {
            var id = GetQueryString("ID");
            FsFileInfo fileInfo = masterService.GetFileInfo(id, WeChatConfig.FileServerName);
            if (fileInfo != null)
            {
                ImageFormat imageFormat = ImageHelper.GetImageFormat(fileInfo.FileName.Split('.').Last());
                return new ImageActionResult(fileInfo.FileFullPath, imageFormat);
            }
            return new ImageActionResult(Server.MapPath(@"/CommonWebResource/RelateResource/image/photo.jpg"), ImageFormat.Jpeg);
        }

        public ActionResult UploadPic()
        {
            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));
                var fileinfo = masterService.AddFile(WeChatConfig.FileServerName, Request.Files[0].FileName, t.Length, "", "", "", "");
                return Json(fileinfo);
            }
            return Json("");
        }
    }
}
