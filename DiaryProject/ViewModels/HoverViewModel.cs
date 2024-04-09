using System.Windows;
using System.Windows.Forms.VisualStyles;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Utils;
using DiaryProject.Utils;
using DiaryProject.Views;
using Application = System.Windows.Application;
using DragEventArgs = System.Windows.DragEventArgs;

namespace DiaryProject.ViewModels;

public class HoverViewModel : BindableBase
{
    private readonly IEventAggregator _aggregator;
    private readonly FileCopyService _fileService;
    private readonly IMemoService _memoService;
    private readonly IMemoLocalRepository _memoRepository;

    private DateTime _targetTime;
    
    public string DateString => _targetTime.ToString("MM/dd");
    public string WeekdayString => _targetTime.GetChineseDayOfWeek();
    public string DayOffset => (new TimeSpan(_targetTime.Ticks - DateTime.Today.Ticks).TotalDays).ToString("+#;-#;0");

    public DelegateCommand NextDay { get; init; }
    public DelegateCommand PreviousDay { get; init; }
    public DelegateCommand Show { get; init; }
    public DelegateCommand ToCurrentDay { get; init; }
    public DelegateCommand OpenHoverSettings { get; init; }
    public DelegateCommand<DragEventArgs> Drop { get; init; }
    public DelegateCommand Exit { get; init; }

    public HoverViewModel(IMemoLocalRepository memoRepository, IMemoService memoService, FileCopyService fileService, IEventAggregator aggregator, IRegionManager regionManager)
    {
        _memoRepository = memoRepository;
        _memoService = memoService;
        _fileService = fileService;
        _aggregator = aggregator;
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
        Drop = new DelegateCommand<DragEventArgs>(arg =>
        {
            if (!arg.Data.GetDataPresent(typeof(string))) return;
            try
            {
                var text = (string?) arg.Data.GetData(typeof(string));
                if (string.IsNullOrEmpty(text)) return;
                AddMemo(text);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        });
        Exit = new DelegateCommand(() => { Application.Current.Shutdown(); });
    }
    
    private async void AddMemo(string content)
    {
        _aggregator.UpdateLoadingStatus(true);
        var t = DateTime.Now;
        if (t is { Hour: 23, Minute: 59 }) t = t.AddMinutes(-1);
#if LOCAL
        var result = await _memoRepository.AddAsync(new MemoDto
        {
            Id = 0,
            Order = position,
            Active = false,
            Category = 1,
            Title = string.Empty,
            Content = string.Empty,
            StartTime = t,
            EndTime = t.AddMinutes(1)
        });
#else
        var memoDto = new MemoDto
        {
            Id = 0,
            Active = _fileService.ReadHoverConfiguration().SetActiveOnAdded,
            Category = _fileService.ReadHoverConfiguration().DefaultMemoCategory + 1,
            Title = content[..5],
            Content = content,
            StartTime = t,
            EndTime = t.AddMinutes(1)
        };
        if (App.IsSynchronizing)
        {
            var response = await _memoService.AddAsync(memoDto);
            App.IsSynchronizing = response.Connected;
        }
        await _memoRepository.AddAsync(memoDto, App.IsSynchronizing);
#endif  
        _aggregator.UpdateLoadingStatus(false);
    }
}