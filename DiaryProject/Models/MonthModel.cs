using System.Diagnostics.CodeAnalysis;
using DiaryProject.Shared.Utils;

namespace DiaryProject.Models;

/// <summary>
/// Model representing one month
/// </summary>
/// <param name="dateTime">Any day in the month</param>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MonthModel(DateTime dateTime) : BindableBase
{
    /// <summary>
    /// The name of the month in the specific calendar title format
    /// </summary>
    public string MonthString => TimeProcessor.GetCalendarTitleMonth(dateTime);

    public DateTime NextMonth() => dateTime.AddMonths(1);
    
    public DateTime LastMonth() => dateTime.AddMonths(-1);
}