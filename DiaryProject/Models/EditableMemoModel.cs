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

    public MemoRecord Memo { get; }

    public FlowDocument ContentDocument { get; set; }

    public string StartTimeHour
    {
        get => $"{Memo.StartTime.Hour:00}";
        set
        {
            var hour = 0;
            try { hour = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            hour = Math.Min(Math.Max(hour, 0), 23);
            
            Memo.StartTime = new DateTime(Memo.StartTime.Year, Memo.StartTime.Month, Memo.StartTime.Day, hour,
                Memo.StartTime.Minute, 0);
            if (Memo.StartTime >= Memo.EndTime && Memo.StartTime is { Hour: 23, Minute: 59 })
            {
                Memo.StartTime = new DateTime(Memo.StartTime.Year, Memo.StartTime.Month, Memo.StartTime.Day, 23,
                    58, 0);
                Memo.EndTime = Memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(StartTimeMinute));
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
            else if (Memo.StartTime >= Memo.EndTime)
            {
                Memo.EndTime = Memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
        }
    }

    public string StartTimeMinute
    {
        get => $"{Memo.StartTime.Minute:00}";
        set
        {
            var minute = 0;
            try { minute = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            minute = Math.Min(Math.Max(minute, 0), 59);
            
            Memo.StartTime = new DateTime(Memo.StartTime.Year, Memo.StartTime.Month, Memo.StartTime.Day, Memo.StartTime.Hour,
                minute, 0);
            if (Memo.StartTime >= Memo.EndTime && Memo.StartTime is { Hour: 23, Minute: 59 })
            {
                Memo.StartTime = new DateTime(Memo.StartTime.Year, Memo.StartTime.Month, Memo.StartTime.Day, 23,
                    58, 0);
                Memo.EndTime = Memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
            else if (Memo.StartTime >= Memo.EndTime)
            {
                Memo.EndTime = Memo.StartTime.AddMinutes(1);
                RaisePropertyChanged(nameof(EndTimeHour));
                RaisePropertyChanged(nameof(EndTimeMinute));
            }
        }
    }
    
    public string EndTimeHour
    {
        get => $"{Memo.EndTime.Hour:00}";
        set
        {
            var hour = 0;
            try { hour = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            hour = Math.Min(Math.Max(hour, 0), 23);
            
            Memo.EndTime = new DateTime(Memo.EndTime.Year, Memo.EndTime.Month, Memo.EndTime.Day, hour,
                Memo.EndTime.Minute, 0);
        }
    }

    public string EndTimeMinute
    {
        get => $"{Memo.EndTime.Minute:00}";
        set
        {
            var minute = 0;
            try { minute = Convert.ToInt32(value); }
            catch (Exception) { /* ignored */ }
            minute = Math.Min(Math.Max(minute, 0), 59);
            
            Memo.EndTime = new DateTime(Memo.EndTime.Year, Memo.EndTime.Month, Memo.EndTime.Day, Memo.EndTime.Hour,
                minute, 0);
        }
    }

    public string HeaderColor => HeaderColors[Memo.Category - 1];

    public int CategoryIndex
    {
        get => Memo.Category - 1;
        set
        {
            Memo.Category = value + 1;
            RaisePropertyChanged(nameof(CategoryText));
            RaisePropertyChanged(nameof(HeaderColor));
            RaisePropertyChanged(nameof(CategoryDisplay));
        }
    }

    public string CategoryText => CategoryTexts[Memo.Category - 1];

    public string CategoryDisplay => CategoryDisplays[Memo.Category - 1];

    public string StatusText => !Memo.Active ? "状态：未激活" : Status ? "状态：进行中" : "状态：未开始";

    public string BackgroundColor => Memo.Active && Status ? "#E6EFFF" : "#F3F3F4";

    #endregion

    public EditableMemoModel(MemoRecord memo)
    {
        Memo = memo;
        ContentDocument = Memo.Content.ConvertToFlowDocument();
        ContentDocument.Focusable = true;
        ContentDocument.LineHeight = 1;
    }
}