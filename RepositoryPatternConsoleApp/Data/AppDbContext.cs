using Microsoft.EntityFrameworkCore;
using RepositoryPatternConsoleApp.Models;

namespace RepositoryPatternConsoleApp.Data
{
    // DbContext
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
    }
}