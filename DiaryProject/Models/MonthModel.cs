using System.Diagnostics.CodeAnalysis;
using DiaryProject.Shared.Utils;

namespace DiaryProject.Models;

/// <summary>
/// 单个月份的模型类
/// </summary>
/// <param name="dateTime">该月份中的任意一天</param>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MonthModel(DateTime dateTime) : BindableBase
{
    public string MonthString => TimeProcessor.GetCalendarTitleMonth(dateTime);

    public DateTime NextMonth() => dateTime.AddMonths(1);
    
    public DateTime LastMonth() => dateTime.AddMonths(-1);
}