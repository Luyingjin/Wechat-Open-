using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using MvcAdapter;
using Formula.Exceptions;

namespace Base.Areas.ShortMsg.Controllers
{
    public class SendMsgController : BaseController<S_S_MsgBody>
    {
        public JsonResult GetSendList(MvcAdapter.QueryBuilder qb, string attachment)
        {
            string userID = FormulaHelper.UserID;
            IQueryable<S_S_MsgBody> query = entities.Set<S_S_MsgBody>().Where(c => c.SenderID == userID && c.IsDeleted == "0");
            if (attachment == "T")
                query = query.Where(c => !string.IsNullOrEmpty(c.AttachFileIDs));
            return Json(query.WhereToGridData(qb));
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var msg = UpdateEntity<S_S_MsgBody>();

            var user = FormulaHelper.GetUserInfo();
            msg.SenderID = user.UserID;
            msg.SenderName = user.UserName;
            msg.SendTime = DateTime.Now;
            if (msg.Content.Length > 2000)
                throw new BusinessException("消息长度不能超过2000个字符!");

            List<S_S_MsgReceiver> listReceiver = new List<S_S_MsgReceiver>();
            if (!string.IsNullOrEmpty(msg.ReceiverIDs) && !string.IsNullOrEmpty(msg.ReceiverNames))
            {
                string[] receiverIDs = msg.ReceiverIDs.Split(',');
                string[] receiverNames = msg.ReceiverNames.Split(',');

                for (int i = 0; i < receiverIDs.Length; i++)
                {
                    S_S_MsgReceiver receiver = new S_S_MsgReceiver();
                    receiver.ID = FormulaHelper.CreateGuid();
                    receiver.MsgBodyID = msg.ID;
                    receiver.UserID = receiverIDs[i];
                    receiver.UserName = receiverNames[i];
                    listReceiver.Add(receiver);
                }
            }
            if (!string.IsNullOrEmpty(msg.ReceiverDeptIDs))
            {
                string[] arrDeptID = msg.ReceiverDeptIDs.Split(',');
                List<S_A_User> list = base.DataBaseFilter<S_A_User>(entities.Set<S_A__OrgUser>().Where(c => arrDeptID.Contains(c.OrgID)).Select(c => c.S_A_User)).Distinct().ToList();
                foreach (S_A_User item in list)
                {
                    S_S_MsgReceiver receiver = listReceiver.Find(c => c.UserID == item.ID);
                    if (receiver == null)
                    {
                        receiver = new S_S_MsgReceiver();
                        receiver.ID = FormulaHelper.CreateGuid();
                        receiver.MsgBodyID = msg.ID;
                        receiver.UserID = item.ID;
                        receiver.UserName = item.Name;
                        listReceiver.Add(receiver);
                    }
                }
            }
            foreach (S_S_MsgReceiver item in listReceiver)
            {
                entities.Set<S_S_MsgReceiver>().Add(item);
            }

            return base.JsonSave<S_S_MsgBody>(msg);
        }

        public JsonResult DeleteMsgBody(string ids)
        {
            string[] arr = ids.Split(',');
            List<S_S_MsgBody> list = entities.Set<S_S_MsgBody>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (S_S_MsgBody item in list)
            {
                item.IsDeleted = "1";
                item.DeleteTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }
    }
}
