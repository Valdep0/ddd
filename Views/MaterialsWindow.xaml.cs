using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MasterPolApp.Services;

namespace MasterPolApp.Views
{
    public partial class MaterialsWindow : Page
    {
        private DatabaseService _db;

        public MaterialsWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            try
            {
                var materials = _db.GetAllMaterials();
                dgMaterials.ItemsSource = materials;

                decimal totalStock = materials.Sum(m => m.stock_quantity);
                txtStats.Text = $"📦 Всего материалов на складе: {totalStock:N0} ед. | Видов материалов: {materials.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}