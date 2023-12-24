using Catalog.Controllers.Products;
using Catalog.Domain;

namespace Catalog.Client
{
    public class ConsoleClient
    {
        private readonly IProductController _productController;

        public ConsoleClient(IProductController productController)
        {
            _productController = productController;
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("1. Get product by name");
                Console.WriteLine("2. Get all products");
                Console.WriteLine("3. Add product");
                Console.WriteLine("4. Update product");
                Console.WriteLine("5. Delete product");
                Console.WriteLine("6. Get products by store ID");
                Console.WriteLine("7. Get cheapest products by store for item");
                Console.WriteLine("8. Purchase products from store");
                Console.WriteLine("9. Get affordable products by store for amount");
                Console.WriteLine("10. Get total price for products in store");
                Console.WriteLine("0. Exit");

                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        GetProductByName();
                        break;
                    case "2":
                        GetAllProducts();
                        break;
                    case "3":
                        AddProduct();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        DeleteProduct();
                        break;
                    case "6":
                        GetProductsByStoreId();
                        break;
                    case "7":
                        GetCheapestProductsByStoreForItem();
                        break;
                    case "8":
                        PurchaseProductsFromStore();
                        break;
                    case "9":
                        GetAffordableProductsByStoreForAmount();
                        break;
                    case "10":
                        GetTotalPriceForProductsInStore();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }

        private void GetProductByName()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();
            try
            {
                Product product = _productController.GetByName(name);
                if (product != null)
                {
                    Console.WriteLine($"Product found: {product.ProductName}, Price: {product.Price}");
                }
                else
                {
                    Console.WriteLine("Product not found");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void GetAllProducts()
        {
            try
            {
                IEnumerable<Product> products = _productController.GetAll();
                foreach (var product in products)
                {
                    Console.WriteLine($"Product: {product.ProductName}, Price: {product.Price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void AddProduct()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();
            Console.Write("Enter product price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                try
                {
                    _productController.Add(new Product { ProductName = name, Price = price });
                    Console.WriteLine("Product added successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid price");
            }
        }

        private void UpdateProduct()
        {
            Console.Write("Enter product name to update: ");
            string name = Console.ReadLine();
            Product product = _productController.GetByName(name);
            if (product != null)
            {
                Console.Write("Enter new price: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    try
                    {
                        product.Price = price;
                        _productController.Update(product);
                        Console.WriteLine("Product updated successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid price");
                }
            }
            else
            {
                Console.WriteLine("Product not found");
            }
        }

        private void DeleteProduct()
        {
            Console.Write("Enter product name to delete: ");
            string name = Console.ReadLine();
            Product product = _productController.GetByName(name);
            if (product != null)
            {
                try
                {
                    _productController.Delete(product);
                    Console.WriteLine("Product deleted successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Product not found");
            }
        }

        private void GetProductsByStoreId()
        {
            Console.Write("Enter store ID: ");
            if (int.TryParse(Console.ReadLine(), out int storeId))
            {
                try
                {
                    IEnumerable<Product> products = _productController.GetProductsByStoreId(storeId);
                    foreach (var product in products)
                    {
                        Console.WriteLine($"Product: {product.ProductName}, Price: {product.Price}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid store ID");
            }
        }

        private void GetCheapestProductsByStoreForItem()
        {
            Console.Write("Enter item name: ");
            string itemName = Console.ReadLine();
            try
            {
                IEnumerable<Product> products = _productController.GetCheapestProductsByStoreForItem(itemName);
                foreach (var product in products)
                {
                    Console.WriteLine($"Product: {product.ProductName}, Price: {product.Price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void PurchaseProductsFromStore()
        {
            Console.Write("Enter store ID: ");
            if (int.TryParse(Console.ReadLine(), out int storeId))
            {
                Console.Write("Enter number of products to purchase: ");
                if (int.TryParse(Console.ReadLine(), out int count))
                {
                    Dictionary<string, int> productsToPurchase = new Dictionary<string, int>();
                    for (int i = 0; i < count; i++)
                    {
                        Console.Write("Enter product name: ");
                        string productName = Console.ReadLine();
                        Console.Write("Enter quantity: ");
                        if (int.TryParse(Console.ReadLine(), out int quantity))
                        {
                            productsToPurchase.Add(productName, quantity);
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity");
                            return;
                        }
                    }
                    try
                    {
                        decimal totalPrice = _productController.PurchaseProductsFromStore(storeId, productsToPurchase);
                        Console.WriteLine($"Total price for the purchase: {totalPrice}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid number of products");
                }
            }
            else
            {
                Console.WriteLine("Invalid store ID");
            }
        }

        private void GetAffordableProductsByStoreForAmount()
        {
            Console.Write("Enter store ID: ");
            if (int.TryParse(Console.ReadLine(), out int storeId))
            {
                Console.Write("Enter amount: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    try
                    {
                        IEnumerable<Product> affordableProducts = _productController.GetAffordableProductsByStoreForAmount(storeId, amount);
                        foreach (var product in affordableProducts)
                        {
                            Console.WriteLine($"Affordable product: {product.ProductName}, Price: {product.Price}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid amount");
                }
            }
            else
            {
                Console.WriteLine("Invalid store ID");
            }
        }

        private void GetTotalPriceForProductsInStore()
        {
            Console.Write("Enter store ID: ");
            if (int.TryParse(Console.ReadLine(), out int storeId))
            {
                Console.Write("Enter number of products to purchase: ");
                if (int.TryParse(Console.ReadLine(), out int count))
                {
                    Dictionary<string, int> productsToPurchase = new Dictionary<string, int>();
                    for (int i = 0; i < count; i++)
                    {
                        Console.Write("Enter product name: ");
                        string productName = Console.ReadLine();
                        Console.Write("Enter quantity: ");
                        if (int.TryParse(Console.ReadLine(), out int quantity))
                        {
                            productsToPurchase.Add(productName, quantity);
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity");
                            return;
                        }
                    }
                    try
                    {
                        decimal totalPrice = _productController.GetTotalPriceForProductsInStore(storeId, productsToPurchase);
                        Console.WriteLine($"Total price for the products in store: {totalPrice}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid number of products");
                }
            }
            else
            {
                Console.WriteLine("Invalid store ID");
            }
        }
    }
}
