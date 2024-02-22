using DiaryProject.Shared.Utils;

namespace DiaryProject.Models;

/// <summary>
/// Model representing one month
/// </summary>
/// <param name="dateTime">Any day in the month</param>
public class MonthModel(DateTime dateTime) : BindableBase
{
    /// <summary>
    /// The name of the month in the specific calendar title format
    /// </summary>
    public string MonthString { get; set; } = TimeProcessor.GetCalendarTitleMonth(dateTime);

    private DateTime DateTime { get; } = dateTime;

    public DateTime NextMonth() => DateTime.AddMonths(1);
    
    public DateTime LastMonth() => DateTime.AddMonths(-1);
}