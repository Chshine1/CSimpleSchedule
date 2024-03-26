namespace DiaryProject.Events;

public enum HoverStatus
{
    RevertVisibility,
    Show,
    Hide
}

public class HoverStatusModel
{
    public HoverStatus IsVisible { get; init; }
}

public class HoverStatusChanged : PubSubEvent<HoverStatusModel>;