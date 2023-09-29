using System;

namespace My_food_manager.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dimension{ get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }

        public Ingredient()
        {
            Id = 0;
            Name = string.Empty;
            Dimension = string.Empty;
            Weight = 0;
            Price = 0;
            Cost = Math.Round(Weight * Price, 2);
        }

        public override string ToString()
        {
            return $"{Name} {Dimension}";
        }
    }
}
