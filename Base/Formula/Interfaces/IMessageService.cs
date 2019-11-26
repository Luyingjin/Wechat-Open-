using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    /// <summary>
    /// 消息服务类
    /// </summary>
    public interface IMessageService : ISingleton
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="link"></param>
        /// <param name="attachFileID"></param>
        /// <param name="receiverIDs"></param>
        /// <param name="receiverNames"></param>
        /// <param name="receiverType"></param>
        /// <param name="msgType"></param>
        void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="link"></param>
        /// <param name="attachFileID"></param>
        /// <param name="receiverIDs"></param>
        /// <param name="receiverNames"></param>
        /// <param name="sendUser"></param>
        /// <param name="receiverType"></param>
        /// <param name="msgType"></param>
        /// <param name="isReadReceipt"></param>
        void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, UserInfo sendUser, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false);
    }

}
