using My_food_manager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace My_food_manager.ViewModel
{
    public class MenuVM : INotifyPropertyChanged
    {

        const string path = "MyMenu.json";

        private DateTime selectedDate;
        private List<string> dishTime;
        private List<string> numberOfDish;
        private string menuDescription;
        private string selectedNumberOfDish;
        private string selectedDishTime;
        private string myNewDish;
        public ObservableCollection<MenuItem> MyMenu { get; set; }
        
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; OnPropertyChanged("SelectedDate"); }
        }

        public List<string> DishTime
        {
            get {
                return dishTime; }
            set { dishTime = value; OnPropertyChanged("DishTime"); }
        }

        public string SelectedDishTime
        {
            get { 
                return selectedDishTime; }
            set { selectedDishTime = value; OnPropertyChanged("SelectedDishTime"); }
        }

        public List<string> NumberOfDish
        {
            get { return numberOfDish; }
            set { numberOfDish = value; OnPropertyChanged("NumberOfDish"); }
        }

        public string MenuDescription
        {
            get { return menuDescription; }
            set { menuDescription = value; OnPropertyChanged("MenuDescription"); }
        }

        public string SelectedNumberOfDish
        {
            get { 
                return selectedNumberOfDish; }
            set { selectedNumberOfDish = value; OnPropertyChanged("SelectedNumberOfDish"); }
        }

        public string MyNewDish
        {
            get
            {
                return myNewDish;
            }
            set { myNewDish = value; OnPropertyChanged("MyNewDish"); }
        }

        private RelayCommand addToMenu;
        public RelayCommand AddToMenu
        {
            get
            {
                if (addToMenu == null)
                    addToMenu = new RelayCommand(obj => {
                        Answer();
                    });

                return addToMenu;
            }
        }

        public EventHandler CloseHandler;
        public ICommand AddNewBidCommand => new RelayCommand(action =>
        {
            var handler = CloseHandler;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);

        }, canExecute => true);


        private void Answer()
        {
            
            if (SelectedDate!=null && SelectedDishTime!=null && SelectedNumberOfDish != null)
            {
                MyMenu = ReadMenuFromFile(path);
                if (MyMenu.FirstOrDefault(e => e.Title !=null && e.Title == SelectedDate.ToString("dd.MM.yyyy")).Items.FirstOrDefault(e => e.Title != null && e.Title == SelectedDishTime).Items.FirstOrDefault(e => e.Title != null && e.Title == selectedNumberOfDish).Items.Any(e => e.Title != null && e.Title == MyNewDish)) 
                {
                    MessageBox.Show("Це блюдо вже є у меню на цей період!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Додати це блюдо у меню?", "Додати в меню", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        MyMenu = AddTreview(MyMenu, SelectedDate.ToString("dd.MM.yyyy"), SelectedDishTime, selectedNumberOfDish, MyNewDish, MenuDescription); ;
                        SaveMenuToFile(MyMenu, path);
                        bool tmp = MainVM.NewDishIsSaved;
                    }
                }
            }
        }

        public MenuVM()
        {
            dishTime = new List<string> { "Сніданок", "Обід", "Вечеря", "Полудень", "День народження", "Свято", "Інше" };
            numberOfDish = new List<string> { "Перші страви", "Другі страви", "Напої", "Десерти", "Додатково", "Інше" };
        }

        private void SaveMenuToFile(ObservableCollection<MenuItem> menu, string path)
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
