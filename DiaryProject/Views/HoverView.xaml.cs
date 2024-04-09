using System.Windows;
using System.Windows.Input;
using DiaryProject.Events;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Dtos;
using DiaryProject.Utils;
using DragEventArgs = System.Windows.DragEventArgs;

namespace DiaryProject.Views;

public partial class HoverView : Window
{
    private readonly FileCopyService _fileService;
    private readonly IEventAggregator _aggregator;
    private readonly IMemoService _memoService;
    private readonly IMemoLocalRepository _memoRepository;

    public HoverView(FileCopyService fileService, IEventAggregator aggregator, IMemoService memoService, IMemoLocalRepository memoRepository)
    {
        _fileService = fileService;
        _aggregator = aggregator;
        _memoService = memoService;
        _memoRepository = memoRepository;
        InitializeComponent();
    }

    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void OnClose(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        Topmost = true;
        Left = 300;
        Top = 300;
    }

    private void PopupOptions(object sender, MouseButtonEventArgs e)
    {
        Menu.IsOpen = true;
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(typeof(string))) return;
        try
        {
            var text = (string?) e.Data.GetData(typeof(string));
            if (string.IsNullOrEmpty(text)) return;
            AddMemo(text);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
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

    private void HideHover(object sender, RoutedEventArgs e)
    {
        App.IsHoverVisible = false;
        _aggregator.GetEvent<HoverStatusChanged>().Publish(new HoverStatusModel { IsVisible = HoverStatus.Hide });
        Menu.IsOpen = false;
        Hide();
    }
}