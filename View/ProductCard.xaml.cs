using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using My_food_manager.Models;
using My_food_manager.ViewModel;

namespace My_food_manager.View
{
    
    public partial class ProductCard : UserControl
    {
        public static readonly DependencyProperty myDepPropertyId = DependencyProperty.Register(nameof(Id), typeof(int), typeof(UserControl));
        public static readonly DependencyProperty myDepPropertyName = DependencyProperty.Register(nameof(Name), typeof(string), typeof(UserControl));
        public static readonly DependencyProperty myDepPropertyImg = DependencyProperty.Register(nameof(UrlPhoto), typeof(string), typeof(UserControl));
        public static readonly DependencyProperty myDepPropertyProvider = DependencyProperty.Register(nameof(Provider), typeof(string), typeof(UserControl));
        public static readonly DependencyProperty myDepPropertyPrice = DependencyProperty.Register(nameof(Price), typeof(double), typeof(UserControl));
        public static readonly DependencyProperty myDepPropertyDimension = DependencyProperty.Register(nameof(Dimension), typeof(string), typeof(UserControl));

        public int Id
        {
            get { return (int)GetValue(myDepPropertyId); }
            set { SetValue(myDepPropertyId, value); }
        }
        public string Name
        {
            get { return (string)GetValue(myDepPropertyName); }
            set { SetValue(myDepPropertyName, value); }
        }

        public string UrlPhoto
        {
            get { return (string)GetValue(myDepPropertyImg); }
            set { SetValue(myDepPropertyImg, value); }
        }

        public string Provider
        {
            get { return (string)GetValue(myDepPropertyProvider); }
            set { SetValue(myDepPropertyProvider, value); }
        }

        public double Price
        {
            get { return (double)GetValue(myDepPropertyPrice); }
            set { SetValue(myDepPropertyPrice, value); }
        }

        public string Dimension
        {
            get { return (string)GetValue(myDepPropertyDimension); }
            set { SetValue(myDepPropertyDimension, value); }
        }

        public ProductCard()
        {
            InitializeComponent();
            DataContext = this;
        }

        private RelayCommand openWindow;
        public RelayCommand OpenWindow
        {
            get
            {
                if (openWindow == null)
                    openWindow = new RelayCommand(obj => 
                    { 
                       MessageBoxResult res = MessageBox.Show($"Додати продукт {Name} з ціною {Price} {Dimension.Replace("\n", "")} в перелік покупок?", "", MessageBoxButton.YesNo); 
                    if (res == MessageBoxResult.Yes)
                        {
                            MainVM.IdProduct = Id;
                            int c = MainVM.IdProduct;
                        }
                    });
                return openWindow;
            }
        }
    }
}
