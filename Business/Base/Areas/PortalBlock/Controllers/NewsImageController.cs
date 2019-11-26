using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Config;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Formula.Helper;

namespace Base.Areas.PortalBlock.Controllers
{
    public class NewsImageController : BaseController
    {
        private const string NewsImagePrefix = "NewsImage_";
        //
        // GET: /PortalBlock/NewsImage/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult Gallerys()
        {
            return View();
        }

        public ActionResult GetPic(string groupID, string id, int? width, int? height)
        {
            S_I_NewsImage newsImage = GetNewsImage(groupID, id);
            if (newsImage != null)
            {
                if (newsImage.PictureEntire != null)
                {
                    return new ImageActionResult(newsImage.PictureEntire, width, height);
                }
            }
            return Content(string.Empty);
        }

        public ActionResult GetPicThumb(string groupID, string id, int? width, int? height)
        {
            S_I_NewsImage newsImage = GetNewsImage(groupID, id);
            if (newsImage != null)
            {
                if (newsImage.PictureThumb != null)
                {
                    return new ImageActionResult(newsImage.PictureThumb, width, height);
                }
            }
            return Content(string.Empty);
        }

        public JsonResult Delete(string id)
        {
            entities.Set<S_I_NewsImage>().Delete(c => c.GroupID == id);
            entities.Set<S_I_NewsImageGroup>().Delete(c => c.ID == id);
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult GetModel(string id, string title)
        {
            S_I_NewsImageGroup model = GetEntity<S_I_NewsImageGroup>(id);
            if (!string.IsNullOrEmpty(title))
                model.Title = title;
            return Json(model);
        }

        public JsonResult GetModelByDeptHome(string id)
        {
            S_I_NewsImageGroup model = GetEntity<S_I_NewsImageGroup>(id);
            if (string.IsNullOrEmpty(id))
            {
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.DeptDoorId = user.UserOrgID;
                model.DeptDoorName = user.UserOrgName;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = user.UserID;
                model.CreateUserName = user.UserName;
            }
            return Json(model);
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            IQueryable<S_I_NewsImageGroup> list = entities.Set<S_I_NewsImageGroup>().Where(c => string.IsNullOrEmpty(c.DeptDoorId)).AsQueryable();
            GridData gridData = list.WhereToGridData(qb);
            return Json(gridData);
        }

        public JsonResult GetPictures(string id)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            id = id == null ? "" : id;
            DataTable dt = sqlHelper.ExecuteDataTable("select ID,GroupID,PictureName,Description,SortIndex from S_I_NewsImage where GroupID = '" + id + "' order by SortIndex");
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPicturesInfo(string id)
        {
            id = id == null ? "" : id;
            List<S_I_NewsImage> list = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == id && c.PictureThumb != null && c.PictureEntire != null).OrderBy(c => c.SortIndex).ToList();
            if (!string.IsNullOrEmpty(id))
                Formula.Helper.CacheHelper.Set(NewsImagePrefix + id, list);

            for (int i = 0; i < list.Count; i++)
            {
                list[i].PictureEntire = null;
                list[i].PictureThumb = null;
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewsImageGroupExtend(string deptHomeID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,b.ID as NewsImageID from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage group by GroupID
                            ) a join S_I_NewsImage b on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c on c.ID=b.GroupID
                            {0}
                            order by c.CreateTime desc";
            string where = !string.IsNullOrEmpty(deptHomeID) ? "where DeptDoorId = '" + deptHomeID + "'" : "where isnull(DeptDoorId,'') = '' ";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, where));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewsImageGroupByDeptHome(string deptHomeID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,c.Remark,b.ID as NewsImageID,b.CreateTime from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage group by GroupID
                            ) a join S_I_NewsImage b on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c on c.ID=b.GroupID
                            where DeptDoorId = '{0}'
                            order by c.CreateTime desc";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, deptHomeID));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadPictures()
        {
            if (Request.Files["FileData"] != null && !string.IsNullOrEmpty(Request["GroupID"]))
            {
                var t = Request.Files["FileData"].InputStream;
                string fileName = Request.Files["FileData"].FileName;
                string extName = fileName.Substring(fileName.LastIndexOf(".") + 1, (fileName.Length - fileName.LastIndexOf(".") - 1)); ;
                Image img = Image.FromStream(t);
                ImageFormat imgFormat = ImageHelper.GetImageFormat(extName);
                byte[] bt = ImageHelper.ImageToBytes(img, imgFormat);
                int height = img.Height;
                int width = img.Width;
                int limitedHeight = !string.IsNullOrEmpty(Request["ThumbHeight"]) ? Convert.ToInt32(Request["ThumbHeight"]) : 60;
                int thumbHeight, thumbWidth;
                byte[] btThumb = null;
                if (height > limitedHeight)
                {
                    thumbHeight = limitedHeight;
                    thumbWidth = thumbHeight * width / height;
                    Image imgThumb = img.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero);
                    btThumb = ImageHelper.ImageToBytes(imgThumb, imgFormat);
                }
                else
                {
                    btThumb = bt;
                }
                S_I_NewsImage newsImage = new S_I_NewsImage();
                string groupID = Request["GroupID"];
                newsImage.ID = Formula.FormulaHelper.CreateGuid();
                newsImage.GroupID = Request["GroupID"];
                newsImage.PictureName = fileName;
                newsImage.PictureEntire = bt;
                newsImage.PictureThumb = btThumb;
                newsImage.SortIndex = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == groupID).Count();
                newsImage.CreateTime = DateTime.Now;
                newsImage.CreateUserID = Formula.FormulaHelper.GetUserInfo().UserID;
                newsImage.CreateUserName = Formula.FormulaHelper.GetUserInfo().UserName;

                entities.Set<S_I_NewsImage>().Add(newsImage);
                entities.SaveChanges();

                return Json(new { ID = newsImage.ID, GroupID = newsImage.GroupID, PictureName = newsImage.PictureName });
            }
            else
            {
                return Json(string.Empty);
            }
        }

        public JsonResult DeleteNewsImage(string groupID, string id)
        {
            base.JsonDelete<S_I_NewsImage>(id);
            List<S_I_NewsImage> list = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == groupID).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SortIndex = i;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult Save()
        {
            var obj = UpdateEntity<S_I_NewsImageGroup>();
            var List = UpdateList<S_I_NewsImage>();
            entities.SaveChanges();

            return Json("");
        }

        private S_I_NewsImage GetNewsImage(string groupID, string id)
        {
            S_I_NewsImage model = null;
            groupID = groupID == null ? "" : groupID;
            if (Formula.Helper.CacheHelper.Get(NewsImagePrefix + groupID) != null)
            {
                List<S_I_NewsImage> list = (List<S_I_NewsImage>)Formula.Helper.CacheHelper.Get(NewsImagePrefix + groupID);
                model = list.Find(t => t.ID == id);
            }
            else
            {
                model = entities.Set<S_I_NewsImage>().Find(id);
            }
            return model;
        }
    }
}
