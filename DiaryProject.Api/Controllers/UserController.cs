using System.Text;
using DiaryProject.Api.Extension;
using DiaryProject.Api.Service;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Mvc;

namespace DiaryProject.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController(IUserService service) : ControllerBase
{
    [HttpPost]
    public async Task<ApiResponse> Register(UserDto userDto)
    {
        var user = await service.RegisterAsync(userDto);
        return !user.Status
            ? new ApiResponse(user.Message)
            : new ApiResponse(true, JwtUserExtension.GetToken(user.Result));
    }

    [HttpGet]
    public async Task<ApiResponse> Login(string userName, string password)
    {
        var user = await service.VerifyUserAsync(userName, password);
        return !user.Status
            ? new ApiResponse(user.Message)
            : new ApiResponse(true, JwtUserExtension.GetToken(user.Result));
    }
}