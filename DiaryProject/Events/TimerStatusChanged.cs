namespace DiaryProject.Events;

public class TimerStatusChangedModel
{
    public int Id { get; init; }
    public bool Status { get; init; }
    public bool SendNotification { get; init; }
}

public class TimerStatusChanged : PubSubEvent<TimerStatusChangedModel>;