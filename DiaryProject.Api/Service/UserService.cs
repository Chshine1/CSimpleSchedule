using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DiaryProject.Api.Context;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DiaryProject.Api.Service;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
    public async Task<ApiResponse> RegisterAsync(UserDto userDto)
    {
        try
        {
            var exists = await unitOfWork.GetRepository<User>().ExistsAsync(u => u.UserName.Equals(userDto.UserName));
            if (exists) return new ApiResponse("Username already exists");
            var user = mapper.Map<User>(userDto);
            await unitOfWork.GetRepository<User>().InsertAsync(user);
            return await unitOfWork.SaveChangesAsync() > 0
                ? new ApiResponse(true, user)
                : new ApiResponse("Failed to register");
        }
        catch (Exception e)
        {
            return new ApiResponse(e.Message);
        }
    }
    
    public async Task<ApiResponse<UserDto>> VerifyUserAsync(string userName, string password)
    {
        try
        {
            var model = await unitOfWork.GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: u => u.UserName.Equals(userName));
            return model.Password != password
                ? new ApiResponse<UserDto> { Status = false, Message = "Wrong password" }
                : new ApiResponse<UserDto> { Status = true, Result = mapper.Map<UserDto>(model) };
        }
        catch (Exception e)
        {
            return new ApiResponse<UserDto> { Status = false, Message = e.Message };
        }
    }
}