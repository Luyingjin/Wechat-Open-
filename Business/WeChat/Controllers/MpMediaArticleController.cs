using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Exceptions;
using Formula.Helper;
using MvcAdapter;
using WeChat.Logic.BusinessFacade;
using WeChat.Logic.Domain;

namespace WeChat.Controllers
{
    public class MpMediaArticleController : BaseController<MpMediaArticle>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpMediaArticle>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            #region 数据校验
            MpMediaArticle entity = UpdateEntity<MpMediaArticle>();
            if (string.IsNullOrEmpty(entity.PicFileID))
                throw new BusinessException("封面不能为空");
            if (string.IsNullOrEmpty(entity.Title))
                throw new BusinessException("标题不能为空");
            #endregion

            #region 微信处理
            string SavePath = ConfigurationManager.AppSettings["KindEditorSavePath"];
            entity.Content = entity.Content.Replace(string.Format("src=\"{0}", SavePath), string.Format("src=\"{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, SavePath));
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = GetQueryString("MpID");
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            if (entity._state == EntityStatus.added.ToString())
            {
                var mediaid = wxFO.AddMediaNews(entity.MpID, entity);
                entity.MediaID = mediaid;
            }
            else
            {

                wxFO.UpdateMediaNews(entity.MpID, entity.MediaID, 0, entity);
                //因为素材修改接口调不通，所以修改的时候，实际上是新增一个素材，再删掉老的素材
                //var mediaid = wxFO.AddMediaNews(entity.MpID, entity);
                //wxFO.DelMediaFile(entity.MpID, entity.MediaID);
                //entity.MediaID = mediaid;
            }
            #endregion

            #region 保存数据
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {

            #region 微信处理
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpMediaArticle>(ID);
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.DelMediaFile(entity.MpID, entity.MediaID);
            #endregion

            #region 假删除
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }

        public JsonResult ViewArticle()
        {
            string MpID = Request["MpID"];
            string MediaID = Request["MediaID"];
            string NickName = Request["NickName"];
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            try
            {
                var fans = entities.Set<MpFans>().Where(c => c.MpID == MpID && c.NickName == NickName).ToList();
                if (fans.Count() == 0)
                    throw new Exception(string.Format("找不到昵称为“{0}”的粉丝", NickName));
                foreach (var f in fans)
                    wxFO.PreViewMedia(MpID, MediaID, f.OpenID);
                return Json(new { error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetListBoxData()
        {
            string MpID = Request["MpID"];
            var result = entities.Set<MpMediaArticle>().Where(c => c.MpID == MpID && c.IsDelete == 0)
                .Select(c => new { 
                    c.ID,c.Title,c.Description,c.Author
                }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
