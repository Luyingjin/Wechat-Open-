using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula
{
    public class CalendarService : Formula.ICalendarService
    {
        public float GetWorkHourCount(DateTime start, DateTime end)
        {
            return Config.Logic.CalendarService.GetWorkHourCount(start, end);
        }

        public float GetWorkDayCount(DateTime start, DateTime end)
        {
            return Config.Logic.CalendarService.GetWorkDayCount(start, end);
        }

        public bool IsWorkDay(DateTime date)
        {
            return Config.Logic.CalendarService.IsWorkDay(date);
        }

        public DateTime GetTimeoutDate(DateTime start, int workDays)
        {
            return Config.Logic.CalendarService.GetTimeoutDate(start, workDays);
        }

        public DateTime GetTimeoutTime(DateTime start, int workHours)
        {
            return Config.Logic.CalendarService.GetTimeoutTime(start, workHours);
        }
    }
}
