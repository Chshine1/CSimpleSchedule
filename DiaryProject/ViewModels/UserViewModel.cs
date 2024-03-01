using System.Diagnostics.CodeAnalysis;
using DiaryProject.Events;
using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserViewModel : NavigationModel
{
    public DelegateCommand LogoutCommand { get; private init; }

    public UserViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        LogoutCommand = new DelegateCommand(() =>
        {
            Aggregator.UpdateUserStatus(UserOperation.ExitAccount, string.Empty);
        });
    }
}