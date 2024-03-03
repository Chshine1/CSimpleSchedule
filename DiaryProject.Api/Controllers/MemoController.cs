using DiaryProject.Api.Extension;
using DiaryProject.Api.Service;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace DiaryProject.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MemoController(IMemoService memoService, IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ApiResponse<MemoDto>> GetSingle(int id)
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.GetSingleAsync(id, user);
    }

    [HttpGet]
    public async Task<ApiResponse<IEnumerable<MemoDto>>> GetAll([FromQuery] MemoParameter parameter)
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.GetAllAsync(parameter, user);
    }

    [HttpPost]
    public async Task<ApiResponse<MemoDto>> Add(MemoDto memoDto)
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.AddAsync(memoDto, user);
    }

    [HttpPost]
    public async Task<ApiResponse<MemoDto>> Update(MemoDto memoDto)
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.UpdateAsync(memoDto, user);
    }

    [HttpDelete]
    public async Task<ApiResponse<MemoDto>> Delete(int id)
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.DeleteAsync(id, user);
    }

    [HttpDelete]
    public async Task<ApiResponse<string>> DeleteAll()
    {
        var user = await JwtUserExtension.GetUserId(HttpContext, userService);
        return await memoService.DeleteAllAsync(user);
    }
}