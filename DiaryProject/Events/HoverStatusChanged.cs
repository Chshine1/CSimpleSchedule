namespace DiaryProject.Events;

public enum HoverVisibility
{
    Reverse,
    Visible,
    Hidden
}

public class HoverStatusModel
{
    public HoverVisibility IsVisible { get; init; }
}

public class HoverStatusChanged : PubSubEvent<HoverStatusModel>;