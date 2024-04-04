using System.Windows;
using System.Windows.Forms.VisualStyles;
using DiaryProject.Shared.Utils;
using DiaryProject.Views;
using Application = System.Windows.Application;

namespace DiaryProject.ViewModels;

public class HoverViewModel : BindableBase
{
    private DateTime _targetTime;
    
    public string DateString => _targetTime.ToString("MM/dd");
    public string WeekdayString => _targetTime.GetChineseDayOfWeek();
    public string DayOffset => (new TimeSpan(_targetTime.Ticks - DateTime.Today.Ticks).TotalDays).ToString("+#;-#;0");

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
            RaisePropertyChanged(nameof(DayOffset));
        });
        PreviousDay = new DelegateCommand(() =>
        {
            _targetTime = _targetTime.AddDays(-1);
            RaisePropertyChanged(nameof(DateString));
            RaisePropertyChanged(nameof(WeekdayString));
            RaisePropertyChanged(nameof(DayOffset));
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
            RaisePropertyChanged(nameof(DayOffset));
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