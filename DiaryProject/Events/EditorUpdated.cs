using DiaryProject.Models;

namespace DiaryProject.Events;

public class EditorStartedModel
{
    public required List<MemoRecord> Memos { get; init; }
    public DateTime EditedDate { get; init; }
}

public class EditorUpdated : PubSubEvent<EditorStartedModel>;