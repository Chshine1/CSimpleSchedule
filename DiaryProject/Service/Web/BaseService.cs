using DiaryProject.Shared.Contact;
using RestSharp;

namespace DiaryProject.Service.Web;

public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
{
    private readonly HttpRestClient _client;
    private readonly string _serviceName;

    protected BaseService(HttpRestClient client, string serviceName)
    {
        _client = client;
        _serviceName = serviceName;
    }

    public async Task<ApiResponse<TEntity>> AddAsync(TEntity entity)
    {
        var request = new BaseRequest
        {
            Method = Method.Post,
            Route = $"api/{_serviceName}/Add",
            Parameter = entity
        };
        return await _client.ExecuteAsync<TEntity>(request);
    }

    public async Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity)
    {
        var request = new BaseRequest
        {
            Method = Method.Post,
            Route = $"api/{_serviceName}/Update",
            Parameter = entity
        };
        return await _client.ExecuteAsync<TEntity>(request);
    }

    public async Task<ApiResponse<TEntity>> DeleteAsync(int id)
    {
        var request = new BaseRequest
        {
            Method = Method.Delete,
            Route = $"api/{_serviceName}/Delete?id={id}"
        };
        return await _client.ExecuteAsync<TEntity>(request);
    }

    public async Task<ApiResponse<TEntity>> GetFirstOrDefaultAsync(int id)
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = $"api/{_serviceName}/GetSingle?id={id}"
        };
        return await _client.ExecuteAsync<TEntity>(request);
    }

    public async Task<ApiResponse<IList<TEntity>>> GetAllAsync()
    {
        var request = new BaseRequest
        {
            Method = Method.Get,
            Route = $"api/{_serviceName}/GetAll"
        };
        return await _client.ExecuteAsync<IList<TEntity>>(request);
    }

    public async Task<ApiResponse<TEntity>> DeleteAllAsync()
    {
        var request = new BaseRequest
        {
            Method = Method.Delete,
            Route = $"api/{_serviceName}/DeleteAll"
        };
        return await _client.ExecuteAsync<TEntity>(request);
    }

    //TODO
    public int GetVersion()
    {
        return 0;
    }
}