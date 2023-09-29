using My_food_manager.Models;
using My_food_manager.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MenuItem = My_food_manager.Models.MenuItem;

namespace My_food_manager
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Application.Current.Resources = Application.LoadComponent(new Uri("View\\DayStyles.xaml", UriKind.Relative)) as ResourceDictionary;
            InitializeComponent();
            //LightSwitch.IsChecked = false;
            //LightSwitch.Click += LightSwitch_Click;
        }

        //private void LightSwitch_Click(object sender, RoutedEventArgs e)
        //{
        //    if (LightSwitch.IsChecked == false)
        //    {
        //        ResourceDictionary res = Application.LoadComponent(new Uri("View\\DayStyles.xaml", UriKind.Relative)) as ResourceDictionary;
        //        Application.Current.Resources.Clear();
        //        Application.Current.Resources.MergedDictionaries.Add(res);
        //    }
        //    else
        //    {
        //        ResourceDictionary res = Application.LoadComponent(new Uri("View\\NightStyles.xaml", UriKind.Relative)) as ResourceDictionary;
        //        Application.Current.Resources.Clear();
        //        Application.Current.Resources.MergedDictionaries.Add(res);
        //    }
        //}

        public class StringToDoubleConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (Double.TryParse(value.ToString(), out double number))
                    return number;
                else
                    return 0;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value.ToString();
            }
        }

        

    }
}
