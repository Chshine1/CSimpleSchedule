using DiaryProject.Shared.Dtos;

namespace DiaryProject.Service.Local;

public class MemoLocalRepository(string dbPath) : BaseLocalRepository<MemoDto>(dbPath), IMemoLocalRepository;