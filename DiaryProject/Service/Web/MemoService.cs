using System.Diagnostics.CodeAnalysis;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;

namespace DiaryProject.Service.Web;

// Implementation type
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MemoService(HttpRestClient client) : BaseService<MemoDto, MemoParameter>(client, "Memo"), IMemoService;