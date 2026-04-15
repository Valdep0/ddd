using System;

namespace MasterPolApp.Models
{
    public class Sale
    {
        public int sale_id { get; set; }
        public int product_id { get; set; }
        public int partner_id { get; set; }
        public decimal quantity { get; set; }
        public DateTime sale_date { get; set; }

        public Product Product { get; set; }
        public Partner Partner { get; set; }
    }
}