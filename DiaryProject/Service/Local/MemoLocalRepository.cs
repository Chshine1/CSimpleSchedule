using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Service.Local;

public class MemoLocalRepository(string dbPath) : BaseLocalRepository<MemoDto>(dbPath), IMemoLocalRepository;