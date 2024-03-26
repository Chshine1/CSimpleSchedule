#define LOCAL
#undef LOCAL

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Utils;
using DiaryProject.Utils;
using ListBox = System.Windows.Controls.ListBox;

namespace DiaryProject.ViewModels;
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CalendarViewModel : NavigationModel
{
    private readonly IMemoService _memoService;
    private readonly IMapper _mapper;
    private readonly IMemoLocalRepository _memoRepository;
    private readonly TimerService _timerService;

    /// <summary>
    /// 用于排列日程预览的容器
    /// </summary>
    private ListBox? _listBox;

    #region BoundProperties

    // ReSharper disable once NullableWarningSuppressionIsUsed
    private MonthModel _currentPage = null!;
    /// <summary>
    /// 日历当前位于的月份
    /// </summary>
    public MonthModel CurrentPage 
    {
        get => _currentPage;
        private set
        {
            _currentPage = value;
            RaisePropertyChanged();
        }
    }
    
    private DatePreviewModel? _selectedMemo;
    public DatePreviewModel? SelectedMemo
    {
        get => _selectedMemo;
        set
        {
            if (_selectedMemo != null) _selectedMemo.IsSelected = false;
            if (value != null)
            {
                _selectedMemo = value; // 当日历切换月份时，保留当前选择项
                _selectedMemo.IsSelected = true;
            }
            if (_selectedMemo != null) Aggregator.UpdateEditorStatus(true);
        }
    }
    
    // ReSharper disable once NullableWarningSuppressionIsUsed
    private ObservableCollection<DatePreviewModel> _bigPanels = null!;
    public ObservableCollection<DatePreviewModel> BigPanels
    {
        get => _bigPanels;
        private set
        {
            _bigPanels = value;
            RaisePropertyChanged();
        }
    }
    
    public DelegateCommand<ListBox> SelectCommand { get; private set; }
    public DelegateCommand Refresh { get; private set; }
    public DelegateCommand NextPage { get; private set; }
    public DelegateCommand LastPage { get; private set; }
    public DelegateCommand ExpandEditor { get; private set; }
    public DelegateCommand LocateToToday { get; private set; }
    public DelegateCommand LocateToSelected { get; private set; }
    public DelegateCommand ClearSelected { get; private set; }
    public DelegateCommand SwitchHover { get; private set; }

    #endregion

    public CalendarViewModel(IMemoService memoService, IEventAggregator aggregator, IMapper mapper, IRegionManager regionManager, IMemoLocalRepository memoRepository, TimerService timerService) : base(aggregator)
    {
        _memoService = memoService;
        _mapper = mapper;
        _memoRepository = memoRepository;
        _timerService = timerService;
        
        SelectCommand = new DelegateCommand<ListBox>(m => { _listBox = m; });
        Refresh = new DelegateCommand(() => { RefreshCalendarByMonth(CurrentPage.Current());});
        NextPage = new DelegateCommand(() => { RefreshCalendarByMonth(CurrentPage.NextMonth()); });
        LastPage = new DelegateCommand(() => { RefreshCalendarByMonth(CurrentPage.LastMonth()); });
        ExpandEditor = new DelegateCommand(() => { regionManager.Regions["MainPanel"].RequestNavigate("MemoEditorView"); });
        LocateToToday = new DelegateCommand(() => { RefreshCalendarByMonth(DateTime.Today); });
        LocateToSelected = new DelegateCommand(() => { if (_selectedMemo != null) RefreshCalendarByMonth(_selectedMemo.Date); });
        ClearSelected = new DelegateCommand(Clear);
        SwitchHover = new DelegateCommand(() => { Aggregator.GetEvent<HoverStatusChanged>().Publish(new HoverStatusModel { IsVisible = HoverVisibility.Reverse }); });
        
        // 编辑器通知时调用，将选中项送到编辑器以进行编辑
        Aggregator.GetEvent<ActionNotified>().Subscribe(n =>
        {
            if (n != ActionsToNotify.PassToMemoEditor || _selectedMemo == null) return;
            Aggregator.PassToEditor(_selectedMemo.GetMemos(), _selectedMemo.Date);
        });
        
        // 设置用户最初无法使用编辑器，因为此时还没有选中项
        Aggregator.UpdateEditorStatus(false);
        RefreshCalendarByMonth(DateTime.Now);
    }

    #region PrivateMethods

    private async void Clear()
    {
        if (_selectedMemo == null) return;
            
        /* TODO:popup confirm dialog */
        var memos = _selectedMemo.GetMemoIndices();
        foreach (var id in memos)
        {
#if LOCAL
                _memoRepository.DeleteAsync(id);
                _timerService.DropTracing(id);
#else
            if (App.IsSynchronizing)
            {
                var response = await _memoService.DeleteAsync(id);
                App.IsSynchronizing = response.Connected;
            }
            await _memoRepository.DeleteAsync(id, App.IsSynchronizing);
            _timerService.DropTracing(id);
#endif
        }
        _selectedMemo?.Clear();
    }

    /// <summary>
    /// 将切换到一个月份并且刷新日历显示
    /// </summary>
    /// <param name="month">该月份中的任意一天</param>
    private async void RefreshCalendarByMonth(DateTime month)
    {
        Aggregator.UpdateLoadingStatus(true);
        CurrentPage = new MonthModel(month);
        BigPanels = await GetDatesPanels(month);
        if (_selectedMemo == null || _selectedMemo.Date.Month != month.Month || _selectedMemo.Date.Year != month.Year)
        {
            Aggregator.UpdateLoadingStatus(false);
            return;
        }
        var listBoxSelectedItem = BigPanels.FirstOrDefault(predicate: m => m.Date.Day == _selectedMemo.Date.Day && m.Date.Month == _selectedMemo.Date.Month);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        _listBox!.SelectedItem = listBoxSelectedItem;
        _selectedMemo = listBoxSelectedItem;
        Aggregator.UpdateLoadingStatus(false);
    }
    
    /// <summary>
    /// 获得日历预览使用的模型
    /// </summary>
    private async Task<ObservableCollection<DatePreviewModel>> GetDatesPanels(DateTime date)
    {
        var dates = TimeProcessor.GetMonthCalendar(date);
        var result = new ObservableCollection<DatePreviewModel>();
        var active = false;

#if LOCAL
        var query = await _memoRepository.GetAllAsync();
#else
        var query = await _memoRepository.GetAllAsync();
        //var query = await _memoService.GetAllAsync(new MemoParameter());
#endif
        if (query.Result != null)
        {
            var groupedMemos = (from m in query.Result group m by m.StartTime.Month * 100 + m.StartTime.Day).ToArray();
            foreach (var d in dates)
            {
                if (d.Day == 1) active = !active;
                var group = groupedMemos.FirstOrDefault(g => g.Key == d.Month * 100 + d.Day);
                result.Add(new DatePreviewModel(active, d, group, _mapper));
            }
            return result;
        }
        foreach (var d in dates)
        {
            if (d.Day == 1) active = !active;
            result.Add(new DatePreviewModel(active, d, null, _mapper));
        }
        return result;
    }

    #endregion

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);
        RefreshCalendarByMonth(_selectedMemo?.Date ?? DateTime.Now);
    }
}