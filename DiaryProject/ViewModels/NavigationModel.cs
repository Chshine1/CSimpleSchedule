using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

public class NavigationModel : BindableBase, INavigationAware
{
    protected readonly IEventAggregator Aggregator;

    protected NavigationModel(IEventAggregator aggregator)
    {
        Aggregator = aggregator;
    } 
    
    public virtual bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return true;
    }
    
    public virtual void OnNavigatedTo(NavigationContext navigationContext)
    {
        Aggregator.NotifyNavigation(navigationContext);
    }

    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    {
        
    }
}