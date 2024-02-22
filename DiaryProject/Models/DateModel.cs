using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Models;

/// <summary>
/// Model for the calendar located in the left bar
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DateModel : BindableBase
{
    public string BackgroundColor { get; set; } = null!;

    public string TextColor { get; set; } = null!;

    public string DateText { get; set; } = null!;
}