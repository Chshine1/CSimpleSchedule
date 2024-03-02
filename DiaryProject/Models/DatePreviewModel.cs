using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AutoMapper;
using DiaryProject.Shared.Dtos;
using SQLite;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;

namespace DiaryProject.Models;

/// <summary>
/// 日程预览中一个单元格的模型类
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DatePreviewModel : BindableBase, INotifyPropertyChanged
{
    private static readonly string[] BackgroundColors = ["LightGray", "#91B9FF", "#E16464", "#E1D77A", "#7BD672", "DarkGray", "#6FA3FF", "#DB4848", "#DBCF5E", "#62CE58"];
    private static string CategoryColor(int category, bool active)
    {
        return BackgroundColors[category - 1 + (active ? 0 : 5)];
    }
    
    private readonly List<MemoRecord> _memos;
    private readonly bool _isNow, _active;
    private bool _isSelected;
    
    private List<MemoRecord> ActiveMemos => (from memo in _memos where memo.Active select memo).ToList();

    #region BoundProperties
    
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(DateText));
            OnPropertyChanged();
        }
    }

    public DateTime Date { get; }

    public string DateText => $"{Date.Day}  {LunarDateTime.FromGregorian(Date).RiInChinese()}{(_isNow ? "(今天)" : "")}{(IsSelected ? "(选中)" : "")}";

    public string MainTextColor => _active ? "#333333" : "Gray";

    public string BackgroundColor => _isNow ? "#D7E5FE" : _active ? "#F3F3F4" : "LightGray";

    public string BorderColor => _isNow ? "#91B9FF" : "#D9D9D9";

    public string FirstColor => ActiveMemos.Count > 0 ? CategoryColor(ActiveMemos[0].Category, _active) : "Transparent";

    public string SecondColor => ActiveMemos.Count > 1 ? CategoryColor(ActiveMemos[1].Category, _active) : "Transparent";
    
    public string ThirdColor => ActiveMemos.Count > 2 ? CategoryColor(ActiveMemos[2].Category, _active) : "Transparent";
    
    public string ForthColor => _memos.Count == 4 && ActiveMemos.Count == 4 ? CategoryColor(ActiveMemos[3].Category, _active) : "Transparent";

    public string FirstInfo => ActiveMemos.Count > 0 ? ActiveMemos[0].Title : string.Empty;

    public string SecondInfo => ActiveMemos.Count > 1 ? ActiveMemos[1].Title : string.Empty;
    
    public string ThirdInfo => ActiveMemos.Count > 2 ? ActiveMemos[2].Title : string.Empty;

    public string ForthInfo => _memos.Count == 4 && ActiveMemos.Count == 4
        ? ActiveMemos[3].Title
        : _memos.Count <= 3 && _memos.Count == ActiveMemos.Count
            ? string.Empty
            : $"+{_memos.Count - Math.Min(3, ActiveMemos.Count)}";

    #endregion
 
    public DatePreviewModel(bool active, DateTime date, IGrouping<int, MemoDto>? grouping, IMapperBase mapper)
    {
        Date = date;
        _active = active;
        _isNow = DateTime.Now.Day == date.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year;
        if (grouping == null)
        {
            _memos = new List<MemoRecord>();
            return;
        }
        _memos = (from m in grouping orderby m.Order select mapper.Map<MemoRecord>(m)).ToList();
    }

    #region PublicMethods

    public List<MemoRecord> GetMemos() => _memos;
    
    public IEnumerable<int> GetMemoIndices()
    {
        return (from memo in _memos select memo.Id).ToArray();
    }
    
    public void Clear()
    {
        _memos.Clear();
        OnPropertyChanged(nameof(FirstColor));
        OnPropertyChanged(nameof(SecondColor));
        OnPropertyChanged(nameof(ThirdColor));
        OnPropertyChanged(nameof(ForthColor));
        OnPropertyChanged(nameof(FirstInfo));
        OnPropertyChanged(nameof(SecondInfo));
        OnPropertyChanged(nameof(ThirdInfo));
        OnPropertyChanged(nameof(ForthInfo));
    }

    #endregion
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MemoRecord
{
    public int Id { get; set; }
    public int Order { get; set; }
    public bool Active { get; set; }
    public int Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}