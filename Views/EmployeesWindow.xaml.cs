using System;
using System.Windows;
using System.Windows.Controls;
using MasterPolApp.Services;

namespace MasterPolApp.Views
{
    public partial class EmployeesWindow : Page
    {
        private DatabaseService _db;

        public EmployeesWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _db.GetAllEmployees();
                dgEmployees.ItemsSource = employees;
                txtStats.Text = $"👥 Всего сотрудников: {employees.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}