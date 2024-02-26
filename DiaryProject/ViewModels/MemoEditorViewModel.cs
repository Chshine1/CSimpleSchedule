#define LOCAL
#undef LOCAL

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
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
public class MemoEditorViewModel : NavigationModel
{
    private readonly IMemoService _memoService;
    private readonly IMapper _mapper;
    private readonly IMemoLocalRepository _memoRepository;
    private readonly TimerService _timerService;

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

    public DelegateCommand<EditableMemoModel> ContentUpdateCommand { get; set; }
    public DelegateCommand<EditableMemoModel> DeleteSpecific { get; set; }
    public DelegateCommand AddMemoCommand { get; set; }
    public DelegateCommand<RichTextBox> ConfirmContentInput { get; set; }

    public MemoEditorViewModel(IEventAggregator aggregator, IMemoService memoService, IMapper mapper, IMemoLocalRepository memoRepository, TimerService timerService) : base(aggregator)
    {
        _memoService = memoService;
        _mapper = mapper;
        _memoRepository = memoRepository;
        _timerService = timerService;
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
            var memo = _memoModels.FirstOrDefault(predicate: m => m.Memo.Id.Equals(arg.Id));
            if (memo == null) return;
            memo.Status = arg.Status;
        });
        _memoModels = new ObservableCollection<EditableMemoModel>();
        ContentUpdateCommand = new DelegateCommand<EditableMemoModel>(UpdateMemo);
        AddMemoCommand = new DelegateCommand(AddMemoAtPosition);
        ConfirmContentInput = new DelegateCommand<RichTextBox>(textBox =>
        {
            var content = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
            var context = textBox.Document.DataContext as EditableMemoModel;
            context.Memo.Content = content;
#if LOCAL
            _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(context.Memo));
#else
            _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(context.Memo));
            if (App.IsUserRegistered) _memoService.UpdateAsync(_mapper.Map<MemoDto>(context.Memo));
#endif
        });
        DeleteSpecific = new DelegateCommand<EditableMemoModel>(e =>
        {
            _memoRepository.DeleteAsync(e.Memo.Id);
            if (App.IsUserRegistered) _memoService.DeleteAsync(e.Memo.Id);
            MemoModels.Remove(e);
        });
    }
    
    private async void UpdateMemo(EditableMemoModel memo)
    {
#if LOCAL
        await _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo));
#else
        await _memoRepository.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo));
        if (App.IsUserRegistered) await _memoService.UpdateAsync(_mapper.Map<MemoDto>(memo.Memo));
#endif
        _timerService.RegisterToTimers(memo.Memo);
    }

    private async void AddMemoAtPosition()
    {
        Aggregator.UpdateLoadingStatus(true);
        /*var latterMemos = from memo in _memoModels where memo.Memo.Order >= position select memo.Memo;
        foreach (var memo in latterMemos)
        {
#if LOCAL
            var changeModel = (await _memoRepository.GetFirstOrDefaultAsync(memo.Id)).Result;
#else
            var changeModel = (await _memoRepository.GetFirstOrDefaultAsync(memo.Id)).Result;
            //var changeModel = (await _memoService.GetFirstOrDefaultAsync(memo.Id)).Result;
#endif
            if (changeModel == null) continue;
            changeModel.Order += 1;
#if LOCAL
            await _memoRepository.UpdateAsync(changeModel);
#else
            await _memoRepository.UpdateAsync(changeModel);
            if (App.IsUserRegistered) await _memoService.UpdateAsync(changeModel);
#endif
        }*/
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
            Order = _memoModels.Count,
            Active = false,
            Category = 1,
            Title = string.Empty,
            Content = string.Empty,
            StartTime = t,
            EndTime = t.AddMinutes(1)
        };
        var result = await _memoRepository.AddAsync(memoDto);
        if (App.IsUserRegistered) await _memoService.AddAsync(memoDto);
#endif        
        var memoRecord = _mapper.Map<MemoRecord>(result.Result);
        _timerService.RegisterToTimers(memoRecord);
        _memoModels.Insert(_memoModels.Count, new EditableMemoModel(memoRecord));
        Aggregator.UpdateLoadingStatus(false);
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);
        Aggregator.NotifyAction(ActionsToNotify.PassToMemoEditor);
    }
}