using System.Diagnostics.CodeAnalysis;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using RestSharp;

namespace DiaryProject.Service.Web;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserService(HttpRestClient client) : IUserService
{
    public async Task<ApiResponse<string>> RegisterAsync(UserDto userDto)
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = "api/User/Register",
            Parameter = userDto
        };
        return await client.ExecuteAsync<string>(request);
    }

    public async Task<ApiResponse<string>> LoginAsync(string userName, string password)
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = $"api/User/Login?userName={userName}&password={password}"
        };
        return await client.ExecuteAsync<string>(request);
    }
}