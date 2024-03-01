using System.Diagnostics.CodeAnalysis;
using DiaryProject.Events;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserViewModel : NavigationModel
{
    private readonly IMemoLocalRepository _repository;
    private readonly IMemoService _memoService;
    
    public DelegateCommand LogoutCommand { get; private init; }

    public UserViewModel(IEventAggregator aggregator, IMemoLocalRepository repository, IMemoService memoService) : base(aggregator)
    {
        _repository = repository;
        _memoService = memoService;
        
        LogoutCommand = new DelegateCommand(() =>
        {
            App.IsUserRegistered = false;
            App.IsSynchronizing = false;
            App.UserToken = string.Empty;
            Aggregator.UpdateUserStatus(UserOperation.ExitAccount, string.Empty);
        });
    }

    public override void OnNavigatedTo(NavigationContext context)
    {
        base.OnNavigatedTo(context);
        if (_repository.GetVersion() != _memoService.GetVersion()) return;
        _repository.UpdateChanges(_memoService);
    }
}