namespace MasterPolApp.Models
{
    public class Material
    {
        public int material_id { get; set; }
        public string material_name { get; set; }
        public string material_type { get; set; }
        public decimal defect_rate { get; set; }
        public decimal stock_quantity { get; set; }
    }
}