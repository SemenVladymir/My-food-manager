using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using My_food_manager.Models;
using My_food_manager.SVConnection;
using My_food_manager.View;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using MenuItem = My_food_manager.Models.MenuItem;

namespace My_food_manager.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        const string path = "MyMenu.json";
        const string CategoryPath = "ProductCategories.json";
        const string GroupsPath = "DishGroups.json";
        const string AllProductsPath = "AllProducts.json";
        const string AllDishedPath = "AllDishets.json";
        const string FavoritePath = "MyFavorites.json";

        const int PORT = 4444;
        const string IP = "127.0.0.1";
        ServerConn server = new ServerConn(PORT, IP);

        List<Dish> alldishes = new List<Dish>();
        List<Product> allproducts = new List<Product>();
        List<Category> categories = new List<Category>();
        private List<string> allcategories = new List<string>();
        private string selectedCategory;
        private string selectedDishName;

        private static int idProduct;
        private static bool _newDishIsSaved = false;
        private static Shopping selectedDataItem;
        private static ObservableCollection<Shopping> productsForShopping = new ObservableCollection<Shopping>();

        private static ObservableCollection<Product> products = new ObservableCollection<Product>();
        private Product selectedProduct;

        private ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
        private Dish selectedDish;

        private ObservableCollection<object> pcards = new ObservableCollection<object>();

        private List<string> groups = new List<string>();
        private string selectedDishGroup;

        private ObservableCollection<Ingredient> ingredients = new ObservableCollection<Ingredient>();
        private Ingredient selectedIngredient;
        private string searchDishName;
        private string searchProduct;
        private static bool _isChecked;
        private static ObservableCollection<MenuItem> menu;
        private ObservableCollection<MenuItem> nodes;
        private static ObservableCollection<Dish> favoriteDishes = new ObservableCollection<Dish>();

        private RelayCommand delete;
        public RelayCommand Delete
        {
            get
            {
                if (delete == null)
                    delete = new RelayCommand(obj =>
                    {
                        MessageBoxResult res = MessageBox.Show("Видалити це блюдо з моїх фаворитів?", "Видалення", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                            favoriteDishes.Remove(favoriteDishes.Single(e=>e.Id==SelectedDish.Id));
                        SaveFavoriteToFile(FavoriteDishes, FavoritePath);
                    });

                return delete;
            }
        }

        private RelayCommand deleteline;
        public RelayCommand DeleteLine
        {
            get
            {
                if (deleteline == null)
                    deleteline = new RelayCommand(obj =>
                    {
                        if (selectedDataItem.Name != "УСЬОГО:")
                        {
                            MessageBoxResult res = MessageBox.Show("Видалити цей продукт?", "Видалення", MessageBoxButton.YesNo);
                            if (res == MessageBoxResult.Yes)
                                productsForShopping.Remove(productsForShopping.Single(e => e.Name == selectedDataItem.Name));
                        }
                    });

                return deleteline;
            }
        }


        private RelayCommand goShopping;
        public RelayCommand GoShopping
        {
            get
            {
                if (goShopping == null)
                    goShopping = new RelayCommand(obj => { if (productsForShopping.Count > 1)
                        ShoppingBot(); });

                return goShopping;
            }
        }

        public static int IdProduct
        {
            get {
                BuyingProducts(idProduct);
                return idProduct; }
            set { idProduct = value; NotifyStaticPropertyChanged("IdProduct"); }
        }

        public static bool NewDishIsSaved
        {
            get
            {
                if (System.IO.File.Exists(path))
                {
                    string json = System.IO.File.ReadAllText(path);
                    Menu = JsonConvert.DeserializeObject<ObservableCollection<MenuItem>>(json);
                }
                return _newDishIsSaved;
            }
            set { _newDishIsSaved = value; NotifyStaticPropertyChanged("NewDishIsSaved"); }
        }

        public static Shopping SelectedDataItem
        {
            get
            {
               
                return selectedDataItem;
            }
            set { selectedDataItem = value; NotifyStaticPropertyChanged("SelectedDataItem"); }
        }

        public static ObservableCollection<Shopping> ProductsForShopping
        {
            get { 
                return productsForShopping; }
            set { productsForShopping = value; NotifyStaticPropertyChanged("ProductsForShopping"); }
        }

        public ObservableCollection<MenuItem> Nodes
        {
            get {return nodes;}
            set { nodes = value; NotifyStaticPropertyChanged("Nodes"); }
        }

        public  List<string> Allcategories
        {
            get { return allcategories; }
            set { allcategories = value; NotifyStaticPropertyChanged("Allcategories"); }
        }

        public string SelectedCategory
        {
            get {
                    SelectedProductByName(!string.IsNullOrEmpty(searchProduct) ? searchProduct : (selectedIngredient!=null && !string.IsNullOrEmpty(selectedIngredient.Name))? selectedIngredient.Name: "");
                return selectedCategory; }
            set { selectedCategory = value; OnPropertyChanged("SelectedCategory"); }
        }

        public string SelectedDishName
        {
            get
            {
                if (dishes.FirstOrDefault(e => e.Name.Equals(selectedDishName))!=null)
                    DishSelected(dishes.FirstOrDefault(e=>e.Name.Equals(selectedDishName)));
                return selectedDishName;
            }
            set { selectedDishName = value; OnPropertyChanged("SelectedDishName"); }
        }

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { products = value; OnPropertyChanged("Products"); }
        }

        public Product SelectedProduct
        {
            get {
                
                return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged("SelectedProduct"); }
        }


        public static ObservableCollection<Dish> FavoriteDishes
        {
            get
            {
                return favoriteDishes;
            }
            set { favoriteDishes = value; NotifyStaticPropertyChanged("FavoriteDishes"); }
        }


        public ObservableCollection<Dish> Dishes
        {
            get { return dishes; }
            set { dishes = value; OnPropertyChanged("Dishes"); }
        }

        public Dish SelectedDish
        {
            get {
                DishSelected(selectedDish);
                return selectedDish; }
            set { selectedDish = value; OnPropertyChanged("SelectedDish"); }
        }

        public List<string> Groups
        {
            get { return groups; }
            set { groups = value; OnPropertyChanged("Groups"); }
        }

        public string SelectedDishGroup
        {
            get {
                SelectedDishes();
                return selectedDishGroup; 
            }
            set { selectedDishGroup = value; OnPropertyChanged("SelectedDishGroup"); }
        }

        public ObservableCollection<object> PCards
        {
            get { return pcards; }
            set { pcards = value; OnPropertyChanged("PCards"); }
        }

        public ObservableCollection<Ingredient> Ingredients
        {
            get { return ingredients; }
            set { ingredients = value; OnPropertyChanged("Ingredients"); }
        }

        public Ingredient SelectedIngredient
        {
            get {
                if (selectedIngredient != null)
                    SelectedProductByName(selectedIngredient.Name);
                return selectedIngredient; }
            set { selectedIngredient = value; OnPropertyChanged("SelectedIngredients"); }
        }

        public string SearchDishName
        {
            get
            {
                SelectedDishes();
                return searchDishName;
            }
            set { searchDishName = value; OnPropertyChanged("SearchDishName"); }
        }

        public string SearchProduct
        {
            get
            {
                SelectedProductByName(searchProduct);
                return searchProduct;
            }
            set { searchProduct = value; OnPropertyChanged("SearchPrroduct"); }
        }

        public static bool IsChecked
        {
            get {
                FavoriteDishes = ReadFavoriteDishesFromFile(FavoritePath);
                return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyStaticPropertyChanged(nameof(IsChecked));
            }
        }

        public static ObservableCollection<MenuItem> Menu
        {
            get { return menu; }
            set { menu = value; NotifyStaticPropertyChanged(nameof(Menu));}
        }

        public MainVM() 
        {
            CategoriesProductReadFromSQL();
            ProductsReadFromSQL();
            //CreateProductCards(products);
            DishesReadFromSQL();
            GetAllDishGroups();
            ProductsForShopping.Add(new Shopping {Name="УСЬОГО:", Price=0, Dimension="-", Provider="-", Quantity=0 });
            Menu = ReadMenuFromFile(path);
            FavoriteDishes = ReadFavoriteDishesFromFile(FavoritePath);
        }

        private static ObservableCollection<Dish> ReadFavoriteDishesFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                return JsonConvert.DeserializeObject<ObservableCollection<Dish>>(json);
            }
            return null;
        }

        private static void SaveFavoriteToFile(ObservableCollection<Dish> favorites, string path)
        {
            string json = JsonConvert.SerializeObject(favorites);
            System.IO.File.WriteAllText(path, json);
        }

        private void SaveMenuToFile (ObservableCollection<MenuItem> menu, string path)
        {
            string json = JsonConvert.SerializeObject(menu);
            System.IO.File.WriteAllText(path, json);
        }

        private ObservableCollection<MenuItem> ReadMenuFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                return JsonConvert.DeserializeObject<ObservableCollection<MenuItem>>(json);
            }
            return null;
        }

        private ObservableCollection<MenuItem> AddTreview(ObservableCollection<MenuItem> oldColl, string date, string mealTime, string dishNumber, string dish, string description)
        {
            if (oldColl.Count > 0 && oldColl.Any(e => e.Title == date))
            {
                var myDate = oldColl.FirstOrDefault(e => e.Title == date);
                if (myDate.Items.Any(e => e.Title == mealTime))
                {
                    var myMealTime = myDate.Items.FirstOrDefault(e => e.Title == mealTime);
                    if (myMealTime.Items.Any(e => e.Title == dishNumber))
                    {
                        var myDishNumber = myMealTime.Items.FirstOrDefault(e => e.Title == dishNumber);
                        if (myDishNumber.Items.Any(e => e.Title == dish))
                        {
                            var myDish = myDishNumber.Items.FirstOrDefault(e => e.Title == dish);
                            myDish.Title = description;
                        }
                        else
                        {
                            MenuItem newDishNumber = new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } };
                            myDishNumber.Items.Add(newDishNumber);
                        }
                    }
                    else
                    {
                        MenuItem newMealTime = new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } };
                        myMealTime.Items.Add(newMealTime);
                        //myMealTime.Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items= new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } };
                    }
                }
                else
                {
                    MenuItem newDate = new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } };
                    myDate.Items.Add(newDate);
                    //myDate.Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } };
                }
            }
            else
            {
                oldColl.Add(new MenuItem { Title = date, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } } } });
            }
            return oldColl;
        }

        private ObservableCollection<MenuItem> CreateTreeView(string date, string mealTime, string dishNumber, string dish, string description)
        {
            return new ObservableCollection<MenuItem> { new MenuItem { Title = date, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } } } } };
        }

        private static void BuyingProducts(int productId)
        {
            Product prod = products.FirstOrDefault(e => e.Id == productId);
            //double quantity = selectedIngredient.Weight;
            ProductsForShopping.Add(new Shopping {Name=prod.Name, Provider=prod.ProviderId==1?"АТБ":prod.ProviderId==2?"Сільпо":"Варус", Quantity= 0.1, Dimension=prod.Dimension, Price=prod.Price, Url=prod.PhotoURL  });
            ProductsForShopping[0].Price = 0;
            foreach (var item in productsForShopping)
                if (item.Price!=0 && item.Quantity!=0)
                    ProductsForShopping[0].Price += Math.Round(item.Price * item.Quantity, 2);   
        }


        private void SelectedDishes()
        {
            Dishes.Clear();
            if (selectedDishGroup != "Усі категорії" && !string.IsNullOrEmpty(searchDishName))
                Dishes = new ObservableCollection<Dish>(alldishes.Where(c => c.Name.ToLower().Contains(searchDishName.ToLower()) && c.Classification.ToLower().Contains(selectedDishGroup.ToLower())).ToList());
            else if (selectedDishGroup != "Усі категорії" && string.IsNullOrEmpty(searchDishName))
                Dishes = new ObservableCollection<Dish>(alldishes.Where(c => c.Classification.Contains(selectedDishGroup)).ToList());
            else if (selectedDishGroup == "Усі категорії" && !string.IsNullOrEmpty(searchDishName))
                Dishes = new ObservableCollection<Dish>(alldishes.Where(c => c.Name.ToLower().Contains(searchDishName.ToLower())));
            else
                Dishes = new ObservableCollection<Dish>(alldishes);
        }


        private void SelectedProductByName(string selected)
        {
            if (!string.IsNullOrEmpty(selected) && selected.Length > 2)
            {
                Products.Clear();
                if (!string.IsNullOrEmpty(selected))
                {
                    selected = selected.Substring(0, 1) == " " ? selected.Replace("  ", " ").Substring(1, selected.Length - 1) : selected.Replace("  ", " ");
                    string[] select = selected.Split(" ");
                    if (selectedCategory != "Усі групи")
                    {
                        int cat = categories.FirstOrDefault(e => e.CategoryName == selectedCategory).Key;
                        if (select.Length > 1)
                        {
                            if (select[0].Length > 4 && select[1].Length > 4)
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.CategoryId == cat && (c.Name.ToLower().Contains(select[0].ToLower().Substring(0, (int)Math.Round(select[0].Length * 0.7))) || c.Name.ToLower().Contains(select[1].ToLower().Substring(0, (int)Math.Round(select[1].Length * 0.7))))).ToList());
                            else
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.CategoryId == cat && (c.Name.ToLower().Contains(select[0].ToLower()) || c.Name.ToLower().Contains(select[1].ToLower()))).ToList());
                        }
                        else
                        {
                            if (select[0].Length > 4)
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.CategoryId == cat && c.Name.ToLower().Contains(select[0].ToLower().Substring(0, (int)Math.Round(select[0].Length * 0.7)))).ToList());
                            else
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.CategoryId == cat && c.Name.ToLower().Contains(select[0].ToLower())).ToList());
                        }
                    }
                    else
                    {
                        if (select.Length > 1)
                        {
                            if (select[0].Length > 4 && select[1].Length > 4)
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.Name.ToLower().Contains(select[0].ToLower().Substring(0, (int)Math.Round(select[0].Length * 0.7))) || c.Name.ToLower().Contains(select[1].ToLower().Substring(0, (int)Math.Round(select[1].Length * 0.7)))).ToList());
                            else
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.Name.ToLower().Contains(select[0].ToLower()) || c.Name.ToLower().Contains(select[1].ToLower())).ToList());
                        }
                        else
                        {
                            if (select[0].Length > 4)
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.Name.ToLower().Contains(select[0].ToLower().Substring(0, (int)Math.Round(select[0].Length * 0.7)))).ToList());
                            else
                                Products = new ObservableCollection<Product>(allproducts.Where(c => c.Name.ToLower().Contains(select[0].ToLower())).ToList());
                        }
                    }
                }
                else if (selectedCategory != "Усі групи")
                    Products = new ObservableCollection<Product>(allproducts.Where(c => c.CategoryId == categories.FirstOrDefault(e => e.CategoryName == selectedCategory).Key));
                else
                    Products = new ObservableCollection<Product>(allproducts);
                CreateProductCards(Products);
            }
        }

        private void DishSelected(Dish selected)
        {
            if (selected == null) return;
            ReadDishById(selected.Id);
            selected = selectedDish;
            PCards.Clear();
            WorkWithIngredients(selected);
            //Ingredients = selected.Ingredients.Replace(";", "<LineBreak/>");
            DishCard card = new DishCard {NewDish=selected };
            card.DataContext = selected;
            //card = new DishCard { DishName = selected.Name, DishIngredients = selected.Ingredients, DishDescription = selected.Description, DishRecipe = selected.RecipeInstruction, DishUrlPhoto = selected.MainPhotoURL };
            //------------------------------
            string[] messageSplit = selected.RecipeInstruction.Replace("\n\n", "\n").Split('\n');
            string[] imgurl = selected.RecipePhotoURL.Split("\n");
            FlowDocument doc = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            int k = 0;
            for (int i = 0; i < messageSplit.Length; ++i)
            {
                string str = messageSplit[i];
                if (str != "" && str != "- ")
                {
                    paragraph.Inlines.Add(new Run(str));
                    paragraph.Inlines.Add(new LineBreak());
                    doc.Blocks.Add(paragraph);
                    if (k < imgurl.Length && !string.IsNullOrEmpty(imgurl[k]))
                    {
                        Image image = new Image();
                        image.Source = (ImageSource)((new ImageSourceConverter()).ConvertFromString(imgurl[k++]));

                        BlockUIContainer cont = new BlockUIContainer(image);
                        Figure figure = new Figure(cont);
                        figure.Width = new FigureLength(400);
                        figure.Height = new FigureLength(400);
                        figure.WrapDirection = WrapDirection.None;
                        figure.VerticalAnchor = FigureVerticalAnchor.PageTop;
                        figure.HorizontalAnchor = FigureHorizontalAnchor.ContentCenter;

                        paragraph.Inlines.Add(figure);
                        doc.Blocks.Add(paragraph);
                    }
                }
            }
                //------------------------------
            card.Recipe.Document = doc;
            PCards.Add(card);

        }

        private void WorkWithIngredients(Dish dish)
        {
            string []tmp = dish.Ingredients.Replace(";;", ";").Replace("--", "-").Replace("––", "–").Split(';');
            Ingredients.Clear();
            foreach (string item in tmp) {
                string[] strings = item.Replace("  ", " ").Replace(" - ", "-").Replace(" – ", "–").Split(new char[] { '-', '–' });
                if (strings.Length > 1) 
                {
                    if(Double.TryParse(strings[1].Split(" ")[0], out double j))
                        Ingredients.Add(new Ingredient { Name = strings[0], Weight = j, Dimension = strings[1].Replace($"j", "") });
                    else
                        Ingredients.Add(new Ingredient { Name = strings[0], Weight = 0, Dimension = strings[1].Replace($"j", "") });
                }
            }
        }

        private void CreateProductCards(object products)
        {
            PCards.Clear();
            foreach (var prod in products as ObservableCollection<Product>)
            {
                if (prod.IsAvailable)
                {
                    ProductCard prodCard;
                    prodCard = new ProductCard {Id = prod.Id, Name = prod.Name, UrlPhoto = prod.PhotoURL, Price = prod.Price, Dimension = prod.Dimension, Provider = prod.ProviderId == 1 ? "АТБ" : prod.ProviderId == 2 ? "Сільпо" : "Варус" };
                    //ProductCard Card = new ProductCard({ Name = prod.Name, UrlPhoto = prod.PhotoURL, Price = prod.Price, Dimension = prod.Dimension, Provider = prod.ProviderId == 1 ? "АТБ" : prod.ProviderId == 2 ? "Сільпо" : "Варус" });
                    prodCard.Width = 190;
                    prodCard.Height = 230;
                    pcards.Add(prodCard);
                }
            }
        }

        private void ProductsReadFromSQL()
        {
            if (System.IO.File.Exists(AllProductsPath))
            {
                string json = System.IO.File.ReadAllText(AllProductsPath);
                List<Product> prod = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(json);
                for (int i = 0; i < 3000; i++)
                    allproducts.Add(prod[i]);
                products = new ObservableCollection<Product>(allproducts.ToList());
                selectedProduct = products[0];
            }
            else
            {
                server.DataSend("#RDPR#");
                string answer = server.DataReceive();
                products = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Product>>(answer);
                for (int i=0; i< products.Count; i++)
                {
                    products[i] = new Product { Id = products[i].Id, CategoryId = products[i].CategoryId, Dimension = products[i].Dimension.Replace(" ", ""), Name = products[i].Name.Replace("i", "і"),
                        PhotoURL = products[i].PhotoURL, Price = products[i].Price, ProviderId = products[i].ProviderId, IsAvailable = products[i].IsAvailable };
                }
                selectedProduct = products[0];
                //products = new ObservableCollection<Product>(products.OrderBy(i => i.Name));
                allproducts = products.ToList();
                string json = JsonConvert.SerializeObject(allproducts);
                System.IO.File.WriteAllText(AllProductsPath, json);
            }
        }

        private void DishesReadFromSQL()
        {
            if (System.IO.File.Exists(AllDishedPath))
            {
                string json = System.IO.File.ReadAllText(AllDishedPath);
                alldishes = System.Text.Json.JsonSerializer.Deserialize<List<Dish>>(json);
                dishes = new ObservableCollection<Dish>(alldishes.ToList());
                ReadDishById(dishes[0].Id);
            }
            else
            {
                server.DataSend("#RDADS#");
                string answer = server.DataReceive();
                dishes = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Dish>>(answer);
                ReadDishById(dishes[0].Id);
                //dishes = new ObservableCollection<Dish>(dishes.OrderBy(i => i.Name));
                alldishes = dishes.ToList();
                string json = JsonConvert.SerializeObject(alldishes);
                System.IO.File.WriteAllText(AllDishedPath, json);
            }
        }

        private void CategoriesProductReadFromSQL()
        {
            if (System.IO.File.Exists(CategoryPath) && System.IO.File.Exists("Categories.json"))
            {
                string json = System.IO.File.ReadAllText(CategoryPath);
                allcategories = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                string jsoncat = System.IO.File.ReadAllText("Categories.json");
                categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(jsoncat);
                SelectedCategory = allcategories[0];
            }
            else
            {
                server.DataSend("#RDCP#");
                string answer = server.DataReceive();
                categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(answer);
                foreach (var item in categories)
                {
                    allcategories.Add(item.CategoryName);
                }
                allcategories = new List<string>(allcategories.OrderBy(e => e).ToList());
                allcategories.Insert(0, "Усі групи");
                SelectedCategory = allcategories[0];
                string json = JsonConvert.SerializeObject(allcategories);
                System.IO.File.WriteAllText(CategoryPath, json);
                string jsoncat = JsonConvert.SerializeObject(categories);
                System.IO.File.WriteAllText("Categories.json", jsoncat);
            }
        }

        private void ReadDishById(int Id)
        {
            server.DataSend("#RDDSID#" + Id);
            string answer = server.DataReceive();
            selectedDish = System.Text.Json.JsonSerializer.Deserialize<Dish>(answer);
        }

        private void GetAllDishGroups()
        {
            if (System.IO.File.Exists(GroupsPath))
            {
                string json = System.IO.File.ReadAllText(GroupsPath);
                groups = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                SelectedDishGroup = groups[0];
            }
            else
            {
                for (int r = 0; r < dishes.Count; r++)
                {
                    string[] clas = dishes[r].Classification.Replace(" /", "/").Replace("/ ", "/").Split("/");
                    for (int k = 0; k < clas.Length; k++)
                    {
                        if (groups.Count == 0)
                            groups.Add(clas[0]);
                        else
                        {
                            if (!groups.Any(e => e == clas[k]))
                                groups.Add(clas[k]);
                        }
                    }
                }
                groups = new List<string>(groups.OrderBy(e => e).ToList());
                groups.Insert(0, "Усі категорії");
                SelectedDishGroup = groups[0];
                string json = JsonConvert.SerializeObject(groups);
                System.IO.File.WriteAllText(GroupsPath, json);
            }
        }


        private async Task ShoppingBot()
        {
            async Task MsgHandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
            {
                await client.SendTextMessageAsync(update.Message.Chat.Id, $"Необхідно купити:");
                int count = 1;
                foreach (var item in ProductsForShopping)
                {
                    if (item.Name != "УСЬОГО:")
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, $"{count++}) {item.Name} кількість {item.Quantity}{item.Dimension.Replace("грн/", "")} \nє у {item.Provider} за ціною {item.Price}{item.Dimension}");
                        
                    }
                }
                return;
            }

            async Task ErrorHandleAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellation)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(exception);
            }

            const string TOKEN = "6369237745:AAFVor75ogmmUEjMFG5T-4_c-umNHYs_X_E";
            ITelegramBotClient client = new TelegramBotClient(TOKEN);
            client = new TelegramBotClient(TOKEN);
            var cts = new CancellationTokenSource();
            var cancelToken = cts.Token;
            client.StartReceiving(MsgHandleAsync, ErrorHandleAsync, new Telegram.Bot.Polling.ReceiverOptions { }, cancelToken);
        }

       


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static void NotifyStaticPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null,
                new PropertyChangedEventArgs(propertyName));
        }


    }
}
