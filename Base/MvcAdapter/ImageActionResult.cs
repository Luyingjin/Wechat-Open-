using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using Formula.Helper;
using System.IO;

namespace MvcAdapter
{
    public class ImageActionResult : ActionResult
    {
        public ImageFormat ContentType { get; set; }
        public Bitmap bitMap { get; set; }
        public string SourceName { get; set; }
        public ImageActionResult(string _SourceName, ImageFormat _ContentType)
        {
            this.SourceName = _SourceName;
            this.ContentType = _ContentType;
        }
        public ImageActionResult(byte[] _byte, int? _width, int? _height)
        {
            Image img = ImageHelper.GetImageFromBytes(_byte);
            if (_width != null || _height != null)
            {
                if (_width != null)
                {
                    if (_height == null)
                        _height = _width.Value * img.Height / img.Width;
                }
                else
                {
                    if (_height != null)
                        _width = _height.Value * img.Width / img.Height;
                }
                img = img.GetThumbnailImage(_width.Value, _height.Value, null, IntPtr.Zero);
            }

            this.bitMap = new Bitmap(img);
            this.ContentType = ImageHelper.GetImageFormat(img);
        }
        public ImageActionResult(Image _image)
        {
            this.bitMap = new Bitmap(_image);
            this.ContentType = ImageHelper.GetImageFormat(_image);
        }
        public ImageActionResult(Image _image, ImageFormat _ContentType)
        {
            this.bitMap = new Bitmap(_image);
            this.ContentType = _ContentType;
        }
        public ImageActionResult(Bitmap _bitMap)
        {
            this.bitMap = _bitMap;
            this.ContentType = ImageFormat.Gif;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (ContentType.Equals(ImageFormat.Bmp)) context.HttpContext.Response.ContentType = "image/bmp";
            else if (ContentType.Equals(ImageFormat.Gif)) context.HttpContext.Response.ContentType = "image/gif";
            else if (ContentType.Equals(ImageFormat.Icon)) context.HttpContext.Response.ContentType = "image/vnd.microsoft.icon";
            else if (ContentType.Equals(ImageFormat.Jpeg)) context.HttpContext.Response.ContentType = "image/jpeg";
            else if (ContentType.Equals(ImageFormat.Png)) context.HttpContext.Response.ContentType = "image/png";
            else if (ContentType.Equals(ImageFormat.Tiff)) context.HttpContext.Response.ContentType = "image/tiff";
            else if (ContentType.Equals(ImageFormat.Wmf)) context.HttpContext.Response.ContentType = "image/wmf";
            else
            {
                context.HttpContext.Response.ContentType = "image/gif";
                this.ContentType = ImageFormat.Gif;
            }
            if (bitMap != null)
            {
                if (ContentType.Equals(ImageFormat.Png))
                {
                    MemoryStream mem = new MemoryStream();
                    bitMap.Save(mem, ContentType);
                    mem.WriteTo(context.HttpContext.Response.OutputStream);
                }
                else
                {
                    bitMap.Save(context.HttpContext.Response.OutputStream, ContentType);
                }
            }
            else
            {
                context.HttpContext.Response.TransmitFile(SourceName);
            }
        }    }
}
