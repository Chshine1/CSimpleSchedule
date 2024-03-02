using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Models;

/// <summary>
/// 视图左端日历中日期的模型类
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DateModel : BindableBase
{
    public required string BackgroundColor { get; set; }

    public required string TextColor { get; set; }

    public string DateText { get; set; } = string.Empty;
}