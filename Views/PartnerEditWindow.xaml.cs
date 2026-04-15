using MasterPolApp.Models;
using MasterPolApp.Services;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MasterPolApp.Views
{
    public partial class PartnerEditWindow : Window
    {
        private Partner _partner;
        private DatabaseService _db;
        private bool _isEditMode;

        public PartnerEditWindow(Partner partner, DatabaseService db)
        {
            InitializeComponent();
            _db = db;
            _partner = partner;
            _isEditMode = (partner != null);

            if (_isEditMode)
            {
                TitleText.Text = "Редактирование партнера";
                LoadPartnerData();
            }
            else
            {
                TitleText.Text = "Добавление партнера";
                _partner = new Partner();
            }

            sliderRating.ValueChanged += SliderRating_ValueChanged;
        }

        private void LoadPartnerData()
        {
            txtName.Text = _partner.name;
            txtDirector.Text = _partner.director;
            txtPhone.Text = _partner.phone;
            txtEmail.Text = _partner.email;
            txtAddress.Text = _partner.legal_address;
            txtInn.Text = _partner.inn;
            sliderRating.Value = _partner.rating;
            txtRatingValue.Text = _partner.rating.ToString();

            for (int i = 0; i < cmbPartnerType.Items.Count; i++)
            {
                var item = cmbPartnerType.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == _partner.partner_type)
                {
                    cmbPartnerType.SelectedIndex = i;
                    break;
                }
            }
        }

        private void SliderRating_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtRatingValue.Text = ((int)sliderRating.Value).ToString();
        }

        // ВАЛИДАЦИЯ ДАННЫХ
        private bool ValidateData()
        {
            bool isValid = true;

            // Скрываем все ошибки
            errorName.Visibility = Visibility.Collapsed;
            errorPhone.Visibility = Visibility.Collapsed;
            errorEmail.Visibility = Visibility.Collapsed;
            errorInn.Visibility = Visibility.Collapsed;

            // Проверка наименования
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorName.Text = "Наименование партнера обязательно";
                errorName.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Проверка телефона (если заполнен)
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phone = txtPhone.Text.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (!Regex.IsMatch(phone, @"^[\d\+]{10,12}$"))
                {
                    errorPhone.Text = "Введите корректный номер телефона";
                    errorPhone.Visibility = Visibility.Visible;
                    isValid = false;
                }
            }

            // Проверка email (если заполнен)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errorEmail.Text = "Введите корректный email адрес";
                    errorEmail.Visibility = Visibility.Visible;
                    isValid = false;
                }
            }

            // Проверка ИНН
            if (!string.IsNullOrWhiteSpace(txtInn.Text))
            {
                string inn = txtInn.Text.Trim();
                if (inn.Length != 10 && inn.Length != 12)
                {
                    errorInn.Text = "ИНН должен содержать 10 или 12 цифр";
                    errorInn.Visibility = Visibility.Visible;
                    isValid = false;
                }
                if (!Regex.IsMatch(inn, @"^\d+$"))
                {
                    errorInn.Text = "ИНН должен содержать только цифры";
                    errorInn.Visibility = Visibility.Visible;
                    isValid = false;
                }
            }

            return isValid;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (!ValidateData())
            {
                MessageBox.Show("Пожалуйста, исправьте ошибки в форме", "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _partner.name = txtName.Text.Trim();
                _partner.director = txtDirector.Text.Trim();
                _partner.phone = txtPhone.Text.Trim();
                _partner.email = txtEmail.Text.Trim();
                _partner.legal_address = txtAddress.Text.Trim();
                _partner.inn = txtInn.Text.Trim();
                _partner.rating = (int)sliderRating.Value;

                var selectedItem = cmbPartnerType.SelectedItem as ComboBoxItem;
                _partner.partner_type = selectedItem?.Content.ToString() ?? "ООО";

                bool success;
                if (_isEditMode)
                {
                    success = _db.UpdatePartner(_partner);
                }
                else
                {
                    success = _db.AddPartner(_partner);
                }

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}