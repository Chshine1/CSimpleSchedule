using System.IO;
using DiaryProject.Service.Web;
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
    
    public async Task<LocalResponse<TEntity>> AddAsync(TEntity entity, bool log)
    {
        try
        {
            await _connection.InsertAsync(entity);
            if (log) await WriteLogEntry(entity.Id, DatabaseOperation.Add);
            return new LocalResponse<TEntity> { Status = true, Result = entity };
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<TEntity>> UpdateAsync(TEntity entity, bool log)
    {
        try
        {
            var target = await _connection.GetAsync<TEntity>(predicate: e => e.Id.Equals(entity.Id));
            target.CopyFrom(entity);
            
            await _connection.UpdateAsync(target, typeof(TEntity));
            if (log) await WriteLogEntry(target.Id, DatabaseOperation.Update);
            return new LocalResponse<TEntity> { Status = true, Result = target };
        }
        catch (InvalidOperationException)
        {
            return await AddAsync(entity, log);
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<TEntity>> DeleteAsync(int id, bool log)
    {
        try
        {
            var entity = await _connection.GetAsync<TEntity>(predicate: e => e.Id.Equals(id));
            
            await _connection.DeleteAsync<TEntity>(id);
            if (log) await WriteLogEntry(id, DatabaseOperation.Delete);
            return new LocalResponse<TEntity> { Status = true, Result = entity };
        }
        catch (Exception e)
        {
            return new LocalResponse<TEntity> { Status = false, Message = e.Message };
        }
    }

    public async Task<LocalResponse<string>> DeleteAllAsync()
    {
        try
        {
            await _connection.ExecuteAsync("DELETE FROM memos");
            await _connection.ExecuteAsync("UPDATE sqlite_sequence SET seq=0 WHERE name='memos'");
            return new LocalResponse<string> { Status = true };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
    
    public async void UpdateChangesAsync(IBaseService<TEntity> webService)
    {
        /*var logs = await _connection.QueryAsync<DatabaseLogDto>("SELECT * FROM logs ORDER BY UpdateTime asc");
        foreach (var log in logs)
        {
            switch (log.Operation)
            {
                case 0:
                    await webService.AddAsync(await _connection.FindAsync<TEntity>(log.EntityId));
                    break;
                case 1:
                    await webService.UpdateAsync(await _connection.FindAsync<TEntity>(log.EntityId));
                    break;
                case 2:
                    await webService.DeleteAsync(log.EntityId);
                    break;
                default:
                    continue;
            }
        }
        await _connection.ExecuteAsync("DELETE FROM logs");
        await _connection.ExecuteAsync("UPDATE sqlite_sequence SET seq=1 WHERE name=\"logs\"");*/
        await webService.DeleteAllAsync();
        var list = (await GetAllAsync()).Result;
        if (list == null || list.Count == 0) return;
        foreach (var entity in list)
        {
            await webService.AddAsync(entity);
        }
    }

    //TODO:
    public int GetVersion()
    {
        return 0;
    }

    /// <summary>
    /// 对一个实体执行某些操作时，记录到日志中
    /// </summary>
    /// <param name="entityId">被操作实体的主键</param>
    /// <param name="operation">被执行的操作</param>
    /// <exception cref="ArgumentOutOfRangeException">无法识别的操作</exception>
    private async Task WriteLogEntry(int entityId, DatabaseOperation operation)
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
}