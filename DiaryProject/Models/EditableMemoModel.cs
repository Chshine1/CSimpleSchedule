using System.Diagnostics.CodeAnalysis;
using System.Windows.Documents;
using DiaryProject.Utils;

namespace DiaryProject.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class EditableMemoModel : BindableBase
{
    private static readonly string[] HeaderColors = ["DarkGray", "#6FA3FF", "#DB4848", "#DBCF5E", "#62CE58"];
    private static readonly string[] CategoryTexts = ["-未分类-", " 日记", " 提醒", " 闹钟", " 备忘录"];
    private static readonly string[] CategoryDisplays = ["事件类别：未分类", "事件类别：日记", "事件类别：提醒", "事件类别：闹钟", "事件类别：备忘录"];
    
    private bool _status;
    private MemoRecord _memo;
    
    #region Properties

    public bool Status
    {
        get => _status;
        set
        {
            _status = value;
            RaisePropertyChanged(nameof(StatusText));
            RaisePropertyChanged(nameof(BackgroundColor));
        }
    }

    public MemoRecord Memo
    {
        get => _memo;
        set
        {
            _memo = value;
            ContentDocument = _memo.Content.ConvertToFlowDocument();
            ContentDocument.Focusable = true;
            ContentDocument.LineHeight = 1;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ContentDocument));
        }
    }

    public FlowDocument? ContentDocument { get; private set; }

    public string StartTimeHour
    {
        get => $"{_memo.StartTime.Hour:00}";
        set
        {
            var hour = 0;
            try { hour = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            hour = Math.Min(Math.Max(hour, 0), 23);
            
            _memo.StartTime = new DateTime(_memo.StartTime.Year, _memo.StartTime.Month, _memo.StartTime.Day, hour,
                _memo.StartTime.Minute, 0);
            if (_memo.StartTime >= _memo.EndTime && _memo.StartTime is { Hour: 23, Minute: 59 })
            {
                _memo.StartTime = new DateTime(_memo.StartTime.Year, _memo.StartTime.Month, _memo.StartTime.Day, 23,
                    58, 0);
                _memo.EndTime = _memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(StartTimeMinute));
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
            else if (_memo.StartTime >= _memo.EndTime)
            {
                _memo.EndTime = _memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
        }
    }

    public string StartTimeMinute
    {
        get => $"{_memo.StartTime.Minute:00}";
        set
        {
            var minute = 0;
            try { minute = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            minute = Math.Min(Math.Max(minute, 0), 59);
            
            _memo.StartTime = new DateTime(_memo.StartTime.Year, _memo.StartTime.Month, _memo.StartTime.Day, _memo.StartTime.Hour,
                minute, 0);
            if (_memo.StartTime >= _memo.EndTime && _memo.StartTime is { Hour: 23, Minute: 59 })
            {
                _memo.StartTime = new DateTime(_memo.StartTime.Year, _memo.StartTime.Month, _memo.StartTime.Day, 23,
                    58, 0);
                _memo.EndTime = _memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
            else if (_memo.StartTime >= _memo.EndTime)
            {
                _memo.EndTime = _memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
        }
    }
    
    public string EndTimeHour
    {
        get => $"{_memo.EndTime.Hour:00}";
        set
        {
            var hour = 0;
            try { hour = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            hour = Math.Min(Math.Max(hour, 0), 23);
            
            _memo.EndTime = new DateTime(_memo.EndTime.Year, _memo.EndTime.Month, _memo.EndTime.Day, hour,
                _memo.EndTime.Minute, 0);
        }
    }

    public string EndTimeMinute
    {
        get => $"{_memo.EndTime.Minute:00}";
        set
        {
            var minute = 0;
            try { minute = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            minute = Math.Min(Math.Max(minute, 0), 59);
            
            _memo.EndTime = new DateTime(_memo.EndTime.Year, _memo.EndTime.Month, _memo.EndTime.Day, _memo.EndTime.Hour,
                minute, 0);
        }
    }

    public string HeaderColor => HeaderColors[_memo.Category - 1];

    public int CategoryIndex
    {
        get => _memo.Category - 1;
        set
        {
            _memo.Category = value + 1;
            RaisePropertyChanged(nameof(CategoryText));
            RaisePropertyChanged(nameof(HeaderColor));
            RaisePropertyChanged(nameof(CategoryDisplay));
        }
    }

    public string CategoryText => CategoryTexts[_memo.Category - 1];

    public string CategoryDisplay => CategoryDisplays[_memo.Category - 1];

    public string StatusText => !Memo.Active ? "状态：未激活" : Status ? "状态：进行中" : "状态：未开始";

    public string BackgroundColor => Memo.Active && Status ? "#E6EFFF" : "#F3F3F4";

    #endregion

    public EditableMemoModel(MemoRecord memo)
    {
        _memo = memo;
        ContentDocument = _memo.Content.ConvertToFlowDocument();
        ContentDocument.Focusable = true;
        ContentDocument.LineHeight = 1;
    }
}