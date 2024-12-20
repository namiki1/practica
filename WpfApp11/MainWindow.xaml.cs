using System.Windows;
using System.Windows.Controls;

namespace WpfApp11
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

      

        // Обработчик события для перехода на страницу "Продукция"
        private void Products_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        // Обработчик события для перехода на страницу "Категории"
        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CategoriesPage());
        }

        // Обработчик события для перехода на страницу "Платежи"
        private void Payments_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PaymentsPage());
        }

     
    }
}