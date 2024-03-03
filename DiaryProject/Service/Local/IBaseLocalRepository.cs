using DiaryProject.Service.Web;
using DiaryProject.Shared.Contact;

namespace DiaryProject.Service.Local;

public interface IBaseLocalRepository<TEntity> where TEntity : class
{
    Task<LocalResponse<TEntity>> AddAsync(TEntity entity, bool log);

    Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity, bool log);

    Task<LocalResponse<TEntity>> DeleteAsync(int id, bool log);

    Task<LocalResponse<string>> DeleteAllAsync();

    Task<LocalResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<LocalResponse<List<TEntity>>> GetAllAsync();

    /// <summary>
    /// 将本地未同步的变化上传到服务器以再次同步
    /// </summary>
    /// <param name="webService">服务器服务的提供者</param>
    void UpdateChanges(IBaseService<TEntity> webService);
}