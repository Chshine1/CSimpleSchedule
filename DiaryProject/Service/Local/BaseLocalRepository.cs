using System.IO;
using DiaryProject.Shared;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Utils;
using SQLite;

namespace DiaryProject.Service.Local;

public class BaseLocalRepository<TEntity> : IBaseLocalRepository<TEntity> where TEntity : BaseDto, new()
{
    private readonly SQLiteAsyncConnection _connection;

    protected BaseLocalRepository(string dbPath)
    {
        if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
        dbPath += "\\save.db";
        _connection = new SQLiteAsyncConnection(dbPath);
        _connection.CreateTableAsync<TEntity>();
        _connection.CreateTableAsync<DatabaseLogDto>();
    }
    
    public async Task<LocalResponse<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            await _connection.InsertAsync(entity);
            await AddLogEntry(entity.Id, DatabaseOperation.Add);
            return new LocalResponse<TEntity> { Status = true, Result = entity };
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity)
    {
        //if (_connection == null) return new LocalResponse<TEntity> { Status = false, Message = "Missing database" };
        try
        {
            var target = await _connection.GetAsync<TEntity>(predicate: e => e.Id.Equals(entity.Id));
            target.CopyFrom(entity);
            await _connection.UpdateAsync(target, typeof(TEntity));
            await AddLogEntry(entity.Id, DatabaseOperation.Update);
            return new LocalResponse<TEntity> { Status = true, Result = target };
        }
        catch (InvalidOperationException)
        {
            return await AddAsync(entity);
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<TEntity>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _connection.GetAsync<TEntity>(predicate: e => e.Id.Equals(id));
            await _connection.DeleteAsync<TEntity>(id);
            await AddLogEntry(id, DatabaseOperation.Delete);
            return new LocalResponse<TEntity> { Status = true, Result = entity };
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<TEntity>> GetFirstOrDefaultAsync(int id)
    {
        try
        {
            var entity = await _connection.GetAsync<TEntity>(id);
            return new LocalResponse<TEntity> { Status = true, Result = entity };
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<List<TEntity>>> GetAllAsync()
    {
        try
        {
            var query = await _connection.QueryAsync<TEntity>("SELECT * FROM memos");
            return new LocalResponse<List<TEntity>> { Status = true, Result = query };
        }
        catch (Exception e)
        {
            return new LocalResponse<List<TEntity>> { Status = false, Message = e.Message };
        }
    }

    private async Task AddLogEntry(int entityId, DatabaseOperation operation)
    {
        DatabaseLogDto? log = null;
        try
        {
            log = await _connection.FindWithQueryAsync<DatabaseLogDto>($"SELECT * FROM logs WHERE EntityId={entityId}");
        }
        catch (Exception)
        {
            // Ignored
        }

        if (log == null)
        {
            await _connection.InsertAsync(new DatabaseLogDto
            {
                EntityId = entityId, Operation = (int)operation, Exists = operation != DatabaseOperation.Add,
                UpdateTime = DateTime.Now
            });
            return;
        }

        switch (operation)
        {
            case DatabaseOperation.Add:
                log.Operation = !log.Exists || log.Operation == 2 ? 0 : 1;
                break;
            case DatabaseOperation.Update:
                log.Operation = !log.Exists ? 0 : 1;
                break;
            case DatabaseOperation.Delete:
                log.Operation = !log.Exists ? 3 : 2;
                break;
            case DatabaseOperation.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }
        log.UpdateTime = DateTime.Now;
        await _connection.UpdateAsync(log);
    }

    public async void DropLogsDatabase()
    {
        await _connection.ExecuteAsync("DELETE FROM logs");
        await _connection.ExecuteAsync("UPDATE sqlite_sequence SET seq=1 WHERE name=\"logs\"");
    }

    public async Task<List<DatabaseLogDto>> GetLogs()
    {
        return await _connection.QueryAsync<DatabaseLogDto>("SELECT * FROM logs ORDER BY UpdateTime");
    }
}