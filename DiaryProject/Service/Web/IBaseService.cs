using DiaryProject.Shared.Contact;

namespace DiaryProject.Service.Web;

/// <summary>
/// 客户端用来访问云端数据库的服务基类
/// </summary>
/// <typeparam name="TEntity">需要访问对象的类型</typeparam>
public interface IBaseService<TEntity> where TEntity : class
{
    Task<ApiResponse<TEntity>> AddAsync(TEntity entity);

    Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity);

    Task<ApiResponse<TEntity>> DeleteAsync(int id);

    Task<ApiResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<ApiResponse<IList<TEntity>>> GetAllAsync();

    Task<ApiResponse<TEntity>> DeleteAllAsync();
}