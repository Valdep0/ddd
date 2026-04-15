using System;

namespace MasterPolApp.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public bool is_active { get; set; }
        public DateTime created_date { get; set; }
        public DateTime? last_login { get; set; }

        // Дополнительные поля из Employees
        public string full_name { get; set; }
        public string position { get; set; }

        // Отображаемое имя (ФИО + роль) - для задания 5c
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(full_name))
                    return $"{full_name} ({GetRoleName(role)})";
                return $"{username} ({GetRoleName(role)})";
            }
        }

        private string GetRoleName(string role)
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
    }
}