using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;

namespace RestaurantPOS.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<OrderModel> Orders { get; set; } = [];

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
            Orders.Add(order);
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

            Orders.Clear();

            var dbOrders = await _databaseService.GetOrdersAsync();

            var orders = dbOrders.Select(o => new OrderModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                TotalAmountPaid = o.TotalAmountPaid,
                TotalItemsCount = o.TotalItemsCount
            });

            foreach (var order in orders)
            {
                Orders.Add(order);
            }

            IsLoading = false;
        }

        [ObservableProperty]
        private OrderItem[] _orderItems = [];

        [RelayCommand]
        public async Task SelectOrderAsync(OrderModel? order)
        {
            var prevSelectedOrder = Orders.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null )
            {
                prevSelectedOrder.IsSelected = false;
                if(prevSelectedOrder.Id == order?.Id)
                {
                    OrderItems = [];
                    return;
                }
            }

            if (order == null || order.Id == 0)
            {
                OrderItems = [];
                return;
            }

            IsLoading = true;
            order.IsSelected = true;
            OrderItems = await _databaseService.GetOrderItemsByOrderIdAsync(order.Id);
            IsLoading = false;
        }
    }
}
