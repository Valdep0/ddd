using MasterPolApp.Models;
using MasterPolApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MasterPolApp.Views
{
    public partial class SalesHistoryWindow : Window
    {
        private Partner _partner;
        private DatabaseService _db;
        private List<Sale> _allSales;

        public SalesHistoryWindow(Partner partner, DatabaseService db)
        {
            InitializeComponent();
            _partner = partner;
            _db = db;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                TitleText.Text = $"История продаж: {_partner.name}";
                txtPartnerName.Text = $"{_partner.name} ({_partner.partner_type})";

                _allSales = _db.GetSalesByPartnerId(_partner.partner_id);

                decimal total = _allSales.Sum(s => s.quantity);
                txtTotalSales.Text = $"Общий объем: {total:N0} м²";
                txtDiscount.Text = $"Скидка: {_partner.DiscountPercent}%";

                txtPartnerInfo.Text = $"Рейтинг: {_partner.rating} | Всего продаж: {_allSales.Count}";

                dgSales.ItemsSource = _allSales;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Period_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_allSales == null) return;

            var selected = (cmbPeriod.SelectedItem as ComboBoxItem).Content.ToString();
            DateTime startDate = DateTime.MinValue;

            switch (selected)
            {
                case "Последний месяц":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "Последний квартал":
                    startDate = DateTime.Now.AddMonths(-3);
                    break;
                case "Последний год":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
                default:
                    dgSales.ItemsSource = _allSales;
                    return;
            }

            var filtered = _allSales.Where(s => s.sale_date >= startDate).ToList();
            dgSales.ItemsSource = filtered;

            // Обновляем статистику по фильтру
            decimal total = filtered.Sum(s => s.quantity);
            txtTotalSales.Text = $"Общий объем: {total:N0} м²";
        }
    }
}