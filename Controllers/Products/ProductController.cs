using Catalog.Domain;
using Catalog.Repository;

namespace Catalog.Controllers.Products
{
    public class ProductController : IProductController
    {
        private readonly IProductRepository<Product> _productRepository;

        public ProductController(IProductRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Product? GetByName(string name)
        {
            return _productRepository.GetByName(name);
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public void Add(Product product)
        {
            _productRepository.Add(product);
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }

        public void Delete(Product product)
        {
            _productRepository.Delete(product);
        }

        public IEnumerable<Product> GetProductsByStoreId(int storeId)
        {
            return _productRepository.GetProductsByStoreId(storeId);
        }

        public IEnumerable<Product> GetCheapestProductsByStoreForItem(string itemName)
        {
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    throw new ArgumentException("Item name cannot be null or empty");
                }

                var allProducts = _productRepository.GetAll();
                var cheapestProducts = allProducts.Where(p => p.ProductName == itemName).OrderBy(p => p.Price).Take(1);
                return cheapestProducts;
            }

        }

        public decimal PurchaseProductsFromStore(int storeId, IDictionary<string, int> productsToPurchase)
        {
            if (storeId <= 0)
            {
                throw new ArgumentException("Store ID must be a positive integer");
            }
            if (productsToPurchase == null || productsToPurchase.Count == 0)
            {
                throw new ArgumentException("Products to purchase cannot be null or empty");
            }

            decimal totalPrice = 0;

            foreach (var productEntry in productsToPurchase)
            {
                string productName = productEntry.Key;
                int quantity = productEntry.Value;

                var product = _productRepository.GetProductByNameAndStoreId(productName, storeId);
                if (product != null && product.StoreId == storeId)
                {
                    totalPrice += product.Price * quantity;
                }
            }

            return totalPrice;
        }

        public IEnumerable<Product> GetAffordableProductsByStoreForAmount(int storeId, decimal amount)
        {
            if (storeId <= 0)
            {
                throw new ArgumentException("Store ID must be a positive integer");
            }
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative");
            }

            var affordableProducts = _productRepository.GetProductsByStoreId(storeId).Where(p => p.Price <= amount);
            return affordableProducts;
        }


        public decimal GetTotalPriceForProductsInStore(int storeId, IDictionary<string, int> productsToPurchase)
        {
            if (storeId <= 0)
            {
                throw new ArgumentException("Store ID must be a positive integer");
            }
            if (productsToPurchase == null || productsToPurchase.Count == 0)
            {
                throw new ArgumentException("Products to purchase cannot be null or empty");
            }

            decimal totalPrice = 0;

            foreach (var productEntry in productsToPurchase)
            {
                string productName = productEntry.Key;
                int quantity = productEntry.Value;

                if (quantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative");
                }

                var product = _productRepository.GetByName(productName);
                if (product != null && product.StoreId == storeId)
                {
                    if (quantity < product.Quantity)
                    {
                        throw new ArgumentException("You cannot buy more than available.");
                    }
                    totalPrice += product.Price * quantity;
                    product.Quantity -= quantity;
                }
            }

            return totalPrice;
        }
    }
}
