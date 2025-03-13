
using RepositoryPatternConsoleApp.Models;
using RepositoryPatternConsoleApp.Repositories;

namespace RepositoryPatternConsoleApp.Services
{
    // Service
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public void CreateProduct(Product product)
        {
            // İş mantığı kontrolleri
            if (product.Price <= 0)
                throw new ArgumentException("Ürün fiyatı sıfırdan büyük olmalıdır");

            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("Ürün adı boş olamaz");

            _repository.Add(product);
        }

        public IEnumerable<Product> SearchProducts(string keyword)
        {
            return string.IsNullOrWhiteSpace(keyword)
                ? _repository.GetAll()
                : _repository.FindByName(keyword);
        }
    }
}