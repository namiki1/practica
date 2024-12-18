using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        // Метод для загрузки категорий в ComboBox
        private void LoadCategories()
        {
            var categories = practicaEntities.GetContext().categs.ToList();
            categories.Insert(0, new categ { id = 0, category_name = "Все категории" });
            CategoryFilterComboBox.ItemsSource = categories;
            CategoryFilterComboBox.SelectedIndex = 0;
        }

        // Метод для загрузки продукции в ListView
        private void LoadProducts()
        {
            ProductsListView.ItemsSource = practicaEntities.GetContext().prodacts.ToList();
        }

        // Метод для применения фильтрации
        private void ApplyFilter()
        {
            var selectedCategory = CategoryFilterComboBox.SelectedItem as categ;
            var query = practicaEntities.GetContext().prodacts.AsQueryable();

            if (selectedCategory != null && selectedCategory.id != 0)
            {
                query = query.Where(p => p.id_cat == selectedCategory.id);
            }

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                query = query.Where(p => p.name_prod.StartsWith(SearchTextBox.Text));
            }

            ProductsListView.ItemsSource = query.ToList();
        }

        // Обработчик события для изменения выбора категории
        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        // Обработчик события для изменения текста в текстовом поле поиска
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        // Обработчик события для добавления нового продукта
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductEditPage());
            LoadProducts(); // Обновляем список продукции после возврата на страницу
        }

        // Обработчик события для редактирования выбранного продукта
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsListView.SelectedItem as prodact;
            if (selectedProduct != null)
            {
                NavigationService.Navigate(new ProductEditPage(selectedProduct));
                LoadProducts(); // Обновляем список продукции после возврата на страницу
            }
            else
            {
                MessageBox.Show("Выберите продукт для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик события для удаления выбранного продукта
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsListView.SelectedItem as prodact;
            if (selectedProduct != null)
            {
                // Подтверждение удаления
                var result = MessageBox.Show("Вы действительно хотите удалить выбранный продукт?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем продукт из контекста
                        practicaEntities.GetContext().prodacts.Remove(selectedProduct);
                        // Сохраняем изменения в базе данных
                        practicaEntities.GetContext().SaveChanges();
                        MessageBox.Show("Продукт успешно удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Обновляем список продуктов
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}