using DiaryProject.Service.Web;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Service.Local;

public interface IBaseLocalRepository<TEntity> where TEntity : class
{
    Task<LocalResponse<TEntity>> AddAsync(TEntity entity);

    Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity);

    Task<LocalResponse<TEntity>> DeleteAsync(int id);

    Task<LocalResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<LocalResponse<List<TEntity>>> GetAllAsync();

    void UpdateChanges(IBaseService<TEntity, MemoParameter> webService);

    void DropLogsDatabase();
    
    int GetVersion();
}