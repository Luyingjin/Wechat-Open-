using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config.Logic
{
    public class AlarmService
    {
        /// <summary>
        /// 发送提醒
        /// </summary>
        /// <param name="alarmTitle">提醒标题</param>
        /// <param name="alarmContent">正文内容</param>
        /// <param name="alarmUrl">事务地址</param>
        /// <param name="ownerName">提醒人</param>
        /// <param name="ownerID">提醒人ID</param>
        /// <param name="deadlineTime">提醒截止时间（过期日期）</param>
        /// <param name="alarmType">提醒模块名，比如经营提醒、OA提醒等等文本</param>
        /// <param name="sendUser">发送人</param>
        /// <param name="isImportant">是否重要，默认否.</param>
        /// <param name="isUrgency">是否紧急,默认否.</param>
        public static void SendAlarm(string alarmTitle, string alarmContent, string alarmUrl, string ownerName, string ownerID, DateTime deadlineTime, string alarmType, UserInfo sendUser, bool isImportant = false, bool isUrgency = false)
        {
            alarmTitle = alarmTitle.Replace("'", "''");
            alarmContent = alarmContent.Replace("'", "''");

            string alarmID = GuidHelper.CreateGuid();

            string userID = "";
            string userName = "系统";

            if (sendUser != null)
            {
                userID = sendUser.UserID;
                userName = sendUser.UserName;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(alarmSql,
               alarmID,
               isImportant ? "1" : "0",
               isUrgency ? "1" : "0",
               alarmType,
               alarmTitle,
               alarmContent,
               alarmUrl,
               ownerName,
               ownerID,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            deadlineTime.ToString("yyyy-MM-dd HH:mm:ss"),
            userName,
            userID);

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
        static string alarmSql
        {
            get
            {
                string sql = @" INSERT INTO S_S_Alarm(ID, Important, Urgency, AlarmType, AlarmTitle, AlarmContent, AlarmUrl, OwnerName, OwnerID, SendTime, DeadlineTime, SenderName, SenderID)
VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');";
                return sql;
            }
        }
    }
}
