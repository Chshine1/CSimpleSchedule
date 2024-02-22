using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Api.Service;

// Service type
public interface IMemoService : IBaseService<MemoDto, MemoParameter>;