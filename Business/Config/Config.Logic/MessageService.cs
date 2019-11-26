using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config.Logic
{
    public  class MessageService
    {


        public static void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false)
        {
            var sendUser = UserService.GetUserInfoBySysName(UserService.GetCurrentUserLoginName());
            SendMsg(title, content, link, attachFileID, receiverIDs, receiverNames, sendUser, receiverType, msgType, isReadReceipt, isImportant);
        }

        public static void SendMsg(string title, string content, string link, string attachFileID, string receiverIDs, string receiverNames, UserInfo sendUser, MsgReceiverType receiverType = MsgReceiverType.UserType, MsgType msgType = MsgType.Normal, bool isReadReceipt = false, bool isImportant = false)
        {
            title = title.Replace("'", "''");
            content = content.Replace("'", "''");

            string msgBodyID = GuidHelper.CreateGuid();

            string userID = "";
            string userName = "系统";

            if (sendUser != null)
            {
                userID = sendUser.UserID;
                userName = sendUser.UserName;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msgBodySql
                , msgBodyID
                , title
                , content
                , attachFileID
                , link
                , 1
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                , userID
                , userName
                , receiverIDs
                , receiverNames
                , (isReadReceipt ? "1" : "0")
                , (isImportant ? "1" : "0"));

            var receiverIDArr = receiverIDs.Split(',');
            var receiverNameArr = receiverNames.Split(',');
            for (int i = 0; i < receiverIDArr.Length; i++)
            {
                sb.AppendFormat(msgReceiverSql
                    , GuidHelper.CreateGuid()
                    , msgBodyID
                    , receiverIDArr[i]
                    , receiverNameArr[i]
                    , 0);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            if (Config.Constant.IsOracleDb)
            {
                string sql = sb.ToString();
                sql = sql.Replace("\r\n", "").Replace("\n", "");
                sql = "begin " + sql + " end;";
                sqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                sqlHelper.ExecuteNonQuery(sb.ToString());
            }
        }

        static string msgBodySql
        {
            get
            {
                string sql = @"
  INSERT INTO S_S_MsgBody
           (ID,Title,Content,AttachFileIDs,LinkUrl,IsSystemMsg,SendTime,SenderID,SenderName,ReceiverIDs,ReceiverNames,IsReadReceipt,Importance)
     VALUES
           ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');";

                if (Config.Constant.IsOracleDb)
                    sql = @"
 INSERT INTO S_S_MsgBody
           (ID,Title,Content,AttachFileIDs,LinkUrl,IsSystemMsg,SendTime,SenderID,SenderName,ReceiverIDs,ReceiverNames,IsReadReceipt,Importance)
     VALUES
           ('{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'),'{7}','{8}','{9}','{10}','{11}','{12}');
";

                return sql;
            }
        }
           

        static string msgReceiverSql= @"
  INSERT INTO S_S_MsgReceiver(ID,MsgBodyID,UserID,UserName,IsDeleted)
     VALUES ('{0}','{1}','{2}','{3}','{4}');
 ";
    }
}
