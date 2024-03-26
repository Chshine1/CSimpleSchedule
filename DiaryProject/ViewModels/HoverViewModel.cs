using System.Windows;
using DiaryProject.Shared.Utils;
using DiaryProject.Views;
using Application = System.Windows.Application;

namespace DiaryProject.ViewModels;

public class HoverViewModel : BindableBase
{
    private DateTime _targetTime;
    
    public string DateString => _targetTime.ToString("MM/dd");
    public string WeekdayString => _targetTime.GetChineseDayOfWeek();

    public DelegateCommand NextDay { get; init; }
    public DelegateCommand PreviousDay { get; init; }
    public DelegateCommand Show { get; init; }
    public DelegateCommand ToCurrentDay { get; init; }
    public DelegateCommand OpenHoverSettings { get; init; }
    public DelegateCommand Exit { get; init; }

    public HoverViewModel(IRegionManager regionManager)
    {
        _targetTime = DateTime.Now;
        NextDay = new DelegateCommand(() =>
        {
            _targetTime = _targetTime.AddDays(1);
            RaisePropertyChanged(nameof(DateString));
            RaisePropertyChanged(nameof(WeekdayString));
        });
        PreviousDay = new DelegateCommand(() =>
        {
            _targetTime = _targetTime.AddDays(-1);
            RaisePropertyChanged(nameof(DateString));
            RaisePropertyChanged(nameof(WeekdayString));
        });
        Show = new DelegateCommand(() =>
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        });
        ToCurrentDay = new DelegateCommand(() =>
        {
            _targetTime = DateTime.Now;
            RaisePropertyChanged(nameof(DateString));
            RaisePropertyChanged(nameof(WeekdayString));
        });
        OpenHoverSettings = new DelegateCommand(() =>
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            regionManager.Regions["MainPanel"].RequestNavigate(nameof(HoverSettingsView));
        });
        Exit = new DelegateCommand(() => { Application.Current.Shutdown(); });
    }
}