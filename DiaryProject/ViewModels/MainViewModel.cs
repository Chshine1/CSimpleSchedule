using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service.Local;
using DiaryProject.Shared.Utils;
using DiaryProject.Views;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MainViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly IMemoLocalRepository _memoRepository;

    private readonly List<int> _idsInSchedule;

    public string ScheduleText => _idsInSchedule.Count == 0 ? "无进行中日程" : $"{_idsInSchedule.Count}进行中日程";

    public string ScheduleColor => _idsInSchedule.Count == 0 ? "Gray" : "#6FA3FF";

    public FontWeight ScheduleTextWeight => _idsInSchedule.Count == 0 ? FontWeights.Normal : FontWeights.Bold;
    
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            RaisePropertyChanged();
        }
    }

    private MonthModel _calendarTitleModelModel; // Model of the current month of the calendar in the left panel

    public MonthModel CalendarTitleModel
    {
        get => _calendarTitleModelModel;
        private set
        {
            _calendarTitleModelModel = value;
            RaisePropertyChanged();
        }
    }

    private ObservableCollection<DateModel> _dateModels; // Models of the days in the month
    public ObservableCollection<DateModel> DateModels
    {
        get => _dateModels;
        set
        {
            _dateModels = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<MenuItemModel> MenuItemModels { get; }

    public DelegateCommand<MenuItemModel> NavigateCommand { get; private set; }
    public DelegateCommand CalendarNext { get; private set; }
    public DelegateCommand CalendarLast { get; private set; }

    public MainViewModel(IRegionManager regionManager, IEventAggregator aggregator, IMemoLocalRepository memoRepository)
    {
        _regionManager = regionManager;
        _memoRepository = memoRepository;
        _idsInSchedule = new List<int>();
        aggregator.GetEvent<LoadingStatusChanged>().Subscribe(arg =>
        {
            //IsLoading = arg.IsOpen;
        });
        aggregator.GetEvent<EditorNavigationChanged>().Subscribe(arg =>
        {
            MenuItemModels![2].IsPageEnabled = arg.IsEnabled;
        });
        aggregator.GetEvent<TimerStatusChanged>().Subscribe(arg =>
        {
            if (arg.Status) _idsInSchedule.Add(arg.Id);
            else _idsInSchedule.Remove(arg.Id);

            RaisePropertyChanged(nameof(ScheduleText));
            RaisePropertyChanged(nameof(ScheduleColor));
            RaisePropertyChanged(nameof(ScheduleTextWeight));
            if (!arg.SendNotification) return;
            NotifySchedule(arg.Id, arg.Status);
        });
        aggregator.GetEvent<AccountEvent>().Subscribe(arg =>
        {
            Debug.Assert(MenuItemModels != null, nameof(MenuItemModels) + " != null");
            MenuItemModels[0].IsUserRegistered = arg.Operation == UserOperation.SuccessfullyLogin;
            if (arg.Operation == UserOperation.ExitAccount)
            {
                MenuItemModels[0].IsUserRegistered = false;
                MenuItemModels[1].IsPageEnabled = false;
                _regionManager.Regions["MainPanel"].RequestNavigate(nameof(LoginView));
                return;
            }
            _regionManager.Regions["MainPanel"].RequestNavigate(arg.Operation == UserOperation.SuccessfullyLogin ? nameof(UserView) : nameof(CalendarView));
            MenuItemModels[1].IsPageEnabled = true;
        });
        
        NavigateCommand = new DelegateCommand<MenuItemModel>(Navigate);
        CalendarNext = new DelegateCommand(() => { RefreshCalendar(CalendarTitleModel.NextMonth()); });
        CalendarLast = new DelegateCommand(() => { RefreshCalendar(CalendarTitleModel.LastMonth()); });
        
        _dateModels = GetDatesPanels(TimeProcessor.GetMonthCalendar(DateTime.Now));

        MenuItemModels = new ObservableCollection<MenuItemModel>
        {
            new() { Icon = "AccountCircle", TargetName = nameof(LoginView), IsPageEnabled = true, IsAccount = true, ToolTipText = "用户"},
            new() { Icon = "CalendarMonth", TargetName = nameof(CalendarView), IsPageEnabled = false, IsAccount = false, ToolTipText = "日历"},
            new() { Icon = "Notebook", TargetName = nameof(MemoEditorView), IsPageEnabled = false, IsAccount = false, ToolTipText = "编辑日程"}
        };

        _calendarTitleModelModel = new MonthModel(DateTime.Now);
    }

    private void Navigate(MenuItemModel obj)
    {
        _regionManager.Regions["MainPanel"].RequestNavigate(obj.TargetName);
    }
    
    private async void NotifySchedule(int id, bool status)
    {
        var memoDto = (await _memoRepository.GetFirstOrDefaultAsync(id)).Result;
        if (memoDto == null) return;
        var memo = memoDto.Title;
        
        var notifyIcon = new NotifyIcon();
        notifyIcon.Icon = SystemIcons.WinLogo;
        notifyIcon.BalloonTipTitle = memo;
        notifyIcon.BalloonTipText = $"开始时间：{memoDto.StartTime:HH:mm}，截止时间：{memoDto.EndTime:HH:mm}\r\n{(status ? "进行中日程" : "日程结束")}";
        notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
        notifyIcon.Visible = true;
        notifyIcon.ShowBalloonTip(5000);
    }

    private void RefreshCalendar(DateTime dateTime)
    {
        CalendarTitleModel = new MonthModel(dateTime);
        DateModels = GetDatesPanels(TimeProcessor.GetMonthCalendar(dateTime));
    }

    private static ObservableCollection<DateModel> GetDatesPanels(List<DateTime> dates)
    {
        var result = new ObservableCollection<DateModel>();
        var active = false;
        foreach (var date in dates)
        {
            if (date.Day == 1) active = !active;
            var ifNow = date.Date.Equals(DateTime.Today);
            var bcg = ifNow ? "#6fa3ff" : active ? "DarkGray" : "LightGray";
            var txt = ifNow ? "#e6efff" : active ? "#333333" : "Gray";
            result.Add(new DateModel{ DateText = date.Day.ToString(), 
                TextColor = txt, 
                BackgroundColor = bcg});
        }
        return result;
    }
}