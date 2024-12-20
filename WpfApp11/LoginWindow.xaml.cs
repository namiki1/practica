using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp11
{
    public partial class LoginWindow : Window
    {
        private int _failedAttempts = 0;
        private string _captcha;
        private bool _isBlocked = false;

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isBlocked)
            {
                MessageBox.Show("Окно авторизации заблокировано на 30 секунд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_failedAttempts >= 1)
            {
                if (string.IsNullOrEmpty(CaptchaTextBox.Text) || CaptchaTextBox.Text != _captcha)
                {
                    MessageBox.Show("Неверная CAPTCHA.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _failedAttempts++;
                    CheckBlock();
                    return;
                }
            }

            var user = practicaEntities.GetContext().Users.FirstOrDefault(u => u.login == login);

            if (user != null && VerifyPassword(password, user.password))
            {
                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                new MainWindow().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _failedAttempts++;
                CheckBlock();
            }
        }

        private void CheckBlock()
        {
            if (_failedAttempts >= 3)
            {
                _isBlocked = true;
                MessageBox.Show("Окно авторизации заблокировано на 30 секунд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Task.Delay(30000).ContinueWith(_ =>
                {
                    _isBlocked = false;
                    _failedAttempts = 0;
                    Dispatcher.Invoke(() => MessageBox.Show("Окно авторизации разблокировано.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information));
                });
            }
            else if (_failedAttempts >= 1)
            {
                CaptchaTextBlock.Visibility = Visibility.Visible;
                CaptchaTextBox.Visibility = Visibility.Visible;
                RefreshCaptchaButton.Visibility = Visibility.Visible;
                GenerateCaptcha();
            }
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            _captcha = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            CaptchaTextBlock.Text = $"Введите CAPTCHA: {_captcha}";
        }

        private void RefreshCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                var hashedInputString = BitConverter.ToString(hashedInput).Replace("-", "").ToLower();
                return hashedInputString == hashedPassword;
            }
        }

        // Метод для хеширования пароля с использованием SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}