using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Config;
using Formula;
using MvcAdapter;

namespace MvcConfig.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/

        public ActionResult GetSignPic(string userId, bool isAutoDefault = true)
        {
            ImageActionResult result = null;
            IUserService service = FormulaHelper.GetService<IUserService>();
            byte[] signImg = service.GetSignImg(userId);
            if (signImg != null)
            {
                Image image = ImageHelper.BytesToImage(signImg);
                if (image != null)
                {
                    ImageFormat imageFormat = ImageHelper.GetImageFormat(image);
                    result = new ImageActionResult(image, imageFormat);
                }
            }
            else if (isAutoDefault && !string.IsNullOrEmpty(userId))
            {
                UserInfo user = service.GetUserInfoByID(userId);
                if (user != null)
                {
                    Bitmap bitMap = ImageHelper.GetImageFromStr(user.UserName);
                    if (bitMap != null)
                    {
                        result = new ImageActionResult(bitMap);
                    }
                }
            }
            
            if (result == null)
            {
                string noneImageName = Server.MapPath(@"/CommonWebResource/RelateResource/image/signname.jpg");
                result = new ImageActionResult(noneImageName, ImageFormat.Jpeg);
            }
            return result;
        }

        public ActionResult GetUserPic(string userId)
        {
            ImageActionResult result = null;
            IUserService service = FormulaHelper.GetService<IUserService>();
            byte[] signImg = service.GetUserImg(userId);
            if (signImg != null)
            {
                Image image = ImageHelper.BytesToImage(signImg);
                if (image != null)
                {
                    ImageFormat imageFormat = ImageHelper.GetImageFormat(image);
                    result = new ImageActionResult(image, imageFormat);
                }
            }
            if (result == null)
            {
                string noneImageName = Server.MapPath(@"/CommonWebResource/RelateResource/image/photo.jpg");
                result = new ImageActionResult(noneImageName, ImageFormat.Jpeg);
            }
            return result;
        }

    }
}
