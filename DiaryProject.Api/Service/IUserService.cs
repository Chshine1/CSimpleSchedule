using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Api.Service;

public interface IUserService
{
    Task<ApiResponse<UserDto>> RegisterAsync(UserDto userDto);
    
    Task<ApiResponse<UserDto>> VerifyUserAsync(string userName, string password);
}