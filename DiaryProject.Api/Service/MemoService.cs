using System.Diagnostics.CodeAnalysis;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DiaryProject.Api.Context;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;
using DiaryProject.Shared.Utils;

namespace DiaryProject.Api.Service;

// Implementation type
public class MemoService(IUnitOfWork unitOfWork, [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")] IMapper mapper) : IMemoService
{
    public async Task<ApiResponse<MemoDto>> GetSingleAsync(int id, int user)
    {
        try
        {
            var model = await unitOfWork.GetRepository<Memo>()
                .GetFirstOrDefaultAsync(predicate: memo => memo.Id.Equals(id) && memo.UserId.Equals(user));
            return new ApiResponse<MemoDto> { Status = true, Result = mapper.Map<MemoDto>(model) };
        }
        catch (Exception e)
        {
            return new ApiResponse<MemoDto> { Status = false, Message = e.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<MemoDto>>> GetAllAsync(MemoParameter parameter, int user)
    {
        try
        {
            var models = await unitOfWork.GetRepository<Memo>()
                .GetAllAsync(predicate: memo => memo.UserId.Equals(user));
            return new ApiResponse<IEnumerable<MemoDto>>
                { Status = true, Result = from m in models where InQuery(m, parameter) select mapper.Map<MemoDto>(m) };
        }
        catch (Exception e)
        {
            return new ApiResponse<IEnumerable<MemoDto>> { Status = false, Message = e.Message };
        }
    }

    public async Task<ApiResponse<MemoDto>> AddAsync(MemoDto model, int user)
    {
        try
        {
            var memo = mapper.Map<Memo>(model);
            memo.UserId = user;
            await unitOfWork.GetRepository<Memo>().InsertAsync(memo);
            return await unitOfWork.SaveChangesAsync() > 0
                ? new ApiResponse<MemoDto> { Status = true, Result = mapper.Map<MemoDto>(memo) }
                : new ApiResponse<MemoDto> { Message = "Failed to add a new memo" };
        }
        catch (Exception e)
        {
            return new ApiResponse<MemoDto>{ Message = e.Message };
        }
    }

    public async Task<ApiResponse<MemoDto>> UpdateAsync(MemoDto model, int user)
    {
        try
        {
            var repository = unitOfWork.GetRepository<Memo>();
            var memo = await repository.GetFirstOrDefaultAsync(predicate: m => m.Id.Equals(model.Id) && m.UserId.Equals(user));
            memo.CopyFrom(mapper.Map<Memo>(model));
            memo.UpdateTime = DateTime.Now;
            memo.UserId = user;
            repository.Update(memo);
            return await unitOfWork.SaveChangesAsync() > 0
                ? new ApiResponse<MemoDto> { Status = true, Result = mapper.Map<MemoDto>(memo) }
                : new ApiResponse<MemoDto> { Message = "Failed to update the memo" };
        }
        catch (Exception e)
        {
            return new ApiResponse<MemoDto> { Message = e.Message };
        }
    }

    public async Task<ApiResponse<MemoDto>> DeleteAsync(int id, int user)
    {
        try
        {
            var repository = unitOfWork.GetRepository<Memo>();
            var memo = await repository.GetFirstOrDefaultAsync(predicate: m => m.Id.Equals(id) && m.UserId.Equals(user));
            repository.Delete(memo);
            return await unitOfWork.SaveChangesAsync() > 0
                ? new ApiResponse<MemoDto> { Status = true, Result = mapper.Map<MemoDto>(memo) }
                : new ApiResponse<MemoDto> { Message = "Failed to delete the memo" };
        }
        catch (Exception e)
        {
            return new ApiResponse<MemoDto> { Message = e.Message };
        }
    }

    public Task<ApiResponse<string>> DeleteAllAsync(int user)
    {
        try
        {
            unitOfWork.ExecuteSqlCommand($"DELETE FROM Memo WHERE UserId={user}");
            return Task.FromResult(new ApiResponse<string> { Status = true });
        }
        catch (Exception e)
        {
            return Task.FromResult(new ApiResponse<string> { Message = e.Message });
        }
    }

    private static bool InQuery(Memo memo, MemoParameter parameter)
    {
        if (!(parameter.Category == 0 || memo.Category == parameter.Category)) return false;
        if (parameter.Range == 0) return true;
        var time = DateTimeOffset.FromUnixTimeMilliseconds(parameter.Timestamp).DateTime;
        return parameter.Range switch
        {
            1 => memo.StartTime.Day.Equals(time.Day),
            2 => memo.StartTime.Month.Equals(time.Month),
            3 => memo.StartTime.Year.Equals(time.Year),
            _ => throw new ArgumentOutOfRangeException(nameof(parameter), "Invalid time range")
        };
    }
}