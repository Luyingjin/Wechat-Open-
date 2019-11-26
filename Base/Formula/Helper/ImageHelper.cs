using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Formula.Helper
{
    public class ImageHelper
    {
        /// <summary>
        /// byte[]转换成Image
        /// </summary>
        /// <param name="byteArrayIn">二进制图片流</param>
        /// <returns>Image</returns>
        public static System.Drawing.Image BytesToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                ms.Flush();
                return returnImage;
            }
        }

        /// <summary>
        /// byte[] 转换 Bitmap
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// 将图片Image转换成Byte[]
        /// </summary>
        /// <param name="Image">image对象</param>
        /// <param name="imageFormat">后缀名</param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image Image, ImageFormat imageFormat)
        {
            if (Image == null) { return null; }
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap Bitmap = new Bitmap(Image))
                {
                    Bitmap.Save(ms, imageFormat);
                    ms.Position = 0;
                    data = new byte[ms.Length];
                    ms.Read(data, 0, Convert.ToInt32(ms.Length));
                    ms.Flush();
                }
            }
            return data;
        }

        /// <summary>
        /// 根据图形获取图形类型
        /// </summary>
        /// <param name="p_Image"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(Image p_Image)
        {
            string strExtName = GetImageExtension(p_Image);
            return GetImageFormat(strExtName);
        }

        /// <summary>
        /// 根据后缀获取图形类型
        /// </summary>
        /// <param name="p_Image"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string strExtName)
        {
            ImageFormat imageFormat;
            switch (strExtName.ToLower())
            {
                case "jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                case "png":
                    imageFormat = ImageFormat.Png;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                default:
                    imageFormat = ImageFormat.Gif;
                    break;
            }
            return imageFormat;
        }

        /// <summary>
        /// 根据图形获取图形的扩展名
        /// </summary>
        /// <param name="p_Image">图形</param>
        /// <returns>扩展名</returns>
        public static string GetImageExtension(Image p_Image)
        {
            Type Type = typeof(ImageFormat);
            System.Reflection.PropertyInfo[] _ImageFormatList = Type.GetProperties(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i != _ImageFormatList.Length; i++)
            {
                ImageFormat _FormatClass = (ImageFormat)_ImageFormatList[i].GetValue(null, null);
                if (_FormatClass.Guid.Equals(p_Image.RawFormat.Guid))
                {
                    return _ImageFormatList[i].Name;
                }
            }
            return "";
        }

        // 由图片地址创建图片
        public static Bitmap GetImageFromBytes(byte[] bytes)
        {
            Bitmap bitmap;

            try
            {
                MemoryStream memStream = new MemoryStream(bytes);
                bitmap = new Bitmap(memStream);
            }
            catch
            {
                bitmap = null;
            }

            return bitmap;
        }

        // 将图片转换为byte类型以便于存储在数据库中
        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            MemoryStream memStream = null;
            try
            {
                memStream = new MemoryStream();
                bitmap.Save(memStream, bitmap.RawFormat);
                byte[] bytes = new byte[memStream.Length];
                memStream.Read(bytes, 0, (int)bytes.Length);

                return bytes;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                memStream.Close();
            }
        }
    }
}
