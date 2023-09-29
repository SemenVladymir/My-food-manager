namespace MyServer.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dimension { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public string PhotoURL { get; set; }
        public int ProviderId { get; set; }
        public bool IsAvailable { get; set; }

        public Product()
        {
            Id = 0;
            Name = string.Empty;
            Dimension = string.Empty;
            CategoryId = 0;
            Price = 0;
            PhotoURL = string.Empty;
            ProviderId = 0;
            IsAvailable = false;
        }

        public Product(string name, string dimension, int categoryId, double price, string photoURL, int providerId, bool isAvailable)
        {
            Name = name;
            Dimension = dimension;
            CategoryId = categoryId;
            Price = price;
            PhotoURL = photoURL;
            ProviderId = providerId;
            IsAvailable = isAvailable;
        }
    }
}
