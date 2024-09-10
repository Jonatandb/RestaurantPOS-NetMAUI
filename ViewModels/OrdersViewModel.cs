using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;

namespace RestaurantPOS.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Order> Orders { get; set; } = [];

        public OrdersViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> CreateOderAsync(CartModel[] cartItems, bool isPaidCash)
        {
            var orderItems = cartItems
                .Select(item => new OrderItem
                {
                    MenuItemId = item.ItemId,
                    Name = item.Name,
                    Icon = item.Icon,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToArray();

            var order = new OrderModel
            {
                OrderDate = DateTime.Now,
                PaymentMode = isPaidCash ? "Cash" : "Online",
                TotalAmountPaid = cartItems.Sum(i => i.Amount),
                TotalItemsCount = cartItems.Length,
                Items = orderItems
            };

            var errorMessage = await _databaseService.PlaceOrderAsync(order);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return false;
            }
            await Toast.Make("Order created successfully").Show();
            return true;
        }

        private bool _isInitialized;

        [ObservableProperty]
        private bool _isLoading;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            IsLoading = true;

            var orders = await _databaseService.GetOrdersAsync();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }

            IsLoading = false;
        }
    }
}
