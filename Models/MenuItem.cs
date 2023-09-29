using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace My_food_manager.Models
{
    public class MenuItem
    {
        public string Title { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; } = new ObservableCollection<MenuItem>();

        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public MenuItem(string date, string mealTime, string dishNumber, string dish, string description)
        {
            Items.Add(new MenuItem { Title = date, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } } } });
        }

        public void Add(string date, string mealTime, string dishNumber, string dish, string description)
        {
            if (Items.Count > 0 && Items.Any(e => e.Title == date))
            {
                var myDate = Items.FirstOrDefault(e => e.Title == date);
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
                Items.Add(new MenuItem { Title = date, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } } } });
            }
        }

        public void ReadFromFile(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                Items = JsonConvert.DeserializeObject<ObservableCollection<MenuItem>>(json);
            }
        }

        public void SaveToFile(string path)
        {
            string json = JsonConvert.SerializeObject(Items);
            File.WriteAllText(path, json);
        }
    }
}
