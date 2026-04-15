using System;

namespace MasterPolApp.Models
{
    public class EmployeeModel
    {
        public int employee_id { get; set; }
        public string full_name { get; set; }
        public string position { get; set; }
        public string passport { get; set; }
        public string phone { get; set; }
        public DateTime hire_date { get; set; }
        public string username { get; set; }
    }
}