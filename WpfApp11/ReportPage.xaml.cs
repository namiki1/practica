using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    public partial class ReportPage : Page
    {
        private order _selectedPayment;

        public ReportPage(order selectedPayment)
        {
            InitializeComponent();
            _selectedPayment = selectedPayment;
            LoadPaymentDetails();
        }

        // Метод для загрузки данных о платеже
        private void LoadPaymentDetails()
        {
            ProductNameTextBlock.Text = _selectedPayment.prodact.name_prod;
            UserNameTextBlock.Text = _selectedPayment.User.family_name;
            PriceTextBlock.Text = _selectedPayment.price;
            CountTextBlock.Text = _selectedPayment.count;

            // Очистка строк от нечисловых символов
            string cleanedPrice = CleanNumericString(_selectedPayment.price);
            string cleanedCount = CleanNumericString(_selectedPayment.count);

            // Проверка и преобразование price с учетом локальных настроек
            if (!decimal.TryParse(cleanedPrice, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price))
            {
                MessageBox.Show("Некорректное значение для цены: " + _selectedPayment.price);
                return; // Прерываем выполнение, если цена некорректна
            }

            // Проверка и преобразование count
            if (!int.TryParse(cleanedCount, out int count))
            {
                MessageBox.Show("Некорректное значение для количества: " + _selectedPayment.count);
                return; // Прерываем выполнение, если количество некорректно
            }

            // Расчет суммы
            decimal sum = price * count;
            SumTextBlock.Text = sum.ToString("F2", CultureInfo.CurrentCulture);

            // Расчет скидки
            decimal discount = CalculateDiscount(sum);
            DiscountTextBlock.Text = discount.ToString("F2", CultureInfo.CurrentCulture);

            // Расчет итоговой суммы
            decimal totalSum = sum - discount;
            TotalSumTextBlock.Text = totalSum.ToString("F2", CultureInfo.CurrentCulture);
        }

        // Метод для расчета скидки
        private decimal CalculateDiscount(decimal sum)
        {
            if (sum >= 10000 && sum < 50000)
            {
                return sum * 0.05m;
            }
            else if (sum >= 50000 && sum < 300000)
            {
                return sum * 0.10m;
            }
            else if (sum >= 300000)
            {
                return sum * 0.15m;
            }
            return 0;
        }

        // Обработчик события для кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Метод для очистки строки от нечисловых символов
        private string CleanNumericString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "0"; // Возвращаем 0, если строка пустая

            // Убираем все символы, кроме цифр, точки и запятой
            return new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
        }

        // Метод для проверки, является ли строка корректной ценой
        private bool IsValidPrice(string input)
        {
            // Проверяем, что строка содержит только цифры, точку или запятую
            return input.All(c => char.IsDigit(c) || c == '.' || c == ',');
        }
    }
}