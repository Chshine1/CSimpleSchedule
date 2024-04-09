using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service.Local;

namespace DiaryProject.ViewModels;

public class HoverSettingsViewModel : NavigationModel
{
    private static readonly string[] CategoryTexts = ["-不分类-", " 日记", " 提醒", " 闹钟", " 备忘录"];

    private readonly FileCopyService _fileService;
    
    private readonly HoverConfiguration _configuration;
    
    public string CategoryText => CategoryTexts[CategoryIndex];
    
    public bool HoverVisible
    {
        get => App.IsHoverVisible;
        set
        {
            App.IsHoverVisible = value;
            Aggregator.GetEvent<HoverStatusChanged>().Publish(new HoverStatusModel
                { IsVisible = value ? HoverStatus.Show : HoverStatus.Hide });
        }
    }

    public bool ShowOnRegistered
    {
        get => _configuration.ShowOnRegistered;
        set
        {
            _configuration.ShowOnRegistered = value;
            _fileService.WriteHoverConfiguration(_configuration);
        }
    }

    public bool SetActiveOnAdded
    {
        get => _configuration.SetActiveOnAdded;
        set
        {
            _configuration.SetActiveOnAdded = value;
            _fileService.WriteHoverConfiguration(_configuration);
        }
    }

    public int CategoryIndex
    {
        get => _configuration.DefaultMemoCategory;
        set
        {
            _configuration.DefaultMemoCategory = value;
            _fileService.WriteHoverConfiguration(_configuration);
            RaisePropertyChanged(nameof(CategoryText));
        }
    }

    public HoverSettingsViewModel(IEventAggregator aggregator, FileCopyService fileService) : base(aggregator)
    {
        _fileService = fileService;
        
        aggregator.GetEvent<HoverStatusChanged>().Subscribe(_ =>
        {
            RaisePropertyChanged(nameof(HoverVisible));
        });
        
        _configuration = _fileService.ReadHoverConfiguration();
    }
}