using System.Diagnostics.CodeAnalysis;
using Arch.EntityFrameworkCore.UnitOfWork;

namespace DiaryProject.Api.Context.Repository;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MemoRepository([SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")] DiaryContext dbContext) : Repository<Memo>(dbContext), IRepository<Memo>;