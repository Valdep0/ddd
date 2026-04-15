using System;
using System.Windows;
using System.Windows.Controls;
using MasterPolApp.Services;

namespace MasterPolApp.Views
{
    public partial class ProductsWindow : Page
    {
        private DatabaseService _db;

        public ProductsWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _db.GetAllProducts();
                dgProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Title = "Добавление продукции",
                Width = 450,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Наименование *", FontSize = 13, Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 0, 0, 5) });
            var txtName = new TextBox { Height = 35, Margin = new Thickness(0, 0, 0, 15), FontSize = 14 };
            stack.Children.Add(txtName);

            stack.Children.Add(new TextBlock { Text = "Тип продукции", FontSize = 13, Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 0, 0, 5) });
            var txtType = new TextBox { Height = 35, Margin = new Thickness(0, 0, 0, 15), FontSize = 14 };
            stack.Children.Add(txtType);

            stack.Children.Add(new TextBlock { Text = "Артикул", FontSize = 13, Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 0, 0, 5) });
            var txtArticle = new TextBox { Height = 35, Margin = new Thickness(0, 0, 0, 15), FontSize = 14 };
            stack.Children.Add(txtArticle);

            stack.Children.Add(new TextBlock { Text = "Коэффициент", FontSize = 13, Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 0, 0, 5) });
            var txtCoeff = new TextBox { Height = 35, Margin = new Thickness(0, 0, 0, 15), FontSize = 14, Text = "1.0" };
            stack.Children.Add(txtCoeff);

            var btnSave = new Button
            {
                Content = "Сохранить",
                Height = 40,
                Background = System.Windows.Media.Brushes.LightGreen,
                Margin = new Thickness(0, 10, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            };

            btnSave.Click += (s, args) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show("Введите наименование продукции!", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    decimal coeff = 1.0m;
                    if (!decimal.TryParse(txtCoeff.Text, out coeff))
                        coeff = 1.0m;

                    var product = new Models.Product
                    {
                        product_name = txtName.Text.Trim(),
                        product_type = txtType.Text.Trim(),
                        article = txtArticle.Text.Trim(),
                        coefficient = coeff
                    };

                    if (_db.AddProduct(product))
                    {
                        LoadProducts();
                        window.Close();
                        MessageBox.Show("Продукция успешно добавлена!", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            stack.Children.Add(btnSave);
            window.Content = stack;
            window.ShowDialog();
        }
    }
}