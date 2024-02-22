using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Api.Context;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record BaseEntity
{
    public int Id { get; set; }
    
    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }
}