using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Service.Local;

public interface IBaseLocalRepository<TEntity> where TEntity : class
{
    Task<LocalResponse<TEntity>> AddAsync(TEntity entity);

    Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity);

    Task<LocalResponse<TEntity>> DeleteAsync(int id);

    Task<LocalResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<LocalResponse<List<TEntity>>> GetAllAsync();

    void DropLogsDatabase();

    Task<List<DatabaseLogDto>> GetLogs();
}