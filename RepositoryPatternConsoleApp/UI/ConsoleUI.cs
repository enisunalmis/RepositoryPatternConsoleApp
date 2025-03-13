using RepositoryPatternConsoleApp.Models;
using RepositoryPatternConsoleApp.Repositories;
using RepositoryPatternConsoleApp.Services;

namespace RepositoryPatternConsoleApp.UI
{
    // Console UI
    public class ConsoleUI
    {
        private readonly IProductRepository _repository;
        private readonly ProductService _productService;

        public ConsoleUI(IProductRepository repository, ProductService productService)
        {
            _repository = repository;
            _productService = productService;
        }

        public void Run()
        {
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
                        TumUrunleriListele();
                        break;
                    case "2":
                        UrunEkle();
                        break;
                    case "3":
                        UrunGuncelle();
                        break;
                    case "4":
                        UrunSil();
                        break;
                    case "5":
                        UrunAra();
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

        private void TumUrunleriListele()
        {
            Console.WriteLine("\n=== Tüm Ürünler ===");
            var urunler = _repository.GetAll();

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

        private void UrunEkle()
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
                Name = ad ?? string.Empty,
                Price = fiyat,
                Stock = stok
            };

            try
            {
                _productService.CreateProduct(urun);
                Console.WriteLine("Ürün başarıyla eklendi.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        private void UrunGuncelle()
        {
            Console.WriteLine("\n=== Ürün Güncelle ===");

            Console.Write("Güncellenecek Ürün ID: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var urun = _repository.GetById(id);
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

            _repository.Update(urun);
            Console.WriteLine("Ürün başarıyla güncellendi.");
        }

        private void UrunSil()
        {
            Console.WriteLine("\n=== Ürün Sil ===");

            Console.Write("Silinecek Ürün ID: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var urun = _repository.GetById(id);
            if (urun == null)
            {
                Console.WriteLine("Ürün bulunamadı!");
                return;
            }

            Console.WriteLine($"'{urun.Name}' isimli ürünü silmek istediğinize emin misiniz? (E/H)");
            string? onay = Console.ReadLine()?.ToUpper();

            if (onay == "E")
            {
                _repository.Delete(id);
                Console.WriteLine("Ürün başarıyla silindi.");
            }
            else
            {
                Console.WriteLine("Silme işlemi iptal edildi.");
            }
        }

        private void UrunAra()
        {
            Console.WriteLine("\n=== Ürün Ara ===");

            Console.Write("Arama Kelimesi: ");
            string? arama = Console.ReadLine();

            var sonuclar = _productService.SearchProducts(arama ?? string.Empty);

            if (!sonuclar.Any())
            {
                Console.WriteLine("Aramanızla eşleşen ürün bulunamadı.");
                return;
            }

            Console.WriteLine("\nArama Sonuçları:");
            foreach (var urun in sonuclar)
            {      
                Console.WriteLine($"ID: {urun.Id}, Ad: {urun.Name}, Fiyat: {urun.Price.ToString("N2") + " TL"}, Stok: {urun.Stock}");
            }
        }
    }
}