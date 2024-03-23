using DiaryProject.Shared.Utils;

namespace DiaryProject.ViewModels;

public class HoverViewModel : BindableBase
{
    private DateTime _targetTime;
    
    public string DateString => _targetTime.ToString("MM/dd");
    public string WeekdayString => _targetTime.GetChineseDayOfWeek();

    public DelegateCommand NextDay { get; init; }
    public DelegateCommand PreviousDay { get; init; }

    public HoverViewModel()
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
    }
}