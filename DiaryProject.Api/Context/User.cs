using Microsoft.EntityFrameworkCore;

namespace DiaryProject.Api.Context;

[Index(nameof(UserName), IsUnique = true)]
public record User : BaseEntity
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public ICollection<Memo> Memos { get; }
}