namespace Catalog.Domain
{
    public class Product
    {
        public string ProductName { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public void Check()
        {
            if (StoreId <= 0)
            {
                throw new ArgumentException("Store ID must be a positive integer");
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                throw new ArgumentException("ProductName must be filled");
            }
            if (Quantity < 0)
            {
                throw new ArgumentException("Quantity must be a positive integer");
            }
            if (Price < 0)
            {
                throw new ArgumentException("Quantity must be a positive floating point number");
            }
        }
    }
}
