using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service.Local;

namespace DiaryProject.ViewModels;

public class HoverSettingsViewModel : NavigationModel
{
    private FileCopyService _fileService;
    
    private HoverConfiguration _configuration;
    
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

    public HoverSettingsViewModel(IEventAggregator aggregator, FileCopyService fileService) : base(aggregator)
    {
        _fileService = fileService;
        
        aggregator.GetEvent<HoverStatusChanged>().Subscribe(_ =>
        {
            RaisePropertyChanged(nameof(HoverVisible));
        });
        
        _configuration = _fileService.ReadHoverConfiguration();
        ShowOnRegistered = _configuration.ShowOnRegistered;
        SetActiveOnAdded = _configuration.SetActiveOnAdded;
    }
}