using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;

namespace WeChat.Controllers
{
    public class MpEventController : BaseController<MpEvent>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpEvent>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult GetModel(string id)
        {
            var eventtype = Request["EventType"];
            if (eventtype == "AutoReply" || eventtype == "Subscribe")
            {
                var mpid = GetQueryString("MpID");
                var entity = entities.Set<MpEvent>().Where(c => c.MpID == mpid && c.EventType == eventtype).FirstOrDefault();
                if (entity != null)
                    return Json(entity);
            }
            return base.GetModel(id);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            #region 数据校验
            var eventtype = Request["EventType"];
            var eid = "";
            if (eventtype == "AutoReply" || eventtype == "Subscribe")
            {
                var mpid = GetQueryString("MpID");
                var e = entities.Set<MpEvent>().Where(c => c.MpID == mpid && c.EventType == eventtype).FirstOrDefault();
                if (e != null)
                    eid = e.ID;
            }

            MpEvent entity = UpdateEntity<MpEvent>(eid);
            //if (string.IsNullOrEmpty(entity.KeyWord))
            //    throw new BusinessException("关键字不能为空");
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

            #region 微信处理
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpEvent>(ID);
            #endregion

            #region 假删除
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }
    }
}
