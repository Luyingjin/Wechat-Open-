using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    /// <summary>
    /// 提醒服务接口
    /// </summary>
    public interface IAlarmService : ISingleton
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
        void SendAlarm(string alarmTitle, string alarmContent, string alarmUrl, string ownerName, string ownerID, DateTime deadlineTime, string alarmType, UserInfo sendUser, bool isImportant = false, bool isUrgency = false);
    }
}
