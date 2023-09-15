using My_food_manager.ViewModel;
using System.Windows;

namespace My_food_manager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM();
        }
    }
}
