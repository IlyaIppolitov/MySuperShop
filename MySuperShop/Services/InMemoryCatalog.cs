using MySuperShop.Interfaces;
using MySuperShop.Models;
using System.Collections.Concurrent;

namespace MySuperShop.Services
{
    public class InMemoryCatalog : ICatalog
	{
		/// <summary> Величина скидки по понедельникам </summary>
		private decimal _mondaySale = 0.7m;

		/// <summary>
		/// Получить коллекцию товаров
		/// </summary>
		/// <returns>Коллекция товаров</returns>
		public List<Product> GetProducts(ICurrentTime curTime)
		{
			if (curTime.getCurrentTime().DayOfWeek != DayOfWeek.Monday)
			{
				return _products.Values.ToList();
			}

			List<Product> saleProducts = new List<Product>(_products.Values.ToList().Count);

			_products.Values.ToList().ForEach((item) =>
			{
				saleProducts.Add((Product)item.Clone());
				saleProducts.Last().Price *= _mondaySale;
			});

			return saleProducts;
		}
		async public Task<List<Product>> GetProductsAsync(ICurrentTime curTime)
		{
			if (curTime.getCurrentTime().DayOfWeek != DayOfWeek.Monday)
			{
				return _products.Values.ToList();
			}

			List<Product> saleProducts = new List<Product>(_products.Values.ToList().Count);

			_products.Values.ToList().ForEach((item) =>
			{
				saleProducts.Add((Product)item.Clone());
				saleProducts.Last().Price *= _mondaySale;
			});

			return saleProducts;

		}

		/// <summary>
		/// Получить товар по значению ID
		/// </summary>
		/// <param name="productId"> ID </param>
		/// <returns>Товар</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Product GetProductById(Guid productId, ICurrentTime curTime)
		{
			if (_products.TryGetValue(productId, out var product))
			{
				if (curTime.getCurrentTime().DayOfWeek != DayOfWeek.Monday)
				{
					return product;
				}

				var saleProduct = (Product)product.Clone();
				saleProduct.Price *= _mondaySale;
				return saleProduct;
			}
			throw new ArgumentOutOfRangeException($"Product with ID: {productId} not found");
		}

		/// <summary>
		/// Добавить товар
		/// </summary>
		/// <param name="product">Товар</param>
		public void AddProduct(Product product)
		{
			if (!_products.TryAdd(product.Id, product))
				throw new ArgumentException($"Product with ID {product.Id} already exists");
		}

		/// <summary>
		/// Удалить товар
		/// </summary>
		/// <param name="productId">Id товара</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void DeleteProduct(Guid productId)
		{
			if (!_products.TryRemove(productId, out var ignored))
			{
				throw new ArgumentOutOfRangeException($"Product with ID: {productId} not found");
			}
		}

		/// <summary>
		/// Обновить информацию о товаре
		/// </summary>
		/// <param name="product">Товар</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void UpdateProduct(Product newProduct)
		{
			if (!_products.TryGetValue(newProduct.Id, out var oldProduct))
			{
				throw new ArgumentOutOfRangeException($"Product with ID: {newProduct.Id} not found");
			}

			if (_products.TryUpdate(newProduct.Id, newProduct, oldProduct))
			{
				throw new ArgumentOutOfRangeException($"Product with ID: {newProduct.Id} not found");
			}

		}

		/// <summary>
		/// Обновить информацию о товаре по его ID
		/// </summary>
		/// <param name="product">Id товара</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void UpdateProductById(Guid productId, Product newProduct)
		{
			if (!_products.TryGetValue(productId, out var oldProduct))
			{
				throw new ArgumentOutOfRangeException($"Product with ID: {newProduct.Id} not found");
			}

			if (_products.TryUpdate(productId, newProduct, oldProduct))
			{
				throw new ArgumentOutOfRangeException($"Product with ID: {newProduct.Id} not found");
			}

		}

		/// <summary>
		/// Очистить каталог
		/// </summary>
		public void Clear()
		{
			_products.Clear();
		}

		/// <summary>
		/// Перечень товаров
		/// </summary>
		/// <remarks>По умолчанию создается 5 случайных товаров</remarks>
		private ConcurrentDictionary<Guid, Product> _products = GenerateProducts(5);

		/// <summary>
		/// Создать коллекцию продуктов
		/// </summary>
		/// <param name="count">Количество продуктов</param>
		/// <returns>Коллекция продуктов</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		static ConcurrentDictionary<Guid, Product> GenerateProducts(int count)
		{


			// Массив реальных названий товаров
			var productNamesPics = new NameAndPic[]
			{
				new NameAndPic("Молоко", "milk.jpg"),
				new NameAndPic("Хлеб", "bread.jpg"),
				new NameAndPic("Яблоки", "apples.jpg"),
				new NameAndPic("Макароны", "macaron.jpeg"),
				new NameAndPic("Сахар", "sugar.jpg"),
				new NameAndPic("Кофе", "kofe.jpg"),
				new NameAndPic("Чай", "tea.jpeg"),
				new NameAndPic("Рис", "rice.jpg"),
				new NameAndPic("Масло подсолнечное", "oil.jpeg"),
				new NameAndPic("Сыр", "cheese.jpg"),
			};

			// Массив реальных названий товаров
			var productNames = new string[]
			{
			"Молоко",
			"Хлеб",
			"Яблоки",
			"Макароны",
			"Сахар",
			"Кофе",
			"Чай",
			"Рис",
			"Масло подсолнечное",
			"Сыр"
			};

			// Проверка введеного количества товаров на соответствие с количеством имеющихся имён
			if (count > productNames.Length)
				throw new ArgumentOutOfRangeException(
					$"Wrong number of products, select less then {productNames.Length}");

			var random = new Random();
			var products = new ConcurrentDictionary<Guid, Product>();

			for (int i = 0; i < count; i++)
			{
				var name = productNamesPics[i].name;
				var price = random.Next(50, 500);
				var producedAt = DateTime.Now.AddDays(-random.Next(1, 30));
				var expiredAt = producedAt.AddDays(random.Next(1, 365));
				var stock = Math.Round(random.NextDouble() * 100);

				var product = new Product(name, price);
				product.Id = i == 0 ? Guid.Empty : Guid.NewGuid();
				product.Pic = productNamesPics[i].pic;
				product.Description = "Описание " + name;
				product.Stock = stock;
				products.TryAdd(product.Id, product);
			}

			return products;
		}

	}

	struct NameAndPic
	{
		public string name;
		public string pic;
		public NameAndPic(string name, string pic)
		{
			this.name = name;
			this.pic = pic;
		}
	};
}
