using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MasterPolApp.Services;

namespace MasterPolApp.Views
{
    public partial class ProductionWindow : Page
    {
        private DatabaseService _db;

        public ProductionWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadProduction();
        }

        private void LoadProduction()
        {
            try
            {
                var orders = _db.GetAllProductionOrders();
                dgProduction.ItemsSource = orders;

                int activeCount = orders.Count(o => o.status == "В процессе" || o.status == "planned");
                txtStats.Text = $"🏭 Активных заказов: {activeCount} | Всего заказов: {orders.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}