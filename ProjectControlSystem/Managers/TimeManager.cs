using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ProjectControlSystem.Managers
{
    public static class TimeManager
    {
        #region  Поля
        static int day = 0;

        // каждую минуту сравниваем время
        readonly static Timer timerDayCheck = new Timer(60000);
        #endregion

        static TimeManager()
        {
            timerDayCheck.Elapsed += timerDayCheck_Elapsed;
            timerDayCheck.Start();
        }

        #region Обрабочики событий
        static void timerDayCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (day != DateTime.Now.Day)
            {
                day = DateTime.Now.Day;
                DoDayChanged();
            }
        }
        #endregion

        public static DateTime AddWorkDays(DateTime date, uint days)
        {
            if (date == DateTime.MinValue)
                return date;

            // Вычисляем дату окончания days суток
            for (int idx = 0; idx < days; )
            {
                date = date.AddDays(1);
                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                    idx++;
            }

            return date;
        }
        public static DateTime RemoveWorkDays(DateTime date, uint days)
        {
            if (date == DateTime.MinValue)
                return date;

            // Вычисляем дату окончания days суток
            for (int idx = 0; idx < days; )
            {
                date = date.AddDays(-1);
                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                    idx++;
            }

            return date;
        }

        /// <summary>Возвращет количество рабочих дней между датами</summary>
        public static int WorkDaysCount(DateTime sTime, DateTime eTime)
        {
            DateTime s = sTime.Date.AddDays(1), e = eTime.Date;
            if (eTime.Date < sTime.Date)
            {
                // сегодняшний день не считается за день
                s = eTime.Date.AddDays(1);
                e = sTime.Date;
            }


            int days = 0;
            for (; s <= e; s = s.AddDays(1))
            {
                if (s.DayOfWeek == DayOfWeek.Sunday || s.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                days++;
            }

            return days;

        }

        #region События
        public static event EventHandler DayChanged;
        static void DoDayChanged()
        {
            if (DayChanged != null)
                DayChanged(null, null);
        }
        #endregion
    }
}
