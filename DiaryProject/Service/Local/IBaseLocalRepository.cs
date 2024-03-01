using DiaryProject.Service.Web;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Service.Local;

public interface IBaseLocalRepository<TEntity> where TEntity : class
{
    Task<LocalResponse<TEntity>> AddAsync(TEntity entity, bool log);

    Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity, bool log);

    Task<LocalResponse<TEntity>> DeleteAsync(int id, bool log);

    Task<LocalResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<LocalResponse<List<TEntity>>> GetAllAsync();

    /// <summary>
    /// Update the local changes to the server
    /// </summary>
    /// <param name="webService">Provider of the server services</param>
    void UpdateChanges(IBaseService<TEntity, MemoParameter> webService);

    /// <summary>
    /// Get the the version of the last synchronization
    /// </summary>
    int GetVersion();
}