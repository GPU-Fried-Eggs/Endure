using Endure.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Data;

public class EndureDbContext : DbContext
{
    public DbSet<Memo>? Memos { get; set; }

    private readonly string m_connectionString;

    public EndureDbContext(string connectionString)
    {
        m_connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(m_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Memo>(); // add link to child data
    }
}