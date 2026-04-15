using System;

namespace MasterPolApp.Models
{
    public class ProductionOrder
    {
        public int production_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int master_user_id { get; set; }
        public string master_name { get; set; }
        public int material_id { get; set; }
        public string material_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public decimal quantity_produced { get; set; }
        public string status { get; set; }
    }
}