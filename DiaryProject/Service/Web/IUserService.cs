using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Service.Web;

public interface IUserService
{
    Task<ApiResponse<string>> RegisterAsync(UserDto userDto);

    Task<ApiResponse<string>> LoginAsync(string userName, string password);
}