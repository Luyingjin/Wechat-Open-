using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    public class AlarmService : IAlarmService
    {
        public void SendAlarm(string alarmTitle, string alarmContent, string alarmUrl, string ownerName, string ownerID, DateTime deadlineTime, string alarmType, UserInfo sendUser, bool isImportant = false, bool isUrgency = false)
        {
            Config.Logic.AlarmService.SendAlarm(alarmTitle, alarmContent, alarmUrl, ownerName, ownerID, deadlineTime, alarmType, sendUser, isImportant, isUrgency);
        }
    }
}
