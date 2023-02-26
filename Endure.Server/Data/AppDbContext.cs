using Endure.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Memo>? Memos { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Memo>();
    }
}