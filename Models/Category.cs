namespace My_food_manager.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public Category()
        {
            Id = 0;
            CategoryName = string.Empty;
        }

        public Category(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}
