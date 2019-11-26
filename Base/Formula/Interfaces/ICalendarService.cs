using System;
namespace Formula
{
    public interface ICalendarService
    {
        float GetWorkHourCount(DateTime start, DateTime end);
        DateTime GetTimeoutDate(DateTime start, int workDays);
        DateTime GetTimeoutTime(DateTime start, int workHours);
        float GetWorkDayCount(DateTime start, DateTime end);
        bool IsWorkDay(DateTime date);
    }
}
