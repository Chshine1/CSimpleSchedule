using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Service.Web;

public interface IUserService
{
    Task<ApiResponse<UserDto>> RegisterAsync(UserDto userDto);

    Task<ApiResponse<string>> LoginAsync(string userName, string password);
}