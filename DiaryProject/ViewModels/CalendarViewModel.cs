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
    private readonly IMapper _mapper;
    private readonly IMemoLocalRepository _memoRepository;

    /// <summary>
    /// The ListBox used for exhibiting the memos
    /// </summary>
    private ListBox? _listBox;

    #region BoundProperties

    // ReSharper disable once NullableWarningSuppressionIsUsed
    private MonthModel _currentPage = null!;
    /// <summary>
    /// The month which the calendar is currently located
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
    
    private MemoModel? _selectedMemo;
    public MemoModel? SelectedMemo
    {
        get => _selectedMemo;
        set
        {
            if (_selectedMemo != null) _selectedMemo.IsSelected = false;
            if (value != null)
            {
                _selectedMemo = value; // Keep selection when switching between months
                _selectedMemo.IsSelected = true;
            }
            if (_selectedMemo != null) Aggregator.UpdateEditorStatus(true);
        }
    }
    
    // ReSharper disable once NullableWarningSuppressionIsUsed
    private ObservableCollection<MemoModel> _bigPanels = null!;
    public ObservableCollection<MemoModel> BigPanels
    {
        get => _bigPanels;
        private set
        {
            _bigPanels = value;
            RaisePropertyChanged();
        }
    }
    
    public DelegateCommand<ListBox> SelectCommand { get; private set; }
    public DelegateCommand NextPage { get; private set; }
    public DelegateCommand LastPage { get; private set; }
    public DelegateCommand ExpandEditor { get; private set; }
    public DelegateCommand LocateToToday { get; private set; }
    public DelegateCommand LocateToSelected { get; private set; }
    public DelegateCommand ClearSelected { get; private set; }

    #endregion

    public CalendarViewModel(IMemoService memoService, IEventAggregator aggregator, IMapper mapper, IRegionManager regionManager, IMemoLocalRepository memoRepository, TimerService timerService) : base(aggregator)
    {
        _mapper = mapper;
        _memoRepository = memoRepository;
        
        SelectCommand = new DelegateCommand<ListBox>(m => { _listBox = m; });
        NextPage = new DelegateCommand(() => { RefreshCalendarByMonth(CurrentPage.NextMonth()); });
        LastPage = new DelegateCommand(() => { RefreshCalendarByMonth(CurrentPage.LastMonth()); });
        ExpandEditor = new DelegateCommand(() => { regionManager.Regions["MainPanel"].RequestNavigate("MemoEditorView"); });
        LocateToToday = new DelegateCommand(() => { RefreshCalendarByMonth(DateTime.Today); });
        LocateToSelected = new DelegateCommand(() => { if (_selectedMemo != null) RefreshCalendarByMonth(_selectedMemo.Date); });
        ClearSelected = new DelegateCommand(() =>
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
                _memoRepository.DeleteAsync(id);
                timerService.DropTracing(id);
                if (App.IsSynchronizing) memoService.DeleteAsync(id);
#endif
            }

            _selectedMemo?.Clear();
        });
        
        // Called when the editor tells this to send the current selected day for editing
        Aggregator.GetEvent<ActionNotified>().Subscribe(n =>
        {
            if (n.ActionToNotify != ActionsToNotify.PassToMemoEditor || _selectedMemo == null) return;
            Aggregator.PassToEditor(_selectedMemo.GetMemos(), _selectedMemo.Date);
        });
        
        // The user can't use the editor at the beginning since there is no selected date
        Aggregator.UpdateEditorStatus(false);
        RefreshCalendarByMonth(DateTime.Now);
    }

    #region PrivateMethods

    /// <summary>
    /// Switch to one month and refresh the contents in the calendar 
    /// </summary>
    /// <param name="month">Any day in the month</param>
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
    /// Get the models of the calendar
    /// </summary>
    private async Task<ObservableCollection<MemoModel>> GetDatesPanels(DateTime date)
    {
        var dates = TimeProcessor.GetMonthCalendar(date);
        var result = new ObservableCollection<MemoModel>();
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
                result.Add(new MemoModel(active, d, group, _mapper));
            }
            return result;
        }
        foreach (var d in dates)
        {
            if (d.Day == 1) active = !active;
            result.Add(new MemoModel(active, d, null, _mapper));
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