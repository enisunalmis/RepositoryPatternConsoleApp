
using Microsoft.EntityFrameworkCore;
using RepositoryPatternConsoleApp.Data;
using RepositoryPatternConsoleApp.Models;

namespace RepositoryPatternConsoleApp.Repositories
{
    // Repository Implementation
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Product> FindByName(string name)
        {
            return _context.Products
                .Where(p => p.Name.Contains(name))
                .ToList();
        }
    }
}