using SQLite;

namespace RestaurantPOS.Data
{
    public class MenuItemCategoryMapping
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public int CategoryId { get; set; }
    }
}
