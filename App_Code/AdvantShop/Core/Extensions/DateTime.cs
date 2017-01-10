
using System;

namespace AdvantShop
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Is this time between 2 datetimes
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool TimeBetween(this DateTime dateTime, DateTime from, DateTime to)
        {
            var start = new TimeSpan(from.Hour, from.Minute, 0);
            var end = new TimeSpan(to.Hour, to.Minute, 0);
            var now = dateTime.TimeOfDay;

            if (start < end)
                return start <= now && now <= end;

            return !(end < now && now < start);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }
}