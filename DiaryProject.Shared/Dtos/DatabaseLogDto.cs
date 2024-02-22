using SQLite;

namespace DiaryProject.Shared.Dtos;

public enum DatabaseOperation
{
    Add,
    Update,
    Delete,
    None
}

[Table("logs")]
public class DatabaseLogDto : BaseDto
{
    public int EntityId { get; set; }

    public int Operation { get; set; }

    public bool Exists { get; set; }

    public DateTime UpdateTime { get; set; }
}