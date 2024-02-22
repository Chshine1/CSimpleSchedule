using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DiaryProject.Api.Context;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class DiaryContext(DbContextOptions<DiaryContext> options) : DbContext(options)
{
    public DbSet<Memo>? Memo { get; set; }

    public DbSet<User>? User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Memo>()
            .Property(m => m.UserId)
            .HasDefaultValue(1);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Memos)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .IsRequired();
    }
}