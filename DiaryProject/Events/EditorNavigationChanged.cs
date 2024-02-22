namespace DiaryProject.Events;

public class EditorStatusModel
{
    public bool IsEnabled { get; init; }
}

public class EditorNavigationChanged : PubSubEvent<EditorStatusModel>;