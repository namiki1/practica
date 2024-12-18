using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для ProductEditPage.xaml
    /// </summary>
    public partial class ProductEditPage : Page
    {
        private practicaEntities _context; // Объявляем контекст
        private prodact _product; // Объявляем экземпляр продукции

        // Конструктор с параметром для редактирования или добавления нового продукта
        public ProductEditPage(prodact product = null)
        {
            InitializeComponent();

            // Инициализация контекста
            _context = practicaEntities.GetContext();

            // Загрузка категорий в ComboBox
            var categories = _context.categs.ToList();
            categories.Insert(0, new categ { id = 0, category_name = "Выберите категорию" });
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;

            // Если передан продукт, заполняем поля для редактирования, иначе создаем новый
            _product = product != null ? product : new prodact();

            if (product != null)
            {
                ProductNameTextBox.Text = product.name_prod;
                CategoryComboBox.SelectedValue = product.id_cat; // Исправлено: присваиваем id_cat, а не categ
            }
        }

        // Обработчик события для кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, заполнены ли все поля
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text) || (CategoryComboBox.SelectedValue == null || (int)CategoryComboBox.SelectedValue == 0))
            {
                InfoTextBlock.Text = "Заполните все поля!";
                return;
            }

            // Сохраняем данные с формы в экземпляр продукта
            _product.name_prod = ProductNameTextBox.Text.Trim();
            _product.id_cat = (int)CategoryComboBox.SelectedValue; // Исправлено: присваиваем id_cat, а не id

            // Формируем ID только для нового продукта, на единицу больше максимального ID в таблице Products
            if (_product.id == 0)
            {
                if (_context.prodacts.Count() != 0)
                {
                    int maxId = _context.prodacts.Max(p => p.id);
                    _product.id = maxId + 1;
                }
                else
                {
                    _product.id = 1; // Если таблица пуста, начинаем с ID = 1
                }

                // Добавляем новый продукт в контекст
                _context.prodacts.Add(_product);
            }

            // Сохраняем изменения в базе данных
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Продукт успешно сохранен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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