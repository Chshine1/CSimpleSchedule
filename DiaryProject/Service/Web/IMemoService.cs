using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Service.Web;

// Service type
public interface IMemoService : IBaseService<MemoDto, MemoParameter>;