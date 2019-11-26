using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Config.Logic
{
    public class CalendarService
    {
        public static float GetWorkHourCount(DateTime start, DateTime end)
        {
            int start1 = 9;//9点上班
            int end1 = 12; //午休
            int start2 = 13;//午休
            int end2 = 18;//6点下班


            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start1"]))
                start1 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start1"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End1"]))
                end1 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End1"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start2"]))
                start2 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start2"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End2"]))
                end2 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End2"]);

            int workHours = 0;
            DateTime time = start;
            while (time < end)
            {
                var date = time.Date;

                var holiday = HolidayTable.AsEnumerable().SingleOrDefault(c => c["Year"].ToString() == date.Year.ToString() && c["Month"].ToString() == date.Month.ToString() && c["Day"].ToString() == date.Day.ToString());

                if (holiday != null && holiday["IsHoliday"].ToString() == "1")
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (date.DayOfWeek == DayOfWeek.Saturday) //周六
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday) //周日
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (time > date.AddHours(end1) && time < date.AddHours(start2)) //中午
                {
                    time = date.AddHours(start2);
                    continue;
                }
                else if (time > date.AddHours(end2)) //夜晚及上班前
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (time < date.AddHours(start1)) //0点到上班前
                {
                    time = date.AddHours(start1);
                    continue;
                }

                time = time.AddHours(1);
                workHours++;
            }
            return workHours;
        }

        public static float GetWorkDayCount(DateTime start, DateTime end)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_C_Holiday where Date >='{0}' and Date<='{1}'", start.Date, end.Date));

            int days = (end - start).Days;
            int workDays = (days / 7) * 5;
            for (int i = 0; i < days % 7; i++)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                    workDays++;
                start = start.AddDays(1);
            }

            workDays = workDays
                - dt.AsEnumerable().Where(c => c["IsHoliday"].ToString() == "1").Count()
                + dt.AsEnumerable().Where(c => c["IsHoliday"].ToString() == "0").Count();
            return workDays;
        }

        public static bool IsWorkDay(DateTime date)
        {
            var row = HolidayTable.AsEnumerable().SingleOrDefault(c => c["Date"].ToString() == date.Date.ToString());
            if (row != null)
                return row["IsHoliday"].ToString() == "0";
            else
                return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        public static DateTime GetTimeoutDate(DateTime start, int workDays)
        {
            DateTime time = start;
            while (workDays > 0)
            {
                var holiday = HolidayTable.AsEnumerable().SingleOrDefault(c => c["Year"].ToString() == time.Year.ToString() && c["Month"].ToString() == time.Month.ToString() && c["Day"].ToString() == time.Day.ToString());

                time = time.AddDays(1);
                if (holiday != null)
                {
                    if (holiday["IsHoliday"].ToString() == "0")
                    {
                        workDays--;
                    }
                }
                else if (time.DayOfWeek >= DayOfWeek.Monday && time.DayOfWeek <= DayOfWeek.Friday) //周一～周五
                {
                    workDays--;
                }
            }
            return time;
        }

        public static DateTime GetTimeoutTime(DateTime start, int workHours)
        {
            int start1 = 9;//9点上班
            int end1 = 12; //午休
            int start2 = 13;//午休
            int end2 = 18;//6点下班

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start1"]))
                start1 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start1"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End1"]))
                end1 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End1"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start2"]))
                start2 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_Start2"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End2"]))
                end2 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WorkTime_End2"]);

            DateTime time = start;
            while (workHours > 0)
            {
                var date = time.Date;

                var holiday = HolidayTable.AsEnumerable().SingleOrDefault(c => c["Year"].ToString() == date.Year.ToString() && c["Month"].ToString() == date.Month.ToString() && c["Day"].ToString() == date.Day.ToString());

                if (holiday != null)
                {
                    if (holiday["IsHoliday"].ToString() == "1")
                    {
                        time = date.AddDays(1).AddHours(start1);
                    }
                    continue;
                }
                else if (date.DayOfWeek == DayOfWeek.Saturday) //周六
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday) //周日
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (time > date.AddHours(end1) && time < date.AddHours(start2)) //中午
                {
                    time = date.AddHours(start2);
                    continue;
                }
                else if (time > date.AddHours(end2)) //夜晚及上班前
                {
                    time = date.AddDays(1).AddHours(start1);
                    continue;
                }
                else if (time < date.AddHours(start1)) //0点到上班前
                {
                    time = date.AddHours(start1);
                    continue;
                }

                time = time.AddHours(1);
                workHours--;
            }
            return time;
        }

        #region HolidayTable

        private static DataTable _holidayTable = null;
        private static DataTable HolidayTable
        {
            get
            {
                if (_holidayTable == null)
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    _holidayTable = sqlHelper.ExecuteDataTable("select * from S_C_Holiday");
                }
                return _holidayTable;
            }
        }

        #endregion

    }
}
