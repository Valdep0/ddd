using MasterPolApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MasterPolApp.Views
{
    public partial class LoginWindow : Window
    {
        private AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var loginButton = sender as Button;
            loginButton.IsEnabled = false;

            try
            {
                var user = _authService.Login(username, password);

                if (user != null)
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!\n\n" +
                                   "Доступные логины из БД:\n" +
                                   "admin, ivanov, petrova, sidorov, kuznetsov, hr_director",
                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    loginButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                loginButton.IsEnabled = true;
            }
        }
    }
}