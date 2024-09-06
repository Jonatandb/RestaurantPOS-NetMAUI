using SQLite;

namespace RestaurantPOS.Data
{
    public class Order 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalItemsCount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string PaymentMode { get; set; } // Cash or Online
    }
}
