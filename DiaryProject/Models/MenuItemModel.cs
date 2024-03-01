using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DiaryProject.Views;

namespace DiaryProject.Models;

/// <summary>
/// Model for page selection located in the left bar
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MenuItemModel : INotifyPropertyChanged
{
    private bool _isPageEnabled;
    private bool _isUserRegistered;

    #region Properties

    public required string Icon { get; set; }

    public required string TargetName { get; set; }

    public bool IsAccount { get; init; }

    public required string ToolTipText { get; init; }

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

    public bool IsUserRegistered
    {
        get => _isUserRegistered;
        set
        {
            _isUserRegistered = value;
            TargetName = _isUserRegistered ? nameof(UserView) : nameof(LoginView);
            OnPropertyChanged(nameof(IconColor));
        }
    }
    
    public string IconColor => IsAccount ? IsUserRegistered ? "#7BD672" : "#E16464" : IsPageEnabled ? "DimGray" : "#AAAAAA";

    #endregion
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}