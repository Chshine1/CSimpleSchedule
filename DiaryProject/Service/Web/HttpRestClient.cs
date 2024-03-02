using System.Net.Http;
using DiaryProject.Shared.Contact;
using Newtonsoft.Json;
using RestSharp;

namespace DiaryProject.Service.Web;

/// <summary>
/// 客户端用于发送请求的类
/// </summary>
public class HttpRestClient
{
    private readonly RestClient _restClient;

    public HttpRestClient(string apiUrl)
    {
        var option = new RestClientOptions(apiUrl);
        _restClient = new RestClient(option);
    }

    /// <summary>
    /// 异步执行指定的请求
    /// </summary>
    /// <returns>收到的响应</returns>
    /// <typeparam name="T">响应返回结果的类型</typeparam>
    public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
    {
        try
        {
            var request = new RestRequest(baseRequest.Route, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            
            if (baseRequest.Parameter != null)
                request.AddParameter("application/json", JsonConvert.SerializeObject(baseRequest.Parameter), ParameterType.RequestBody);

            if (App.IsUserRegistered && App.UserToken != null)
                request.AddHeader("auth", App.UserToken);

            var response = await _restClient.ExecuteAsync(request);
            // ReSharper disable once NullableWarningSuppressionIsUsed
            return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content ?? throw new HttpRequestException())!;
        }
        catch (Exception e)
        {
            return new ApiResponse<T> { Status = false, Message = e.Message, Connected = false };
        }
    }
}