using Catalog.Domain;
using Catalog.Repository;
using Npgsql;

public class PostgreProductRepository : IProductRepository<Product>
{
    private readonly string _connectionString;
    private readonly NpgsqlConnection _connection;

    public PostgreProductRepository(string connectionString)
    {
        _connectionString = connectionString;
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();
    }

    public Product? GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Product name cannot be null or empty");
        }

        using (var cmd = new NpgsqlCommand("SELECT * FROM Products WHERE ProductName = @name", _connection))
        {
            cmd.Parameters.AddWithValue("name", name);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Product
                    {
                        ProductName = reader.GetString(0),
                        StoreId = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDecimal(3)
                    };
                }
                return null;
            }
        }
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

        using (var cmd = new NpgsqlCommand("SELECT * FROM Products WHERE StoreId = @storeId AND ProductName = @productName", _connection))
        {
            cmd.Parameters.AddWithValue("storeId", storeId);
            cmd.Parameters.AddWithValue("productName", productName);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new Product
                    {
                        ProductName = reader.GetString(0),
                        StoreId = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDecimal(3)
                    };
                }
                return null;
            }
        }
    }

    public IEnumerable<Product> GetAll()
    {
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM Products", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var products = new List<Product>();
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductName = reader.GetString(0),
                            StoreId = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2),
                            Price = reader.GetDecimal(3)
                        });
                    }
                    return products;
                }
            }
        }
    }

    public void Add(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        if (string.IsNullOrEmpty(product.ProductName))
        {
            throw new ArgumentException("Product name cannot be null or empty");
        }
        if (product.Quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative");
        }
        if (product.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }

        using (var cmd = new NpgsqlCommand("INSERT INTO Products (ProductName, StoreId, Quantity, Price) VALUES (@productName, @storeId, @quantity, @price)", _connection))
        {
            cmd.Parameters.AddWithValue("productName", product.ProductName);
            cmd.Parameters.AddWithValue("storeId", product.StoreId);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.ExecuteNonQuery();
        }
    }

    public void Update(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        if (string.IsNullOrEmpty(product.ProductName))
        {
            throw new ArgumentException("Product name cannot be null or empty");
        }
        if (product.Quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative");
        }
        if (product.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }

        using (var cmd = new NpgsqlCommand("UPDATE Products SET Quantity = @quantity, Price = @price WHERE ProductName = @productName AND StoreId = @storeId", _connection))
        {
            cmd.Parameters.AddWithValue("productName", product.ProductName);
            cmd.Parameters.AddWithValue("storeId", product.StoreId);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        if (string.IsNullOrEmpty(product.ProductName))
        {
            throw new ArgumentException("Product name cannot be null or empty");
        }

        using (var cmd = new NpgsqlCommand("DELETE FROM Products WHERE ProductName = @productName AND StoreId = @storeId", _connection))
        {
            cmd.Parameters.AddWithValue("productName", product.ProductName);
            cmd.Parameters.AddWithValue("storeId", product.StoreId);
            cmd.ExecuteNonQuery();
        }
    }

    public IEnumerable<Product> GetProductsByStoreId(int storeId)
    {
        if (storeId <= 0)
        {
            throw new ArgumentException("Store ID must be a positive integer");
        }

        using (var cmd = new NpgsqlCommand("SELECT * FROM Products WHERE StoreId = @storeId", _connection))
        {
            cmd.Parameters.AddWithValue("storeId", storeId);
            using (var reader = cmd.ExecuteReader())
            {
                var products = new List<Product>();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductName = reader.GetString(0),
                        StoreId = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDecimal(3)
                    });
                }
                return products;
            }
        }
    }

    public IEnumerable<Product> GetCheapestProductsByStoreForItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            throw new ArgumentException("Item name cannot be null or empty");
        }
       
        using (var cmd = new NpgsqlCommand("SELECT * FROM Products WHERE ProductName = @itemName ORDER BY Price ASC", _connection))
        {
            cmd.Parameters.AddWithValue("itemName", itemName);
            using (var reader = cmd.ExecuteReader())
            {
                var products = new List<Product>();
                if (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductName = reader.GetString(0),
                        StoreId = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDecimal(3)
                    });
            }
            return products;
            }
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

        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                foreach (var productEntry in productsToPurchase)
                {
                    string productName = productEntry.Key;
                    int quantity = productEntry.Value;

                    using (var cmd = new NpgsqlCommand("SELECT Price FROM Products WHERE StoreId = @storeId AND ProductName = @productName", _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("storeId", storeId);
                        cmd.Parameters.AddWithValue("productName", productName);
                        var price = cmd.ExecuteScalar();
                        if (price == null)
                        {
                            throw new InvalidOperationException($"Product '{productName}' not found in store with ID {storeId}");
                        }
                        totalPrice += (decimal)price * quantity;
                    }
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
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

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT * FROM Products WHERE StoreId = @storeId AND Price <= @amount", connection))
            {
                cmd.Parameters.AddWithValue("storeId", storeId);
                cmd.Parameters.AddWithValue("amount", amount);
                using (var reader = cmd.ExecuteReader())
                {
                    var products = new List<Product>();
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductName = reader.GetString(0),
                            StoreId = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2),
                            Price = reader.GetDecimal(3)
                        });
                    }
                    return products;
                }
            }
        }
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

        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                foreach (var productEntry in productsToPurchase)
                {
                    string productName = productEntry.Key;
                    int quantity = productEntry.Value;

                    using (var cmd = new NpgsqlCommand("SELECT Price FROM Products WHERE StoreId = @storeId AND ProductName = @productName", _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("storeId", storeId);
                        cmd.Parameters.AddWithValue("productName", productName);
                        var price = cmd.ExecuteScalar();
                        if (price == null)
                        {
                            throw new InvalidOperationException($"Product '{productName}' not found in store with ID {storeId}");
                        }
                        totalPrice += (decimal)price * quantity;
                    }
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        return totalPrice;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
