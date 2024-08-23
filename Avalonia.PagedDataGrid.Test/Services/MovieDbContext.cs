using Avalonia.PagedDataGrid.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Avalonia.PagedDataGrid.Test.Services;

public class MovieDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Data Source=MovieDb.db");
    }
}
