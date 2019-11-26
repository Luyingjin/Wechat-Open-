using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;

namespace WeChat.Controllers
{
    public class MpSelfArticleController : BaseController<MpSelfArticle>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpSelfArticle>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult Save()
        {
            #region 数据校验
            MpSelfArticle entity = UpdateEntity<MpSelfArticle>();
            if (string.IsNullOrEmpty(entity.Title))
                throw new BusinessException("标题不能为空");
            #endregion

            #region 保存数据
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = GetQueryString("MpID");
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {
            #region 假删除
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpSelfArticle>(ID);
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }

        public JsonResult GetListBoxData()
        {
            string MpID = Request["MpID"];
            var result = entities.Set<MpSelfArticle>().Where(c => c.MpID == MpID && c.IsDelete == 0)
                .Select(c => new
                {
                    c.ID,
                    c.Title,
                    c.Description,
                    c.Url
                }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
