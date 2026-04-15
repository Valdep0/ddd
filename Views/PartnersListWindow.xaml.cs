using MasterPolApp.Models;
using MasterPolApp.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MasterPolApp.Views
{
    public partial class PartnersListWindow : Page
    {
        private DatabaseService _db;
        private string _currentSearch = "";

        public PartnersListWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadPartners();
        }

        private void LoadPartners()
        {
            try
            {
                var partners = _db.GetAllPartners(_currentSearch);
                PartnersList.ItemsSource = partners;

                // Обновляем статистику
                txtStats.Text = $"📋 Всего партнеров: {partners.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ПОИСК
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _currentSearch = txtSearch.Text.Trim();
            LoadPartners();
        }

        // СБРОС ПОИСКА
        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            _currentSearch = "";
            LoadPartners();
        }

        private void AddPartnerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editWindow = new PartnerEditWindow(null, _db);
                editWindow.Owner = Window.GetWindow(this);

                if (editWindow.ShowDialog() == true)
                {
                    LoadPartners();
                    MessageBox.Show("Партнер успешно добавлен!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var partner = button.Tag as Partner;

                var editWindow = new PartnerEditWindow(partner, _db);
                editWindow.Owner = Window.GetWindow(this);

                if (editWindow.ShowDialog() == true)
                {
                    LoadPartners();
                    MessageBox.Show("Данные партнера обновлены!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var partner = button.Tag as Partner;

                var historyWindow = new SalesHistoryWindow(partner, _db);
                historyWindow.Owner = Window.GetWindow(this);
                historyWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}