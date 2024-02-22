using DiaryProject.Shared.Contact;

namespace DiaryProject.Service.Web;

/// <summary>
/// Base service for the client to access the database
/// </summary>
/// <typeparam name="TEntity">Type of the entity to be accessed</typeparam>
/// <typeparam name="TParameter">Parameters for query</typeparam>
public interface IBaseService<TEntity, in TParameter> where TEntity : class
{
    Task<ApiResponse<TEntity>> AddAsync(TEntity entity);

    Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity);

    Task<ApiResponse<TEntity>> DeleteAsync(int id);

    Task<ApiResponse<TEntity>> GetFirstOrDefaultAsync(int id);

    Task<ApiResponse<IList<TEntity>>> GetAllAsync(TParameter parameter);
}