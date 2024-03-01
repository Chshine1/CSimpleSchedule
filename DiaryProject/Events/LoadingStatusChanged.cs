namespace DiaryProject.Events;

public class LoadingStatusModel
{
    public bool IsOpen { get; init; }
}

public class LoadingStatusChanged : PubSubEvent<LoadingStatusModel>;