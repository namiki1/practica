using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для PaymentEditPage.xaml
    /// </summary>
    public partial class PaymentEditPage : Page
    {
        private practicaEntities _context; // Объявляем контекст
        private order _payment; // Объявляем экземпляр платежа

        // Конструктор с параметром для редактирования или добавления нового платежа
        public PaymentEditPage(order payment = null)
        {
            InitializeComponent();

            // Инициализация контекста
            _context = practicaEntities.GetContext();

            // Загрузка продуктов и пользователей в ComboBox
            LoadProducts();
            LoadUsers();

            // Если передан платеж, заполняем поля для редактирования, иначе создаем новый
            _payment = payment != null ? payment : new order();

            if (payment != null)
            {
                ProductComboBox.SelectedValue = payment.product_id;
                UserComboBox.SelectedValue = payment.user_id;
                PriceTextBox.Text = payment.price;
                CountTextBox.Text = payment.count;
                SumTextBox.Text = payment.sum;
            }
        }

        // Метод для загрузки продуктов в ComboBox
        private void LoadProducts()
        {
            var products = _context.prodacts.ToList();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.SelectedIndex = 0;
        }

        // Метод для загрузки пользователей в ComboBox
        private void LoadUsers()
        {
            var users = _context.Users.ToList();
            UserComboBox.ItemsSource = users;
            UserComboBox.SelectedIndex = 0;
        }

        // Обработчик события для кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, заполнены ли все поля
            if (ProductComboBox.SelectedValue == null || UserComboBox.SelectedValue == null ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) || string.IsNullOrWhiteSpace(CountTextBox.Text) ||
                string.IsNullOrWhiteSpace(SumTextBox.Text))
            {
                InfoTextBlock.Text = "Заполните все поля!";
                return;
            }

            // Сохраняем данные с формы в экземпляр платежа
            _payment.product_id = (int)ProductComboBox.SelectedValue;
            _payment.user_id = (int)UserComboBox.SelectedValue;
            _payment.price = PriceTextBox.Text.Trim();
            _payment.count = CountTextBox.Text.Trim();
            _payment.sum = SumTextBox.Text.Trim();

            // Формируем ID только для нового платежа, на единицу больше максимального ID в таблице orders
            if (_payment.id == 0)
            {
                if (_context.orders.Count() != 0)
                {
                    int maxId = _context.orders.Max(p => p.id);
                    _payment.id = maxId + 1;
                }
                else
                {
                    _payment.id = 1; // Если таблица пуста, начинаем с ID = 1
                }

                // Добавляем новый платеж в контекст
                _context.orders.Add(_payment);
            }

            // Сохраняем изменения в базе данных
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Платеж успешно сохранен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack(); // Возвращаемся на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик события для кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возвращаемся на предыдущую страницу
        }
    }
}