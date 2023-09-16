namespace My_food_manager.Models
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

        public Product()
        {
            Id = 0;
            Name = string.Empty;
            Dimension = string.Empty;
            CategoryId = 0;
            Price = 0;
            PhotoURL = string.Empty;
            ProviderId = 0;
        }

        public Product(string name, string dimension, int categoryId, double price, string photoURL, int providerId)
        {
            Name = name;
            Dimension = dimension;
            CategoryId = categoryId;
            Price = price;
            PhotoURL = photoURL;
            ProviderId = providerId;
        }
    }
}
