using System;
using KanBan.Model;

namespace KanBan.ViewModel
{
    public class WeeklyTimesheet
    {
        public DateOnly? WeekDate { get; set; }

        public static DateOnly GetWeekStartDate(DateOnly date)
        {
           
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Saturday)) % 7;
            return date.AddDays(-diff); 
        }

        public static DateOnly GetWeekEndDate(DateOnly weekStartDate)
        {
            return weekStartDate.AddDays(6);
        }
    }
}
