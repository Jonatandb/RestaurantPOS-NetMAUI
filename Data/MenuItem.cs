using SQLite;

namespace RestaurantPOS.Data
{
    public class MenuItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
