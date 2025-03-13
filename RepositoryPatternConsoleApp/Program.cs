
using Microsoft.EntityFrameworkCore;
using RepositoryPatternConsoleApp.Data;
using RepositoryPatternConsoleApp.Repositories;
using RepositoryPatternConsoleApp.Services;
using RepositoryPatternConsoleApp.UI;

namespace RepositoryPatternConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // DbContext options
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProductDb;Trusted_Connection=True;")
                .Options;

            // DbContext oluştur
            using (var dbContext = new AppDbContext(options))
            {
                // Veritabanını oluştur
                dbContext.Database.EnsureCreated();

                // Repository oluştur
                IProductRepository repository = new ProductRepository(dbContext);

                // ProductService oluştur
                var productService = new ProductService(repository);

                // ConsoleUI oluştur ve çalıştır
                var consoleUI = new ConsoleUI(repository, productService);
                consoleUI.Run();
            }
        }
    }
}