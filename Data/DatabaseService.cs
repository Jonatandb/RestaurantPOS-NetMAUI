using RestaurantPOS.Models;
using SQLite;

namespace RestaurantPOS.Data
{
    public class DatabaseService : IAsyncDisposable
    {
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RestaurantPOS.db3");

            _connection = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        }

        public async Task InitializeDatabase()
        {
            await _connection.CreateTableAsync<MenuCategory>();
            await _connection.CreateTableAsync<MenuItem>();
            await _connection.CreateTableAsync<MenuItemCategoryMapping>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<OrderItem>();

            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            var firstCategory = await _connection.Table<MenuCategory>().FirstOrDefaultAsync();

            if (firstCategory != null)
            {
                return; // DB has been seeded
            }

            var categories = SeedData.GetMenuCategories();
            var menuItems = SeedData.GetMenuItems();
            var mappings = SeedData.GetMenuItemCategoryMappings();

            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItems);
            await _connection.InsertAllAsync(mappings);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<MenuCategory[]> GetMenuCategoriesAsync() => await _connection.Table<MenuCategory>().ToArrayAsync();

        public async Task<MenuItem[]> GetMenuItemsByCategoryIdAsync(int categoryId)
        {
            var query = @"
                            SELECT mi.*
                            FROM MenuItem AS mi
                                INNER JOIN MenuItemCategoryMapping AS mcm
                                    ON mi.Id = mcm.MenuItemId
                            WHERE mcm.CategoryId = ?
                        ";
            var menuItems = await _connection.QueryAsync<MenuItem>(query, categoryId);

            return [.. menuItems];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>String with error message if any. Null if no error.</returns>
        public async Task<string?> PlaceOrderAsync(OrderModel model)
        {
            var newOrder = new Order
            {
                OrderDate = model.OrderDate,
                PaymentMode = model.PaymentMode,
                TotalAmountPaid = model.TotalAmountPaid,
                TotalItemsCount = model.TotalItemsCount
            };

            if (await _connection.InsertAsync(newOrder) > 0)
            {
                foreach (var item in model.Items)
                {
                    item.OrderId = newOrder.Id;
                }
                if (await _connection.InsertAllAsync(model.Items) == 0)
                {
                    await _connection.DeleteAsync(newOrder);
                    return "Error inserting order items";
                }
            }
            else
            {
                return "Error inserting order";
            }
            model.Id = newOrder.Id;
            return null;
        }

        public async Task<Order[]> GetOrdersAsync() => await _connection.Table<Order>().OrderByDescending(o => o.OrderDate).ToArrayAsync();

        public async Task<OrderItem[]> GetOrderItemsByOrderIdAsync(int orderId) => await _connection
            .Table<OrderItem>()
            .Where(oi => oi.OrderId == orderId)
            .ToArrayAsync();

        public async Task<MenuCategory[]> GetCategoriesByMenuItemIdAsync(int menuItemId)
        {
            var query = @"
                        SELECT cat.* 
                        FROM MenuCategory cat
                        INNER JOIN MenuItemCategoryMapping mcm
                        ON cat.Id = mcm.CategoryId
                        WHERE mcm.MenuItemId = ?
                    ";
            var categories = await _connection.QueryAsync<MenuCategory>(query, menuItemId);
            return [.. categories];
        }

        public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
        {
            if (model.Id == 0)
            {
                MenuItem menuItem = new()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Icon = model.Icon,
                    Description = model.Description,
                    Price = model.Price
                };

                if (await _connection.InsertAsync(menuItem) > 0)
                {
                    var categoryMapping = model.SelectedCategories
                                                .Select(c => new MenuItemCategoryMapping
                                                {
                                                    Id = c.Id,
                                                    CategoryId = c.Id,
                                                    MenuItemId = menuItem.Id
                                                });
                    if (await _connection.InsertAllAsync(categoryMapping) > 0)
                    {
                        model.Id = menuItem.Id;
                        return null;
                    }
                    else
                    {
                        await _connection.DeleteAsync(menuItem);
                    }
                }
                return "Error saving menu item";
            }
            else
            {
                string? errorMessage = null;

                await _connection.RunInTransactionAsync(db =>
                {
                    var menuItem = db.Find<MenuItem>(model.Id);

                    menuItem.Name = model.Name;
                    menuItem.Icon = model.Icon;
                    menuItem.Description = model.Description;
                    menuItem.Price = model.Price;

                    if (db.Update(menuItem) == 0)
                    {
                        errorMessage = "Error updating menu item";
                        throw new Exception();
                    }

                    var deleteQuery = @"
                        DELETE FROM MenuItemCategoryMapping 
                        WHERE MenuItemId = ?";
                    db.Execute(deleteQuery, menuItem.Id);

                    var categoryMapping = model.SelectedCategories
                            .Select(c => new MenuItemCategoryMapping
                            {
                                Id = c.Id,
                                CategoryId = c.Id,
                                MenuItemId = menuItem.Id
                            });
                    if (db.InsertAll(categoryMapping) == 0)
                    {
                        errorMessage = "Error updating menu item categories";
                        throw new Exception();
                    }
                });

                return errorMessage;
            }
        }
    }
}
