using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Api.Context;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record Memo : BaseEntity
{
    public int Order { get; set; }

    public bool Active { get; set; }

    public int Category { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }

    public User? User { get; set; }
    public int UserId { get; set; }
}