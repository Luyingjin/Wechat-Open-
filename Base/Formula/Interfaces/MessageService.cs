using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    public class MessageService : IMessageService
    {
        public void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false)
        {
            Config.Logic.MessageService.SendMsg(title, content, link, attachFileID, receiverIDs, receiverNames, receiverType, msgType, isReadReceipt, isImportant);
        }

        public void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, UserInfo sendUser, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false)
        {
            Config.Logic.MessageService.SendMsg(title, content, link, attachFileID, receiverIDs, receiverNames, sendUser, receiverType, msgType, isReadReceipt, isImportant);
        }
    }
}
