using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Models;

/// <summary>
/// Model for the calendar located in the left bar
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DateModel : BindableBase
{
    public required string BackgroundColor { get; set; }

    public required string TextColor { get; set; }

    public string DateText { get; set; } = string.Empty;
}