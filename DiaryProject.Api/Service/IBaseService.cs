using DiaryProject.Shared.Contact;

namespace DiaryProject.Api.Service;

public interface IBaseService<T, in TParameters>
{
    public Task<ApiResponse<T>> GetSingleAsync(int id, int user);

    public Task<ApiResponse<IEnumerable<T>>> GetAllAsync(TParameters parameters, int user);

    public Task<ApiResponse<T>> AddAsync(T model, int user);

    public Task<ApiResponse<T>> UpdateAsync(T model, int user);

    public Task<ApiResponse<T>> DeleteAsync(int id, int user);

    public Task<ApiResponse<string>> DeleteAllAsync(int user);
}