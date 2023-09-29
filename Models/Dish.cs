namespace My_food_manager.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Classification { get; set; }
        public string? MainPhotoURL { get; set; }
        public string? Description { get; set; }
        public string? Ingredients { get; set; }
        public string? RecipeInstruction { get; set; }
        public string? RecipePhotoURL { get; set; }
        public string? AllText { get; set; }

        public Dish()
        {
            Id = 0;
            Name = string.Empty;
            Classification = string.Empty;
            MainPhotoURL = string.Empty;
            Description = string.Empty;
            Ingredients = string.Empty;
            RecipeInstruction = string.Empty;
            RecipePhotoURL = string.Empty;
            AllText = string.Empty;
        }

        public Dish(string name, string classification, string mainPhotoURL, string description, string ingredients, string recipeInstruction, string recipePhotoURL, string allText)
        {
            Name = name;
            Classification = classification;
            MainPhotoURL = mainPhotoURL;
            Description = description;
            Ingredients = ingredients;
            RecipeInstruction = recipeInstruction;
            RecipePhotoURL = recipePhotoURL;
            AllText = allText;
        }
    }
}
