using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using MasterPolApp.Models;

namespace MasterPolApp.Services
{
    public class DatabaseService
    {
        private string _connectionString;

        public DatabaseService()
        {
            // Имя сервера
            _connectionString = @"Data Source=PC410-11;Initial Catalog=MasterPolDB;Integrated Security=True;";
        }

        // ==================== АВТОРИЗАЦИЯ ====================
        public User Login(string username, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT u.user_id, u.username, u.role, u.email, u.is_active,
                                            e.full_name, e.position
                                     FROM Users u
                                     LEFT JOIN Employees e ON u.user_id = e.user_id
                                     WHERE u.username = @username AND u.password = @password";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    user_id = reader.GetInt32(0),
                                    username = reader.GetString(1),
                                    role = reader.GetString(2),
                                    email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    is_active = reader.GetBoolean(4),
                                    full_name = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    position = reader.IsDBNull(6) ? null : reader.GetString(6)
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при входе: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при входе: " + ex.Message);
            }
        }

        //ПАРТНЕРЫ 
        public List<Partner> GetAllPartners(string searchText = null)
        {
            var partners = new List<Partner>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT partner_id, partner_type, name, director, email, phone, 
                                            legal_address, inn, rating FROM Partners";

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " WHERE name LIKE @search OR partner_type LIKE @search OR director LIKE @search";
                        query += " ORDER BY name";
                    }
                    else
                    {
                        query += " ORDER BY name";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@search", $"%{searchText}%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                partners.Add(new Partner
                                {
                                    partner_id = reader.GetInt32(0),
                                    partner_type = reader.GetString(1),
                                    name = reader.GetString(2),
                                    director = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    email = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    phone = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    legal_address = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    inn = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    rating = reader.GetInt32(8),
                                    Sales = new List<Sale>()
                                });
                            }
                        }
                    }

                    // Загружаем продажи для каждого партнера
                    foreach (var partner in partners)
                    {
                        partner.Sales = GetSalesByPartnerId(partner.partner_id);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке партнеров: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке партнеров: " + ex.Message);
            }

            return partners;
        }

        public Partner GetPartnerById(int partnerId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT partner_id, partner_type, name, director, email, phone, 
                                            legal_address, inn, rating 
                                     FROM Partners WHERE partner_id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", partnerId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Partner
                                {
                                    partner_id = reader.GetInt32(0),
                                    partner_type = reader.GetString(1),
                                    name = reader.GetString(2),
                                    director = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    email = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    phone = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    legal_address = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    inn = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    rating = reader.GetInt32(8)
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке партнера: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке партнера: " + ex.Message);
            }

            return null;
        }

        public bool AddPartner(Partner partner)
        {
            ValidatePartner(partner);

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Partners (partner_type, name, director, email, phone, legal_address, inn, rating)
                                     VALUES (@partner_type, @name, @director, @email, @phone, @legal_address, @inn, @rating)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@partner_type", partner.partner_type);
                        cmd.Parameters.AddWithValue("@name", partner.name);
                        cmd.Parameters.AddWithValue("@director", (object)partner.director ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object)partner.email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone", (object)partner.phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@legal_address", (object)partner.legal_address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@inn", (object)partner.inn ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@rating", partner.rating);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) 
                    throw new Exception("Партнер с таким ИНН уже существует");
                throw new Exception("Ошибка базы данных при добавлении партнера: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении партнера: " + ex.Message);
            }
        }

        public bool UpdatePartner(Partner partner)
        {
            ValidatePartner(partner);

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Partners SET 
                                     partner_type = @partner_type,
                                     name = @name,
                                     director = @director,
                                     email = @email,
                                     phone = @phone,
                                     legal_address = @legal_address,
                                     inn = @inn,
                                     rating = @rating
                                     WHERE partner_id = @partner_id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@partner_id", partner.partner_id);
                        cmd.Parameters.AddWithValue("@partner_type", partner.partner_type);
                        cmd.Parameters.AddWithValue("@name", partner.name);
                        cmd.Parameters.AddWithValue("@director", (object)partner.director ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object)partner.email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone", (object)partner.phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@legal_address", (object)partner.legal_address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@inn", (object)partner.inn ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@rating", partner.rating);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    throw new Exception("Партнер с таким ИНН уже существует");
                throw new Exception("Ошибка базы данных при обновлении партнера: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при обновлении партнера: " + ex.Message);
            }
        }

        public bool DeletePartner(int partnerId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Сначала удаляем связанные продажи
                    string deleteSales = "DELETE FROM Sales WHERE partner_id = @id";
                    using (var cmd = new SqlCommand(deleteSales, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", partnerId);
                        cmd.ExecuteNonQuery();
                    }

                    // Затем удаляем партнера
                    string deletePartner = "DELETE FROM Partners WHERE partner_id = @id";
                    using (var cmd = new SqlCommand(deletePartner, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", partnerId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при удалении партнера: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении партнера: " + ex.Message);
            }
        }

        private void ValidatePartner(Partner partner)
        {
            if (string.IsNullOrWhiteSpace(partner.name))
                throw new Exception("Наименование партнера обязательно для заполнения");

            if (string.IsNullOrWhiteSpace(partner.partner_type))
                throw new Exception("Тип партнера обязателен для заполнения");

            if (!string.IsNullOrEmpty(partner.inn))
            {
                string inn = partner.inn.Trim();
                if (inn.Length != 10 && inn.Length != 12)
                    throw new Exception("ИНН должен содержать 10 или 12 цифр");

                foreach (char c in inn)
                {
                    if (!char.IsDigit(c))
                        throw new Exception("ИНН должен содержать только цифры");
                }
            }

            if (!string.IsNullOrEmpty(partner.email) && !partner.email.Contains("@"))
                throw new Exception("Введите корректный email адрес");
        }

        // ==================== ПРОДАЖИ ====================
        public List<Sale> GetSalesByPartnerId(int partnerId)
        {
            var sales = new List<Sale>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT s.sale_id, s.product_id, s.partner_id, s.quantity, s.sale_date,
                                            p.product_id, p.product_name, p.product_type, p.coefficient, p.article
                                     FROM Sales s
                                     JOIN Products p ON s.product_id = p.product_id
                                     WHERE s.partner_id = @partnerId
                                     ORDER BY s.sale_date DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@partnerId", partnerId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sales.Add(new Sale
                                {
                                    sale_id = reader.GetInt32(0),
                                    product_id = reader.GetInt32(1),
                                    partner_id = reader.GetInt32(2),
                                    quantity = reader.GetDecimal(3),
                                    sale_date = reader.GetDateTime(4),
                                    Product = new Product
                                    {
                                        product_id = reader.GetInt32(5),
                                        product_name = reader.GetString(6),
                                        product_type = reader.IsDBNull(7) ? null : reader.GetString(7),
                                        coefficient = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                                        article = reader.IsDBNull(9) ? null : reader.GetString(9)
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке продаж: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке продаж: " + ex.Message);
            }

            return sales;
        }

        public List<Sale> GetAllSales()
        {
            var sales = new List<Sale>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT s.sale_id, s.product_id, s.partner_id, s.quantity, s.sale_date,
                                            p.product_name, p.product_type, p.article,
                                            pr.name as partner_name, pr.partner_type
                                     FROM Sales s
                                     JOIN Products p ON s.product_id = p.product_id
                                     JOIN Partners pr ON s.partner_id = pr.partner_id
                                     ORDER BY s.sale_date DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(new Sale
                            {
                                sale_id = reader.GetInt32(0),
                                product_id = reader.GetInt32(1),
                                partner_id = reader.GetInt32(2),
                                quantity = reader.GetDecimal(3),
                                sale_date = reader.GetDateTime(4),
                                Product = new Product
                                {
                                    product_name = reader.GetString(5),
                                    product_type = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    article = reader.IsDBNull(7) ? null : reader.GetString(7)
                                },
                                Partner = new Partner
                                {
                                    name = reader.GetString(8),
                                    partner_type = reader.GetString(9)
                                }
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке всех продаж: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке всех продаж: " + ex.Message);
            }

            return sales;
        }

        public bool AddSale(Sale sale)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Sales (product_id, partner_id, quantity, sale_date)
                                     VALUES (@product_id, @partner_id, @quantity, @sale_date)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@product_id", sale.product_id);
                        cmd.Parameters.AddWithValue("@partner_id", sale.partner_id);
                        cmd.Parameters.AddWithValue("@quantity", sale.quantity);
                        cmd.Parameters.AddWithValue("@sale_date", sale.sale_date);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при добавлении продажи: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении продажи: " + ex.Message);
            }
        }

        // ==================== ПРОДУКЦИЯ ====================
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT product_id, product_name, product_type, coefficient, article FROM Products ORDER BY product_name";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                product_id = reader.GetInt32(0),
                                product_name = reader.GetString(1),
                                product_type = reader.IsDBNull(2) ? null : reader.GetString(2),
                                coefficient = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                                article = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке продукции: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке продукции: " + ex.Message);
            }

            return products;
        }

        public Product GetProductById(int productId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT product_id, product_name, product_type, coefficient, article FROM Products WHERE product_id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Product
                                {
                                    product_id = reader.GetInt32(0),
                                    product_name = reader.GetString(1),
                                    product_type = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    coefficient = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                                    article = reader.IsDBNull(4) ? null : reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке продукта: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке продукта: " + ex.Message);
            }

            return null;
        }

        public bool AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.product_name))
                throw new Exception("Наименование продукции обязательно");

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Products (product_name, product_type, coefficient, article)
                                     VALUES (@name, @type, @coefficient, @article)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.product_name);
                        cmd.Parameters.AddWithValue("@type", (object)product.product_type ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@coefficient", product.coefficient);
                        cmd.Parameters.AddWithValue("@article", (object)product.article ?? DBNull.Value);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при добавлении продукции: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении продукции: " + ex.Message);
            }
        }

        // ==================== МАТЕРИАЛЫ ====================
        public List<Material> GetAllMaterials()
        {
            var materials = new List<Material>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT material_id, material_name, material_type, defect_rate, stock_quantity FROM Materials ORDER BY material_name";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(new Material
                            {
                                material_id = reader.GetInt32(0),
                                material_name = reader.GetString(1),
                                material_type = reader.IsDBNull(2) ? null : reader.GetString(2),
                                defect_rate = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                                stock_quantity = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке материалов: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке материалов: " + ex.Message);
            }

            return materials;
        }

        public bool UpdateMaterialStock(int materialId, decimal newQuantity)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Materials SET stock_quantity = @quantity WHERE material_id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@quantity", newQuantity);
                        cmd.Parameters.AddWithValue("@id", materialId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при обновлении остатков: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при обновлении остатков: " + ex.Message);
            }
        }

        // ==================== ПРОИЗВОДСТВО ====================
        public List<ProductionOrder> GetAllProductionOrders()
        {
            var orders = new List<ProductionOrder>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT p.production_id, p.order_id, p.product_id, pr.product_name,
                                            p.master_user_id, u.username as master_name,
                                            p.material_id, m.material_name,
                                            p.start_date, p.end_date, p.quantity_produced, p.status
                                     FROM Production p
                                     JOIN Products pr ON p.product_id = pr.product_id
                                     JOIN Users u ON p.master_user_id = u.user_id
                                     JOIN Materials m ON p.material_id = m.material_id
                                     ORDER BY p.start_date DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new ProductionOrder
                            {
                                production_id = reader.GetInt32(0),
                                order_id = reader.GetInt32(1),
                                product_id = reader.GetInt32(2),
                                product_name = reader.GetString(3),
                                master_user_id = reader.GetInt32(4),
                                master_name = reader.GetString(5),
                                material_id = reader.GetInt32(6),
                                material_name = reader.GetString(7),
                                start_date = reader.GetDateTime(8),
                                end_date = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                                quantity_produced = reader.GetDecimal(10),
                                status = reader.GetString(11)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке производства: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке производства: " + ex.Message);
            }

            return orders;
        }

        // ==================== СОТРУДНИКИ ====================
        public List<EmployeeModel> GetAllEmployees()
        {
            var employees = new List<EmployeeModel>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT e.employee_id, e.full_name, e.position, e.passport, e.phone, e.hire_date, u.username
                                     FROM Employees e
                                     LEFT JOIN Users u ON e.user_id = u.user_id
                                     ORDER BY e.full_name";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new EmployeeModel
                            {
                                employee_id = reader.GetInt32(0),
                                full_name = reader.GetString(1),
                                position = reader.IsDBNull(2) ? null : reader.GetString(2),
                                passport = reader.IsDBNull(3) ? null : reader.GetString(3),
                                phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                                hire_date = reader.GetDateTime(5),
                                username = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке сотрудников: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке сотрудников: " + ex.Message);
            }

            return employees;
        }

        // ==================== СТАТИСТИКА ====================
        public Dictionary<string, int> GetStatistics()
        {
            var stats = new Dictionary<string, int>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        (SELECT COUNT(*) FROM Partners) as PartnersCount,
                                        (SELECT COUNT(*) FROM Products) as ProductsCount,
                                        (SELECT COUNT(*) FROM Employees) as EmployeesCount,
                                        (SELECT COUNT(*) FROM Sales) as SalesCount";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats["Партнеры"] = reader.GetInt32(0);
                            stats["Продукция"] = reader.GetInt32(1);
                            stats["Сотрудники"] = reader.GetInt32(2);
                            stats["Продажи"] = reader.GetInt32(3);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка базы данных при загрузке статистики: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке статистики: " + ex.Message);
            }

            return stats;
        }

        // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================
        public string GetRoleName(string role)
        {
            switch (role)
            {
                case "admin": return "Администратор";
                case "manager": return "Менеджер";
                case "analyst": return "Аналитик";
                case "master": return "Мастер производства";
                case "storekeeper": return "Кладовщик";
                case "hr": return "Кадровик";
                default: return "Пользователь";
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}