using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp11
{
    /// <summary>
    /// Логика взаимодействия для CategoryEditPage.xaml
    /// </summary>
    public partial class CategoryEditPage : Page
    {
        private practicaEntities _context; // Объявляем контекст
        private categ _category; // Объявляем экземпляр категории

        // Конструктор с параметром для редактирования или добавления новой категории
        public CategoryEditPage(categ category = null)
        {
            InitializeComponent();

            // Инициализация контекста
            _context = practicaEntities.GetContext();

            // Если передана категория, заполняем поля для редактирования, иначе создаем новую
            _category = category != null ? category : new categ();

            if (category != null)
            {
                CategoryNameTextBox.Text = category.category_name;
            }
        }

        // Обработчик события для кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, заполнено ли поле названия категории
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                InfoTextBlock.Text = "Заполните название категории!";
                return;
            }

            // Сохраняем данные с формы в экземпляр категории
            _category.category_name = CategoryNameTextBox.Text.Trim();

            // Формируем ID только для новой категории, на единицу больше максимального ID в таблице categs
            if (_category.id == 0)
            {
                if (_context.categs.Count() != 0)
                {
                    int maxId = _context.categs.Max(c => c.id);
                    _category.id = maxId + 1;
                }
                else
                {
                    _category.id = 1; // Если таблица пуста, начинаем с ID = 1
                }

                // Добавляем новую категорию в контекст
                _context.categs.Add(_category);
            }

            // Сохраняем изменения в базе данных
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Категория успешно сохранена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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