using DiaryProject.Events;

namespace DiaryProject.ViewModels;

public class HoverSettingsViewModel : NavigationModel
{
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

    public HoverSettingsViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        aggregator.GetEvent<HoverStatusChanged>().Subscribe(_ =>
        {
            RaisePropertyChanged(nameof(HoverVisible));
        });
    }
}