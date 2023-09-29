using MaterialDesignThemes.Wpf;
using My_food_manager.Models;
using My_food_manager.ViewModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace My_food_manager.View
{
    public partial class DishCard : UserControl
    {
        public static readonly DependencyProperty myDepPropertyNewDish = DependencyProperty.Register(nameof(NewDish), typeof(Dish), typeof(UserControl));
        
        public Dish NewDish
        {
            get { return (Dish)GetValue(myDepPropertyNewDish); }
            set { SetValue(myDepPropertyNewDish, value); }
        }
        
        public DishCard()
        {
            InitializeComponent();
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NewWindow newWindow = new NewWindow();
            AddToMenu menu = new AddToMenu();
            MenuVM vm = new MenuVM();
            vm.MyNewDish = NewDish.Name;
            menu.DataContext = vm;
            newWindow.Content = menu;
            if (NewDish.Name != null)
                newWindow.Title = $"Додати {NewDish.Name} в меню";
            newWindow.Show();
            MyMenu menu2 = new MyMenu();
            newWindow.Closed += (s, ee) => {  };
        }

        private void btnAddFavor_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Додати це блюдо в ваші фаворити?", "Додати до фаворитів", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                ObservableCollection<Dish> tmp = new ObservableCollection<Dish>();
                string json;
                if (System.IO.File.Exists("MyFavorites.json"))
                {
                    json = System.IO.File.ReadAllText("MyFavorites.json");
                    tmp = JsonConvert.DeserializeObject<ObservableCollection<Dish>>(json);
                }
                if (!tmp.Any(e => e.Name == NewDish.Name))
                {
                    tmp.Add(NewDish);
                    json = JsonConvert.SerializeObject(tmp);
                    System.IO.File.WriteAllText("MyFavorites.json", json);
                    bool t = MainVM.IsChecked;
                }
                else
                {
                    MessageBox.Show("Таке блюдо вже є в ваших фаворитах!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
     }
}
