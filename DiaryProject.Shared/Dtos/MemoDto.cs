using SQLite;

namespace DiaryProject.Shared.Dtos;

[Table("memos")]
public class MemoDto : BaseDto
{
    private int _memoOrder;
    private bool _active;
    private int _category;
    private string _title = null!;
    private string _content = null!;
    private DateTime _startTime;
    private DateTime _endTime;
    
    public int MemoOrder
    {
        get => _memoOrder;
        set
        {
            _memoOrder = value;
            OnPropertyChanged();
        }
    }
    
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            OnPropertyChanged();
        }
    }
    
    public int Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged();
        }
    }
    
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }
    
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }
    
    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged();
        }
    }
    
    public DateTime EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            OnPropertyChanged();
        }
    }
}