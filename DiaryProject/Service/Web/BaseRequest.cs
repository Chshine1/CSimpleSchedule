using RestSharp;

namespace DiaryProject.Service.Web;

/// <summary>
/// 客户端发送Http请求的记录
/// </summary>
public record BaseRequest
{
    public Method Method { get; init; }
    
    public required string Route { get; init; }
    
    public string ContentType { get; set; } = "application/json";
    
    public object? Parameter { get; init; }
}