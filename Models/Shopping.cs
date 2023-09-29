using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace My_food_manager.Models
{
    public class Shopping : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        private double quantity { get; set; }
        public double Price { get; set; }
        public string Dimension { get; set; }
        public string Url { get; set; }

        public Shopping() 
        {
            Name = string.Empty;
            Provider = string.Empty;
            quantity = double.MinValue;
            Price = double.MinValue;
            Dimension = string.Empty;
            Url = string.Empty;
        }

        public double Quantity
        {
            get { return quantity; }
            set { quantity = value; OnPropertyChanged("Quantity"); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
