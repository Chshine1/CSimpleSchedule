using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DiaryProject.Models;

/// <summary>
/// Model for page selection located in the left bar
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MenuItemModel : INotifyPropertyChanged
{
    public string Icon { get; set; } = null!;

    public string TargetName { get; init; } = null!;

    public bool IsAccount { get; init; }

    private bool _isPageEnabled;
    public bool IsPageEnabled
    {
        get => _isPageEnabled;
        set
        {
            _isPageEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IconColor));
        }
    }

    private bool _isUserRegistered;

    public bool IsUserRegistered
    {
        get => _isUserRegistered;
        set
        {
            _isUserRegistered = value;
            OnPropertyChanged(nameof(IconColor));
        }
    }

    public string IconColor => IsAccount ? IsUserRegistered ? "#7BD672" : "#E16464" : IsPageEnabled ? "DimGray" : "#AAAAAA";
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}