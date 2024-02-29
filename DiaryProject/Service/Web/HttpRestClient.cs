using System.Net.Http;
using DiaryProject.Shared.Contact;
using Newtonsoft.Json;
using RestSharp;

namespace DiaryProject.Service.Web;

/// <summary>
/// Client class for sending http requests
/// </summary>
public class HttpRestClient
{
    private readonly RestClient _restClient;

    public HttpRestClient(string apiUrl)
    {
        var option = new RestClientOptions(apiUrl);
        _restClient = new RestClient(option);
    }
    
    /*public async Task<ApiResponse> ExecuteAsync(BaseRequest baseRequest)
    {
        var request = new RestRequest(baseRequest.Route, baseRequest.Method);
        request.AddHeader("Content-Type", baseRequest.ContentType);
        if (baseRequest.Parameter != null)
        {
            request.AddParameter("param", JsonConvert.SerializeObject(baseRequest.Parameter),
                ParameterType.RequestBody);
        }

        var response = await _restClient.ExecuteAsync(request);
        if (response.Content == null) throw new HttpRequestException("Null response content");
        return JsonConvert.DeserializeObject<ApiResponse>(response.Content)!;
    }*/

    /// <summary>
    /// Execute the given http request
    /// </summary>
    /// <returns>An api response</returns>
    /// <typeparam name="T">Returned class</typeparam>
    public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
    {
        try
        {
            var request = new RestRequest(baseRequest.Route, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                request.AddParameter("application/json", JsonConvert.SerializeObject(baseRequest.Parameter),
                    ParameterType.RequestBody);
            }

            if (App.IsUserRegistered && App.UserToken != null) request.AddHeader("auth", App.UserToken);

            var response = await _restClient.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content ?? throw new HttpRequestException())!;
        }
        catch (Exception e)
        {
            return new ApiResponse<T> { Status = false, Message = e.Message, Connected = false };
        }
    }
}