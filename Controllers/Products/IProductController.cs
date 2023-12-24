using Catalog.Domain;

namespace Catalog.Controllers.Products
{
    public interface IProductController
    {
        Product? GetByName(string name);
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        IEnumerable<Product> GetProductsByStoreId(int storeId);
        IEnumerable<Product> GetCheapestProductsByStoreForItem(string itemName);
        IEnumerable<Product> GetAffordableProductsByStoreForAmount(int storeId, decimal amount);
        decimal PurchaseProductsFromStore(int storeId, IDictionary<string, int> productsToPurchase);
        decimal GetTotalPriceForProductsInStore(int storeId, IDictionary<string, int> productsToPurchase);
    }
}
