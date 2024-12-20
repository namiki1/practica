using System.Windows;

namespace WpfApp11
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запуск окна авторизации первым
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}