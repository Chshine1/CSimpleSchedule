namespace DiaryProject.Shared.Utils;

public static class TimeProcessor
{
    private static readonly string[] WeekDayNames = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];

    private static readonly string[] MonthNames = ["一","二","三","四","五","六","七","八","九","十","十一","十二"];
    
    /// <summary> Get current day of week in Chinese </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string GetChineseDayOfWeek(this DateTime dateTime)
    {
        return WeekDayNames[Convert.ToInt32(dateTime.DayOfWeek)];
    }
    
    /// <summary>
    /// Get the first day (sunday) of the given week
    /// </summary>
    /// <param name="dateTime">Any day in the week</param>
    /// <returns>The datetime of the first day</returns>
    private static DateTime GetFirstDayOfWeek(this DateTime dateTime)
    {
        var t = dateTime.DayOfWeek == 0 ? 6 : (int)dateTime.DayOfWeek + 1;
        return dateTime.AddDays(-t);
    }

    /// <summary>
    /// Get all days in the given month, starting from Sunday
    /// </summary>
    /// <param name="month">Any day in the month</param>
    /// <returns></returns>
    public static List<DateTime> GetMonthCalendar(DateTime month)
    {
        var result = new List<DateTime> { Capacity = 42 };
        var m = new DateTime(month.Year, month.Month, 1);
        var start = m.GetFirstDayOfWeek();
        for (var i = 0; i < 42; i++)
        {
            result.Add(start.AddDays(1));
            start = start.AddDays(1);
        }
        return result;
    }

    /// <summary>
    /// The name of the month in the specific format for calendar titles
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string GetCalendarTitleMonth(DateTime dateTime)
    {
        return $"{dateTime.Year}年  {MonthNames[dateTime.Month - 1]}月";
    }

    public static string GetCalendarTitleDate(DateTime dateTime)
    {
        return $"{dateTime.Year}年 {dateTime.Month}月{dateTime.Day}日";
    }
}