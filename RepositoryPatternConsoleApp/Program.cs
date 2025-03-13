using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

                bool devam = true;
                while (devam)
                {
                    Console.Clear();
                    Console.WriteLine("=== Ürün Yönetim Uygulaması ===");
                    Console.WriteLine("1. Tüm Ürünleri Listele");
                    Console.WriteLine("2. Ürün Ekle");
                    Console.WriteLine("3. Ürün Güncelle");
                    Console.WriteLine("4. Ürün Sil");
                    Console.WriteLine("5. Ürün Ara");
                    Console.WriteLine("0. Çıkış");
                    Console.Write("Seçiminiz: ");

                    string? secim = Console.ReadLine();

                    switch (secim)
                    {
                        case "1":
                            TumUrunleriListele(repository);
                            break;
                        case "2":
                            UrunEkle(productService);
                            break;
                        case "3":
                            UrunGuncelle(repository);
                            break;
                        case "4":
                            UrunSil(repository);
                            break;
                        case "5":
                            UrunAra(productService);
                            break;
                        case "0":
                            devam = false;
                            break;
                        default:
                            Console.WriteLine("Geçersiz seçim!");
                            break;
                    }

                    if (devam)
                    {
                        Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void TumUrunleriListele(IProductRepository repository)
        {
            Console.WriteLine("\n=== Tüm Ürünler ===");
            var urunler = repository.GetAll();

            if (!urunler.Any())
            {
                Console.WriteLine("Hiç ürün bulunamadı.");
                return;
            }

            foreach (var urun in urunler)
            {
                Console.WriteLine($"ID: {urun.Id}, Ad: {urun.Name}, Fiyat: {urun.Price:C}, Stok: {urun.Stock}");
            }
        }

        static void UrunEkle(ProductService productService)
        {
            Console.WriteLine("\n=== Ürün Ekle ===");

            Console.Write("Ürün Adı: ");
            string? ad = Console.ReadLine();

            Console.Write("Fiyat: ");
            string? fiyatStr = Console.ReadLine();
            if (!decimal.TryParse(fiyatStr, out decimal fiyat))
            {
                Console.WriteLine("Geçersiz fiyat!");
                return;
            }

            Console.Write("Stok: ");
            string? stokStr = Console.ReadLine();
            if (!int.TryParse(stokStr, out int stok))
            {
                Console.WriteLine("Geçersiz stok miktarı!");
                return;
            }

            var urun = new Product
            {
                Name = ad ?? string.Empty, // Null kontrolü yapıyoruz
                Price = fiyat,
                Stock = stok
            };

            try
            {
                productService.CreateProduct(urun);
                Console.WriteLine("Ürün başarıyla eklendi.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        static void UrunGuncelle(IProductRepository repository)
        {
            Console.WriteLine("\n=== Ürün Güncelle ===");

            Console.Write("Güncellenecek Ürün ID: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var urun = repository.GetById(id);
            if (urun == null)
            {
                Console.WriteLine("Ürün bulunamadı!");
                return;
            }

            Console.WriteLine($"Mevcut Bilgiler: Ad: {urun.Name}, Fiyat: {urun.Price:C}, Stok: {urun.Stock}");

            Console.Write("Yeni Ürün Adı (boş bırakırsanız değişmez): ");
            string? ad = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ad))
            {
                urun.Name = ad;
            }

            Console.Write("Yeni Fiyat (boş bırakırsanız değişmez): ");
            string? fiyatStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fiyatStr) && decimal.TryParse(fiyatStr, out decimal fiyat))
            {
                urun.Price = fiyat;
            }

            Console.Write("Yeni Stok (boş bırakırsanız değişmez): ");
            string? stokStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(stokStr) && int.TryParse(stokStr, out int stok))
            {
                urun.Stock = stok;
            }

            repository.Update(urun);
            Console.WriteLine("Ürün başarıyla güncellendi.");
        }

        static void UrunSil(IProductRepository repository)
        {
            Console.WriteLine("\n=== Ürün Sil ===");

            Console.Write("Silinecek Ürün ID: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var urun = repository.GetById(id);
            if (urun == null)
            {
                Console.WriteLine("Ürün bulunamadı!");
                return;
            }

            Console.WriteLine($"'{urun.Name}' isimli ürünü silmek istediğinize emin misiniz? (E/H)");
            string? onay = Console.ReadLine()?.ToUpper();

            if (onay == "E")
            {
                repository.Delete(id);
                Console.WriteLine("Ürün başarıyla silindi.");
            }
            else
            {
                Console.WriteLine("Silme işlemi iptal edildi.");
            }
        }

        static void UrunAra(ProductService productService)
        {
            Console.WriteLine("\n=== Ürün Ara ===");

            Console.Write("Arama Kelimesi: ");
            string? arama = Console.ReadLine();

            var sonuclar = productService.SearchProducts(arama ?? string.Empty);

            if (!sonuclar.Any())
            {
                Console.WriteLine("Aramanızla eşleşen ürün bulunamadı.");
                return;
            }

            Console.WriteLine("\nArama Sonuçları:");
            foreach (var urun in sonuclar)
            {
                Console.WriteLine($"ID: {urun.Id}, Ad: {urun.Name}, Fiyat: {urun.Price:C}, Stok: {urun.Stock}");
            }
        }
    }

    // 1. Entity (Varlık)
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Default değer atayarak nullability uyarısını engelliyoruz
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    // 2. DbContext
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!; // null forgiving operator
    }

    // 3. Repository Interface
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id); // Nullable dönüş tipi
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        IEnumerable<Product> FindByName(string name);
    }

    // 4. Repository Implementation
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

        public Product? GetById(int id) // Nullable dönüş tipi
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

    // 5. Service
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