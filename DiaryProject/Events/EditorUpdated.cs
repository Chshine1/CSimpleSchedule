using DiaryProject.Models;

namespace DiaryProject.Events;

public class EditorUpdatedModel
{
    public required List<MemoRecord> Memos { get; init; }
    public DateTime EditedDate { get; init; }
}

public class EditorUpdated : PubSubEvent<EditorUpdatedModel>;