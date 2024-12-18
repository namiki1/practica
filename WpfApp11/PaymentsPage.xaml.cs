using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для PaymentsPage.xaml
    /// </summary>
    public partial class PaymentsPage : Page
    {
        public PaymentsPage()
        {
            InitializeComponent();
            LoadPayments();
            LoadCategories();
        }

        // Метод для загрузки платежей в ListView
        private void LoadPayments()
        {
            PaymentsListView.ItemsSource = practicaEntities.GetContext().orders.ToList();
        }

        // Метод для загрузки категорий в ComboBox
        private void LoadCategories()
        {
            var categories = practicaEntities.GetContext().categs.ToList();
            categories.Insert(0, new categ { id = 0, category_name = "Все категории" });
            CategoryFilterComboBox.ItemsSource = categories;
            CategoryFilterComboBox.SelectedIndex = 0;
        }

        // Метод для применения фильтра
        private void ApplyFilter()
        {
            var query = practicaEntities.GetContext().orders.AsQueryable();

            // Фильтр по категории
            var selectedCategory = CategoryFilterComboBox.SelectedItem as categ;
            if (selectedCategory != null && selectedCategory.id != 0)
            {
                query = query.Where(p => p.prodact.id_cat == selectedCategory.id);
            }

            // Фильтр по поиску
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                query = query.Where(p => p.prodact.name_prod.Contains(SearchTextBox.Text));
            }

            PaymentsListView.ItemsSource = query.ToList();
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

        // Обработчик события для добавления нового платежа
        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PaymentEditPage());
            LoadPayments(); // Обновляем список платежей после возврата на страницу
        }

        // Обработчик события для редактирования выбранного платежа
        private void EditPayment_Click(object sender, RoutedEventArgs e)
        {
            var selectedPayment = PaymentsListView.SelectedItem as order;
            if (selectedPayment != null)
            {
                NavigationService.Navigate(new PaymentEditPage(selectedPayment));
                LoadPayments(); // Обновляем список платежей после возврата на страницу
            }
            else
            {
                MessageBox.Show("Выберите платеж для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик события для удаления выбранного платежа
        private void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
            var selectedPayment = PaymentsListView.SelectedItem as order;
            if (selectedPayment != null)
            {
                // Подтверждение удаления
                var result = MessageBox.Show("Вы действительно хотите удалить выбранный платеж?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем платеж из контекста
                        practicaEntities.GetContext().orders.Remove(selectedPayment);
                        // Сохраняем изменения в базе данных
                        practicaEntities.GetContext().SaveChanges();
                        MessageBox.Show("Платеж успешно удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Обновляем список платежей
                        LoadPayments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите платеж для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}