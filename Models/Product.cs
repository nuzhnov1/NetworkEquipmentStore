namespace NetworkEquipmentStore.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProductCategory Category { get; set; }
        public string ImageName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
