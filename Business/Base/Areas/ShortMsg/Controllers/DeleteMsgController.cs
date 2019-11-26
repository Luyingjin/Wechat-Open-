using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using Formula;

namespace Base.Areas.ShortMsg.Controllers
{
    public class DeleteMsgController : BaseController
    {
        //
        // GET: /ShortMsg/DeleteMsg/

        public JsonResult GetList(MvcAdapter.QueryBuilder qb, string attachment)
        {
            string sql = "";
            if (Config.Constant.IsOracleDb)
            {
                string whereAttach = attachment == "T" ? " and nvl(AttachFileIDs,'') <> '' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgBody.TYPE,S_S_MsgBody.Title,substr(nvl(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgReceiver.DeleteTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='1' {1}
", FormulaHelper.UserID, whereAttach);
            }
            else
            {
                string whereAttach = attachment == "T" ? " and isnull(AttachFileIDs,'') <> '' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgBody.[Type],S_S_MsgBody.Title,substring(isnull(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgReceiver.DeleteTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='1' {1}
", FormulaHelper.UserID, whereAttach);
            }
            return Json(SQLHelper.CreateSqlHelper("Base").ExecuteGridData(sql, qb));
        }

        public JsonResult Delete(string ids)
        {
            string[] arr = ids.Split(',');
            entities.Set<S_S_MsgReceiver>().Delete(c => ids.Contains(c.ID));
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult DeleteMsg(string ids)
        {
            string[] arrIds = ids.Split(',');
            List<S_S_MsgReceiver> list = entities.Set<S_S_MsgReceiver>().Where(c => arrIds.Contains(c.ID)).ToList();
            foreach (S_S_MsgReceiver item in list)
            {
                item.IsDeleted = "1";
                item.DeleteTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json("");
        }

    }
}
