using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        public CategoriesPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        // Метод для загрузки категорий в ListView
        private void LoadCategories()
        {
            CategoriesListView.ItemsSource = practicaEntities.GetContext().categs.ToList();
        }

        // Обработчик события для добавления новой категории
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoryEditPage());
            LoadCategories(); // Обновляем список категорий после возврата на страницу
        }

        // Обработчик события для редактирования выбранной категории
        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoriesListView.SelectedItem as categ;
            if (selectedCategory != null)
            {
                NavigationService.Navigate(new CategoryEditPage(selectedCategory));
                LoadCategories(); // Обновляем список категорий после возврата на страницу
            }
            else
            {
                MessageBox.Show("Выберите категорию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}