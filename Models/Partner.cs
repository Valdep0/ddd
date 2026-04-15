using System.Collections.Generic;

namespace MasterPolApp.Models
{
    public class Partner
    {
        public int partner_id { get; set; }
        public string partner_type { get; set; }
        public string name { get; set; }
        public string director { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string legal_address { get; set; }
        public string inn { get; set; }
        public int rating { get; set; }

        public List<Sale> Sales { get; set; }

        public decimal TotalSalesQuantity
        {
            get
            {
                if (Sales == null || Sales.Count == 0) return 0;
                decimal total = 0;
                foreach (var sale in Sales) total += sale.quantity;
                return total;
            }
        }

        public int DiscountPercent
        {
            get
            {
                decimal total = TotalSalesQuantity;
                if (total >= 100000) return 15;
                if (total >= 50000) return 12;
                if (total >= 25000) return 10;
                if (total >= 10000) return 7;
                if (total >= 5000) return 5;
                return 0;
            }
        }
    }
}