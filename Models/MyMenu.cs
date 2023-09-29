using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace My_food_manager.Models 
{
    public class MyMenu
    {
        public ObservableCollection<MenuItem> Nodes { get; set; }



        public MyMenu()
        {
            Nodes = new ObservableCollection<MenuItem>();

        }

        public void Add(string date, string mealTime, string dishNumber, string dish, string description)
        {
            if (Nodes.Count > 0 && Nodes.Any(e => e.Title == date))
            {
                var myDate = Nodes.FirstOrDefault(e => e.Title == date);
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
                Nodes.Add(new MenuItem { Title = date, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = mealTime, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dishNumber, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = dish, Items = new ObservableCollection<MenuItem> { new MenuItem { Title = description } } } } } } } } });
            }
        }

        //public void CreateMenuTree()
        //{
        //    if (Menu == null || Menu.Items.Count==0)
        //    {
        //        Menu = new MenuItem() { Title = "Моє меню" };
        //        MenuItem Dates = new MenuItem() { Title = date.Date.ToString() };
        //        MenuItem MealTimes = new MenuItem() { Title = mealTime };
        //        MenuItem DishNumbers = new MenuItem() { Title = dishNumber };
        //        DishNumbers.Items.Add(new MenuItem() { Title = disheName });
        //        MealTimes.Items.Add(DishNumbers);
        //        Dates.Items.Add(MealTimes);
        //        Menu.Items.Add(Dates);
        //    }
        //}

        public void ReadFromFile(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                Nodes = JsonConvert.DeserializeObject<ObservableCollection<MenuItem>>(json);
            }
        }

        public void SaveToFile(string path)
        {
           string json = JsonConvert.SerializeObject(Nodes);
            File.WriteAllText(path, json);
        }

      
    }
}
