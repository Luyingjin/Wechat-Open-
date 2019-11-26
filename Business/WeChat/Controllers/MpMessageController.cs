using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.Logic.BusinessFacade;
using MvcAdapter;

namespace WeChat.Controllers
{
    public class MpMessageController : BaseController<MpMessage>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpMessage>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult Save()
        {
            #region 数据校验
            MpMessage entity = UpdateEntity<MpMessage>();
            #endregion

            #region 微信处理
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = GetQueryString("MpID");
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.SendMessage(entity);
            #endregion

            #region 保存数据
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {
            #region 假删除
            var IDs = (Request["ListIDs"] ?? "").Split(',');
            var mpid = GetQueryString("MpID");
            var etys = entities.Set<MpMessage>().Where(c => IDs.Contains(c.ID) && c.IsDelete == 0).ToList();
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            for (int i = 0; i < etys.Count(); i++)
            {
                var entity = etys[i];
                entity.IsDelete = 1;
            }
            entities.SaveChanges();
            return Json(JsonAjaxResult.Successful());
            #endregion
        }
    }
}
