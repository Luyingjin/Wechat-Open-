using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Configuration;

namespace Base.Areas.PortalBlock.Controllers
{
    public class KindEditorController : Controller
    {
        //文件保存目录路径
        string SavePath = ConfigurationManager.AppSettings["KindEditorSavePath"];

        #region uploadJson

        //
        // GET: /KindEditorHandler/Upload

        public ActionResult Upload()
        {
            ////文件保存目录路径
            //const string savePath = "/Content/Uploads/";

            //文件保存目录URL
            var saveUrl = SavePath;

            //定义允许上传的文件扩展名
            var extTable = new Hashtable
                               {
                                    {"image", "gif,jpg,jpeg,png,bmp"},
                                    {"flash", "swf,flv"},
                                   {"media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb"},
                                   {"file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2"}
                                };

            //最大文件大小
            const int maxSize = 2000000;

            var imgFile = Request.Files["imgFile"];

            if (imgFile == null)
            {
                return ShowError("请选择文件。");
            }

            var dirPath = Server.MapPath(SavePath);
            if (!Directory.Exists(dirPath))
            {
                //return ShowError("上传目录不存在。" + dirPath);
                Directory.CreateDirectory(dirPath);
            }

            var dirName = Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }

            if (!extTable.ContainsKey(dirName))
            {
                return ShowError("目录名不正确。");
            }

            var fileName = imgFile.FileName;
            var extension = Path.GetExtension(fileName);
            if (extension == null)
            {
                return ShowError("extension == null");
            }

            var fileExt = extension.ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                return ShowError("上传文件大小超过限制。");
            }

            if (String.IsNullOrEmpty(fileExt) ||
               Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return ShowError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
            }

            //创建文件夹
            dirPath += dirName + "/";
            saveUrl += dirName + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            dirPath += ymd + "/";
            saveUrl += ymd + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            var filePath = dirPath + newFileName;

            imgFile.SaveAs(filePath);

            var fileUrl = saveUrl + newFileName;

            var hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = fileUrl;

            return Json(hash, "text/html;charset=UTF-8");
        }

        private JsonResult ShowError(string message)
        {
            var hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = message;

            return Json(hash, "text/html;charset=UTF-8");
        }

        #endregion

        #region fileManagerJson

        //
        // GET: /KindEditorHandler/FileManager

        public ActionResult FileManager()
        {
            ////根目录路径，相对路径
            //String rootPath = "/Content/Uploads/";

            //根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
            var rootUrl = SavePath;

            //图片扩展名
            const string fileTypes = "gif,jpg,jpeg,png,bmp";

            String currentPath;
            String currentUrl;
            String currentDirPath;
            String moveupDirPath;

            var dirPath = Server.MapPath(SavePath);
            var dirName = Request.QueryString["dir"];
            if (!String.IsNullOrEmpty(dirName))
            {
                if (Array.IndexOf("image,flash,media,file".Split(','), dirName) == -1)
                {
                    return Content("Invalid Directory name.");
                }
                dirPath += dirName + "/";
                rootUrl += dirName + "/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }

            //根据path参数，设置各路径和URL
            var path = Request.QueryString["path"];
            path = String.IsNullOrEmpty(path) ? "" : path;
            if (path == "")
            {
                currentPath = dirPath;
                currentUrl = rootUrl;
                currentDirPath = "";
                moveupDirPath = "";
            }
            else
            {
                currentPath = dirPath + path;
                currentUrl = rootUrl + path;
                currentDirPath = path;
                moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
            }

            //排序形式，name or size or type
            String order = Request.QueryString["order"];
            order = String.IsNullOrEmpty(order) ? "" : order.ToLower();

            //不允许使用..移动到上一级目录
            if (Regex.IsMatch(path, @"\.\."))
            {
                return Content("Access is not allowed.");
            }

            //最后一个字符不是/
            if (path != "" && !path.EndsWith("/"))
            {
                return Content("Parameter is not valid.");
            }
            //目录不存在或不是目录
            if (!Directory.Exists(currentPath))
            {
                return Content("Directory does not exist.");
            }

            //遍历目录取得文件信息
            string[] dirList = Directory.GetDirectories(currentPath);
            string[] fileList = Directory.GetFiles(currentPath);

            switch (order)
            {
                case "size":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new SizeSorter());
                    break;
                case "type":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new TypeSorter());
                    break;
                default:
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new NameSorter());
                    break;
            }

            var result = new Hashtable();
            result["moveup_dir_path"] = moveupDirPath;
            result["current_dir_path"] = currentDirPath;
            result["current_url"] = currentUrl;
            result["total_count"] = dirList.Length + fileList.Length;
            var dirFileList = new List<Hashtable>();
            result["file_list"] = dirFileList;
            foreach (var t in dirList)
            {
                var dir = new DirectoryInfo(t);
                var hash = new Hashtable();
                hash["is_dir"] = true;
                hash["has_file"] = (dir.GetFileSystemInfos().Length > 0);
                hash["filesize"] = 0;
                hash["is_photo"] = false;
                hash["filetype"] = "";
                hash["filename"] = dir.Name;
                hash["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                dirFileList.Add(hash);
            }
            foreach (var t in fileList)
            {
                var file = new FileInfo(t);
                var hash = new Hashtable();
                hash["is_dir"] = false;
                hash["has_file"] = false;
                hash["filesize"] = file.Length;
                hash["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0);
                hash["filetype"] = file.Extension.Substring(1);
                hash["filename"] = file.Name;
                hash["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                dirFileList.Add(hash);
            }

            return Json(result, "text/html;charset=UTF-8", JsonRequestBehavior.AllowGet);
        }


        private class NameSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                var xInfo = new FileInfo(x.ToString());
                var yInfo = new FileInfo(y.ToString());

                return String.CompareOrdinal(xInfo.FullName, yInfo.FullName);
            }
        }

        private class SizeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                var xInfo = new FileInfo(x.ToString());
                var yInfo = new FileInfo(y.ToString());

                return xInfo.Length.CompareTo(yInfo.Length);
            }
        }

        private class TypeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                var xInfo = new FileInfo(x.ToString());
                var yInfo = new FileInfo(y.ToString());

                return String.CompareOrdinal(xInfo.Extension, yInfo.Extension);
            }
        }

        #endregion

    }
}
