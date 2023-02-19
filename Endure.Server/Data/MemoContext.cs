using Endure.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Data;

public class MemoContext : DbContext
{
    public DbSet<Memo>? Memos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Endure");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Memo>(); // add link to child data
    }
}