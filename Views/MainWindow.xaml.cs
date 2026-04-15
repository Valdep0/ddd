using System;
using System.Windows;
using MasterPolApp.Models;

namespace MasterPolApp.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // Отображаем ФИО и роль пользователя
            txtUserInfo.Text = _currentUser.DisplayName;

            // Загружаем страницу партнеров по умолчанию
            MainFrame.Navigate(new PartnersListWindow());
        }

        // НАВИГАЦИЯ ПО ПОДСИСТЕМАМ
        private void NavigateToPartners(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PartnersListWindow());
        }

        private void NavigateToProducts(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsWindow());
        }

        private void NavigateToMaterials(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MaterialsWindow());
        }

        private void NavigateToProduction(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductionWindow());
        }

        private void NavigateToEmployees(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesWindow());
        }

        // ВЫХОД ИЗ СИСТЕМЫ
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                                         "Подтверждение выхода",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}