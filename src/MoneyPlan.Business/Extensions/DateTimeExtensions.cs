namespace MoneyPlan.Business
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        /// <summary>
        /// Produces the next working day - if the current date isn't already a working day.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetWorkingDay(this DateTime dateTime)
        {
            List<DateTime> bankHolidays = new List<DateTime>();
            bankHolidays.Add(new DateTime(2023, 01, 01));

            bool keepdoing = true;
            while (keepdoing)
            {
                if (dateTime.DayOfWeek == DayOfWeek.Saturday ||
                     dateTime.DayOfWeek == DayOfWeek.Sunday ||
                     bankHolidays.Any(x => x.IsSameMonthAndDay(dateTime)))
                {
                    dateTime = dateTime.AddDays(1);
                }
                else
                {
                    keepdoing = false;
                }
            }
            return dateTime;
        }

        public static bool IsSameMonthAndDay(this DateTime dateTime, DateTime compareDateTime)
        {
            return (dateTime.Month == compareDateTime.Month && dateTime.Day == compareDateTime.Day);
        }
    }
}
