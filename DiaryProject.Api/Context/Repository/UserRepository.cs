using Arch.EntityFrameworkCore.UnitOfWork;

namespace DiaryProject.Api.Context.Repository;

public class UserRepository(DiaryContext dbContext) : Repository<User>(dbContext), IRepository<User>;