using DiaryProject.Utils;

namespace DiaryProject.Events;

public class NotifyActionModel
{
    public ActionsToNotify ActionToNotify { get; init; }
}

public class ActionNotified : PubSubEvent<NotifyActionModel>;