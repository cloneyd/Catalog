namespace Catalog.Repository
{
    public interface IProductRepository<Product>
    {
        Product? GetByName(string name);
        Product? GetProductByNameAndStoreId(string productName, int storeId);
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        IEnumerable<Product> GetProductsByStoreId(int storeId);
    }
}
