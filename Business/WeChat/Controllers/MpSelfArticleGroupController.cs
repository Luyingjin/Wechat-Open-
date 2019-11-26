using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;
using Formula.Helper;

namespace WeChat.Controllers
{
    public class MpSelfArticleGroupController : BaseController<MpSelfArticleGroup>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpSelfArticleGroup>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult Save()
        {
            #region 数据校验
            var mpid = GetQueryString("MpID");
            MpSelfArticleGroup entity = UpdateEntity<MpSelfArticleGroup>();
            if (string.IsNullOrEmpty(entity.Name))
                throw new BusinessException("名称不能为空");

            var listData = Request.Form["SubItemIDs"];
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(listData);
            var listids = rows.Select(c => c["ID"].ToString());
            var articlelist = entities.Set<MpSelfArticle>().Where(c => c.MpID == mpid && listids.Contains(c.ID)).ToList();
            if (articlelist.Count() < 2)
                throw new BusinessException("图文数量不能小于2");
            #endregion

            #region 保存数据
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = mpid;
            var sortedlist = listids.Select(c => articlelist.Where(d => d.ID == c).FirstOrDefault()).Where(c => c != null);

            var oldlist = entities.Set<MpSelfArticleGroupItem>().Where(c => c.MpID == mpid && c.GroupID == entity.ID);
            foreach (var old in oldlist)
                entities.Set<MpSelfArticleGroupItem>().Remove(old);
            foreach (var newitem in sortedlist.Select((c, i) => new { c, i }))
            {
                var subitem = GetEntity<MpSelfArticleGroupItem>("");
                subitem.GroupID = entity.ID;
                subitem.MpID = entity.MpID;
                subitem.ArticleID = newitem.c.ID;
                subitem.SortIndex = newitem.i;
                entities.Set<MpSelfArticleGroupItem>().Add(subitem);
            }
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {
            #region 假删除
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpMediaArticleGroup>(ID);
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }

        public JsonResult GetItemList()
        {
            var GroupID = GetQueryString("GroupID");
            var result = entities.Set<MpSelfArticleGroupItem>().Where(c => c.GroupID == GroupID).OrderBy(c => c.SortIndex)
                .Select(c => new
                {
                    c.MpSelfArticle.ID,
                    c.MpSelfArticle.Title,
                    c.MpSelfArticle.Description,
                    c.MpSelfArticle.Url
                }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
