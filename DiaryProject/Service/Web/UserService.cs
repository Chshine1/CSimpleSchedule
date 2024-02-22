using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using RestSharp;

namespace DiaryProject.Service.Web;

public class UserService : IUserService
{
    private readonly HttpRestClient _client;

    public UserService(HttpRestClient client)
    {
        _client = client;
    }
    
    public async Task<ApiResponse<UserDto>> RegisterAsync(UserDto userDto)
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = "api/User/Register",
            Parameter = userDto
        };
        return await _client.ExecuteAsync<UserDto>(request);
    }

    public async Task<ApiResponse<string>> LoginAsync(string userName, string password)
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = $"api/User/Login?userName={userName}&password={password}"
        };
        return await _client.ExecuteAsync<string>(request);
    }
}