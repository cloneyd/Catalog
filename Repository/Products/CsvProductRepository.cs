using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Catalog.Domain;
using Catalog.Repository;

public class ProductMap : ClassMap<Product>
{
    public ProductMap()
    {
        Map(p => p.ProductName).Name("ProductName");
        Map(p => p.StoreId).Name("StoreId");
        Map(p => p.Quantity).Name("Quantity");
        Map(p => p.Price).Name("Price");
    }
}

public class CsvProductRepository : IProductRepository<Product>
{
    private readonly string _csvFilePath;
    private readonly object _fileLock = new object();
    private CsvReader _csvReader;
    private CsvWriter _csvWriter;

    public CsvProductRepository(string csvFilePath)
    {
        _csvFilePath = csvFilePath;
        var reader = new StreamReader(_csvFilePath);
        _csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        _csvWriter = new CsvWriter(new StreamWriter(_csvFilePath, append: true), CultureInfo.InvariantCulture);
    }

    public Product? GetByName(string name)
    {
        _csvReader.Context.RegisterClassMap<ProductMap>();
        var records = _csvReader.GetRecords<Product>();
        return records.FirstOrDefault(p => p.ProductName == name);
    }

    public Product? GetProductByNameAndStoreId(string productName, int storeId)
    {
        if (storeId <= 0)
        {
            throw new ArgumentException("Store ID must be a positive integer");
        }
        if (string.IsNullOrEmpty(productName))
        {
            throw new ArgumentException("ProductName must be filled");
        }

        var products = _csvReader.GetRecords<Product>().Where(p => p.StoreId == storeId).Where(p => p.ProductName == productName);
        if (products.Count() > 0)
        {
            return products.ElementAt(0);
        }

        return null;
    }

    public IEnumerable<Product> GetAll()
    {
        _csvReader.Context.RegisterClassMap<ProductMap>();
        return _csvReader.GetRecords<Product>().ToList();
    }

    public void Add(Product product)
    {
        product.Check();

        lock (_fileLock)
        {
            _csvWriter.WriteRecord(product);
            _csvWriter.Flush();
        }
    }

    public void Update(Product product)
    {
        product.Check();

        lock (_fileLock)
        {
            var products = GetAll().ToList();
            var existingProduct = products.FirstOrDefault(p => p.ProductName == product.ProductName && p.StoreId == product.StoreId);
            if (existingProduct != null)
            {
                products.Remove(existingProduct);
                products.Add(product);
                WriteAllRecords(products);
            }
        }
    }

    public void Delete(Product product)
    {
        lock (_fileLock)
        {
            var products = GetAll().ToList();
            var existingProduct = products.FirstOrDefault(p => p.ProductName == product.ProductName && p.StoreId == product.StoreId);
            if (existingProduct != null)
            {
                products.Remove(existingProduct);
                WriteAllRecords(products);
            }
        }
    }

    public IEnumerable<Product> GetProductsByStoreId(int storeId)
    {
        if (storeId <= 0)
        {
            throw new ArgumentException("Store ID must be a positive integer");
        }

        var products = _csvReader.GetRecords<Product>().Where(p => p.StoreId == storeId);
        return products.ToList();
    }

    private void WriteAllRecords(IEnumerable<Product> products)
    {
        _csvWriter.Context.RegisterClassMap<ProductMap>();
        _csvWriter.WriteRecords(products);
        _csvWriter.Flush();
    }

    public void Dispose()
    {
        _csvReader.Dispose();
        _csvWriter.Dispose();
    }
}
