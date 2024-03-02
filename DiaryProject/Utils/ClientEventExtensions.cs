using DiaryProject.Events;
using DiaryProject.Models;

namespace DiaryProject.Utils;

public enum ActionsToNotify
{
    PassToMemoEditor
}

public static class ClientEventExtensions
{
    public static void UpdateLoadingStatus(this IEventAggregator aggregator, bool isOpen)
    {
        aggregator.GetEvent<LoadingStatusChanged>().Publish(new LoadingStatusModel { IsOpen = isOpen });
    }

    public static void PassToEditor(this IEventAggregator aggregator, List<MemoRecord> memos, DateTime editedDate)
    {
        aggregator.GetEvent<EditorUpdated>().Publish(new EditorUpdatedModel { Memos = memos, EditedDate = editedDate});
    }

    /// <summary>
    /// 设置用户能否使用编辑器
    /// </summary>
    public static void UpdateEditorStatus(this IEventAggregator aggregator, bool isEnabled)
    {
        aggregator.GetEvent<EditorNavigationChanged>().Publish(new EditorStatusModel { IsEnabled = isEnabled });
    }

    public static void NotifyAction(this IEventAggregator aggregator, ActionsToNotify action)
    {
        aggregator.GetEvent<ActionNotified>().Publish(action);
    }

    public static void NotifyNavigation(this IEventAggregator aggregator, NavigationContext context)
    {
        aggregator.GetEvent<PageNavigatedTo>().Publish(context);
    }

    public static void UpdateUserStatus(this IEventAggregator aggregator, UserOperation operation, string token)
    {
        aggregator.GetEvent<AccountEvent>().Publish(operation);
    }
}