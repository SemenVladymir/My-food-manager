namespace My_food_manager.Models
{
    public class DishGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DishGroup()
        {
            Id = 0;
            Name = string.Empty;
        }

        public DishGroup(string name)
        {
            Name = name;
        }
    }
}
