namespace My_food_manager.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int Key { get; set; }

        public Category()
        {
            Id = 0;
            CategoryName = string.Empty;
            Key = 0;
        }

        public Category(int key, string categoryName)
        {
            Key = key;
            CategoryName = categoryName;
        }
    }
}
