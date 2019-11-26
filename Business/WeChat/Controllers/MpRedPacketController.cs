using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;

namespace WeChat.Controllers
{
    public class MpRedPacketController : BaseController<MpRedPacket>
    {

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpRedPacket>().Where(c => c.MpID == mpid && c.IsDelete != 1).WhereToGridData(qb);
            return Json(result);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            #region 数据校验
            MpRedPacket entity = UpdateEntity<MpRedPacket>();
            if (string.IsNullOrEmpty(entity.Name))
                throw new BusinessException("名称不能为空");
            if (string.IsNullOrEmpty(entity.Token))
                throw new BusinessException("Token不能为空");
            var sameety = entities.Set<MpRedPacket>().Where(c => c.MpID == entity.MpID && c.Token == entity.Token && c.ID != entity.ID && c.IsDelete != 1);
            if (sameety.Count() > 0)
                throw new BusinessException(string.Format("Token和活动{0}重复", sameety.FirstOrDefault().Name));
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
            var entity = GetEntity<MpRedPacket>(ID);
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }
    }
}
