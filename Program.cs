using Catalog.Client;
using Catalog.Domain;
using Catalog.Repository;

namespace Catalog.Controllers.Products
{

    class Program
    {
        static void Main(string[] args)
        {
            string configFilePath = "config.property";
            string source = ReadConfigProperty(configFilePath, "SOURCE");
            string path = ReadConfigProperty(source, "PATH");
            IProductRepository<Product> repo;

            if (source == "csv")
            {
                repo = new CsvProductRepository(path);
            }
            else if (source == "database")
            {
                repo = new PostgreProductRepository(path);
            }
            else
            {
                Console.WriteLine("Недопустимое значение SOURCE в файле config.property");
                return;
            }

            var controller = new ProductController(repo);
            ConsoleClient iface = new ConsoleClient(controller);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
            };

            while (true)
            {
                iface.Run();
            }
        }

        static string ReadConfigProperty(string filePath, string propertyName)
        {
            string value = string.Empty;
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2 && parts[0].Trim() == propertyName)
                    {
                        value = parts[1].Trim();
                        break;
                    }
                }
            }
            return value;
        }
    }
}
