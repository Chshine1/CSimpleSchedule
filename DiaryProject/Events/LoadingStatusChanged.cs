namespace DiaryProject.Events;

public class LoadingStatusChangeModel
{
    public bool IsOpen { get; init; }
}

public class LoadingStatusChanged : PubSubEvent<LoadingStatusChangeModel>;