using System;
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
    public class MpMediaArticleGroupController : BaseController<MpMediaArticleGroup>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpMediaArticleGroup>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult Save()
        {
            #region 数据校验
            var mpid=GetQueryString("MpID");
            MpMediaArticleGroup entity = UpdateEntity<MpMediaArticleGroup>();
            if (string.IsNullOrEmpty(entity.Name))
                throw new BusinessException("名称不能为空");

            var listData = Request.Form["SubItemIDs"];
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(listData);
            var listids=rows.Select(c=>c["ID"].ToString());
            var articlelist = entities.Set<MpMediaArticle>().Where(c => c.MpID == mpid && listids.Contains(c.ID)).ToList();
            if (articlelist.Count() < 2)
                throw new BusinessException("图文数量不能小于2");
            #endregion

            #region 微信处理
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = mpid;

            var sortedlist = listids.Select(c => articlelist.Where(d => d.ID == c).FirstOrDefault()).Where(c => c != null);

            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            if (entity._state == EntityStatus.added.ToString())
            {
                var mediaid = wxFO.AddMediaNews(entity.MpID, sortedlist.ToArray());
                entity.MediaID = mediaid;
            }
            else
            {
                foreach (var m in sortedlist.Select((c, i) => new { c, i }))
                {
                    wxFO.UpdateMediaNews(entity.MpID, entity.MediaID,m.i, m.c);
                }
                //因为素材修改接口调不通，所以修改的时候，实际上是新增一个素材，再删掉老的素材
                //var mediaid = wxFO.AddMediaNews(entity.MpID, sortedlist.ToArray());
                //wxFO.DelMediaFile(entity.MpID, entity.MediaID);
                //entity.MediaID = mediaid;
            }
            #endregion

            #region 保存数据
            var oldlist = entities.Set<MpMediaArticleGroupItem>().Where(c => c.MpID == mpid && c.GroupID == entity.ID);
            foreach (var old in oldlist)
                entities.Set<MpMediaArticleGroupItem>().Remove(old);
            foreach (var newitem in sortedlist.Select((c, i) => new { c, i }))
            {
                var subitem = GetEntity<MpMediaArticleGroupItem>("");
                subitem.GroupID = entity.ID;
                subitem.MpID = entity.MpID;
                subitem.ArticleID = newitem.c.ID;
                subitem.SortIndex = newitem.i;
                entities.Set<MpMediaArticleGroupItem>().Add(subitem);
            }
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {

            #region 微信处理
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpMediaArticleGroup>(ID);
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.DelMediaFile(entity.MpID, entity.MediaID);
            #endregion

            #region 假删除
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }

        public JsonResult ViewGroupArticle()
        {
            string MpID = Request["MpID"];
            string MediaID = Request["MediaID"];
            string NickName = Request["NickName"];
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            try
            {
                var fans = entities.Set<MpFans>().Where(c => c.MpID == MpID && c.NickName == NickName).ToList();
                if (fans.Count() == 0)
                    throw new Exception(string.Format("找不到昵称为“{0}”的粉丝",NickName));
                foreach (var f in fans)
                    wxFO.PreViewMedia(MpID, MediaID, f.OpenID);
                return Json(new { error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetItemList()
        {
            var GroupID = GetQueryString("GroupID");
            var result = entities.Set<MpMediaArticleGroupItem>().Where(c => c.GroupID == GroupID).OrderBy(c => c.SortIndex)
                .Select(c => new
                {
                    c.MpMediaArticle.ID,
                    c.MpMediaArticle.Title,
                    c.MpMediaArticle.Description,
                    c.MpMediaArticle.Author
                }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
