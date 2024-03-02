using System.Diagnostics.CodeAnalysis;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Service.Web;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MemoService(HttpRestClient client) : BaseService<MemoDto>(client, "Memo"), IMemoService;