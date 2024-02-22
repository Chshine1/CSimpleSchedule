using RestSharp;

namespace DiaryProject.Service.Web;

/// <summary>
/// Base record for http requests
/// </summary>
public record BaseRequest
{
    public Method Method { get; init; }
    
    public required string Route { get; init; }
    
    public string ContentType { get; set; } = "application/json";
    
    public object? Parameter { get; init; }
}