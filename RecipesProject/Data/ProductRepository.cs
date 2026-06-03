using RecipesProject.Models;

namespace RecipesProject.Data
{
    public class ProductRepository
    {
        private readonly DBContext _context;

        public ProductRepository(DBContext context)
        {
            _context = context;
        }

        // Получить все продукты
        public List<ProductItem> GetAllMyProducts()
        {
            var allProducts = new List<ProductItem>();

            foreach (var p in _context.PermanentProducts.ToList())
            {
                allProducts.Add(new ProductItem { Id = p.Id, Name = p.Name, IsPermanent = true });
            }

            foreach (var p in _context.MyProducts.ToList())
            {
                allProducts.Add(new ProductItem { Id = p.Id, Name = p.Name, IsPermanent = false });
            }

            return allProducts;
        }

        // Добавить продукт
        public void AddMyProduct(string name, bool isPermanent)
        {
            string nameLower = name.ToLower();

            if (isPermanent)
            {
                if (_context.PermanentProducts.Any(p => p.Name.ToLower() == nameLower))
                {
                    throw new InvalidOperationException($"Постоянный продукт '{name}' уже существует!");
                }
                _context.PermanentProducts.Add(new PermanentProduct { Name = name });
            }
            else
            {
                if (_context.MyProducts.Any(p => p.Name.ToLower() == nameLower))
                {
                    throw new InvalidOperationException($"Временный продукт '{name}' уже есть!");
                }
                _context.MyProducts.Add(new MyProduct { Name = name });
            }

            _context.SaveChanges();
        }

        // Удалить продукт
        public void RemoveMyProduct(int id, bool isPermanent)
        {
            if (isPermanent)
            {
                var product = _context.PermanentProducts.Find(id);
                if (product != null)
                {
                    _context.PermanentProducts.Remove(product);
                    _context.SaveChanges();
                    return;
                }
            }
            else
            {
                var product = _context.MyProducts.Find(id);
                if (product != null)
                {
                    _context.MyProducts.Remove(product);
                    _context.SaveChanges();
                    return;
                }
            }
            throw new InvalidOperationException($"❌ продукт с ID {id} не найден");
        }

        // Получить постоянные продукты
        public List<PermanentProduct> GetPermanentProducts()
        {
            return _context.PermanentProducts.ToList();
        }

        // Заполнить постоянными продуктами
        public void SeedPermanentProducts()
        {
            string[] products = { "Хлеб", "Крупы", "Масло", "Молоко", "Яйца", "Сахар", "Соль", "Мука", "Рис", "Макароны" };

            foreach (string name in products)
            {
                if (!_context.PermanentProducts.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    _context.PermanentProducts.Add(new PermanentProduct { Name = name });
                }
            }
            _context.SaveChanges();
        }
    }

    public class ProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsPermanent { get; set; }
    }
}
