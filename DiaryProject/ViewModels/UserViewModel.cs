using System.Diagnostics.CodeAnalysis;
using DiaryProject.Events;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserViewModel : NavigationModel
{
    public DelegateCommand LogoutCommand { get; private init; }
    public DelegateCommand UseLocalCommand { get; private init; }
    public DelegateCommand UseWebCommand { get; private init; }

    public UserViewModel(IEventAggregator aggregator, IMemoLocalRepository repository, IMemoService memoService) : base(aggregator)
    {
        LogoutCommand = new DelegateCommand(() =>
        {
            App.IsUserRegistered = false;
            App.IsSynchronizing = false;
            App.UserToken = string.Empty;
            Aggregator.UpdateUserStatus(UserOperation.ExitAccount, string.Empty);
        });
        UseLocalCommand = new DelegateCommand(() =>
        {
            repository.UpdateChangesAsync(memoService);
            App.IsSynchronizing = true;
            Aggregator.GetEvent<SynchronizingEvent>().Publish(true);
        });
        UseWebCommand = new DelegateCommand(() =>
        {
            memoService.UpdateChangesAsync(repository);
            App.IsSynchronizing = true;
            Aggregator.GetEvent<SynchronizingEvent>().Publish(true);
        });
    }
}