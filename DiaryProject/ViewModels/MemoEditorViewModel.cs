#define LOCAL
#undef LOCAL

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Documents;
using AutoMapper;
using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Utils;
using DiaryProject.Utils;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MemoEditorViewModel : NavigationModel
{
    private readonly IMemoService _memoService;
    private readonly IMapper _mapper;
    private readonly IMemoLocalRepository _memoRepository;
    private readonly TimerService _timerService;

    #region BoundProperties

    private DateTime _editedTime;
    public string DateString => TimeProcessor.GetCalendarTitleDate(_editedTime);
    
    private ObservableCollection<EditableMemoModel> _memoModels;
    public ObservableCollection<EditableMemoModel> MemoModels
    {
        get => _memoModels;
        set
        {
            _memoModels = value;
            RaisePropertyChanged();
        }
    }

    public DelegateCommand<EditableMemoModel> ContentUpdateCommand { get; private init; }
    public DelegateCommand<EditableMemoModel> DeleteSpecific { get; private init; }
    public DelegateCommand AddMemoCommand { get; private init; }
    public DelegateCommand<RichTextBox> ConfirmContentInput { get; private init; }

    #endregion

    public MemoEditorViewModel(IEventAggregator aggregator, IMemoService memoService, IMapper mapper, IMemoLocalRepository memoRepository, TimerService timerService) : base(aggregator)
    {
        _memoService = memoService;
        _mapper = mapper;
        _memoRepository = memoRepository;
        _timerService = timerService;
        
        _memoModels = new ObservableCollection<EditableMemoModel>();
        
        Aggregator.GetEvent<EditorUpdated>().Subscribe(arg =>
        {
            var memos = new ObservableCollection<EditableMemoModel>();
            foreach (var memo in arg.Memos)
            {
                memos.Add(new EditableMemoModel(memo));
            }
            MemoModels = memos;
            _editedTime = arg.EditedDate;
            RaisePropertyChanged(nameof(DateString));
        });
        // 改变备忘录的视觉表现状态
        Aggregator.GetEvent<TimerStatusChanged>().Subscribe(arg =>
        {
            var memo = MemoModels.FirstOrDefault(predicate: m => m.Memo.Id.Equals(arg.Id));
            if (memo == null) return;
            memo.Status = arg.Status;
        });
        
        ContentUpdateCommand = new DelegateCommand<EditableMemoModel>(UpdateMemo);
        AddMemoCommand = new DelegateCommand(AddMemo);
        ConfirmContentInput = new DelegateCommand<RichTextBox>(ConfirmInput);
        DeleteSpecific = new DelegateCommand<EditableMemoModel>(Delete);
    }

    #region PrivateMethods

    private async void UpdateMemo(EditableMemoModel memo)
    {
#if LOCAL
        await _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo));
#else
        if (App.IsSynchronizing)
        {
            var response = await _memoService.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo));
            App.IsSynchronizing = response.Connected;
        }
        await _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo), App.IsSynchronizing);
#endif
        _timerService.RegisterToTimers(memo.Memo);
    }

    private async void AddMemo()
    {
        Aggregator.UpdateLoadingStatus(true);
        var t = _editedTime.Date == DateTime.Today ? DateTime.Now : _editedTime;
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
            MemoOrder = _memoModels.Count,
            Active = false,
            Category = 1,
            Title = string.Empty,
            Content = string.Empty,
            StartTime = t,
            EndTime = t.AddMinutes(1)
        };
        if (App.IsSynchronizing)
        {
            var response = await _memoService.AddAsync(memoDto);
            App.IsSynchronizing = response.Connected;
        }
        var result = await _memoRepository.AddAsync(memoDto, App.IsSynchronizing);
#endif        
        var memoRecord = _mapper.Map<MemoRecord>(result.Result);
        _timerService.RegisterToTimers(memoRecord);
        _memoModels.Insert(_memoModels.Count, new EditableMemoModel(memoRecord));
        Aggregator.UpdateLoadingStatus(false);
    }
    
    private async void Delete(EditableMemoModel e)
    {
        if (App.IsSynchronizing)
        {
            var response = await _memoService.DeleteAsync(e.Memo.Id);
            App.IsSynchronizing = response.Connected;
        }
        await _memoRepository.DeleteAsync(e.Memo.Id, App.IsSynchronizing);
        MemoModels.Remove(e);
    }

    private async void ConfirmInput(RichTextBox textBox)
    {
        var content = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
        if (textBox.Document.DataContext is not EditableMemoModel context) return;
        context.Memo.Content = content;
#if LOCAL
            _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(context.Memo));
#else
        if (App.IsSynchronizing)
        {
            var response = await _memoService.UpdateAsync(_mapper.Map<MemoDto>(context.Memo));
            App.IsSynchronizing = response.Connected;
        }
        await _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(context.Memo), !App.IsSynchronizing);
#endif
    }

    #endregion

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);
        Aggregator.NotifyAction(ActionsToNotify.PassToMemoEditor);
    }
}