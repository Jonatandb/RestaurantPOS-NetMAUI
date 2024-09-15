using CommunityToolkit.Mvvm.ComponentModel;
using RestaurantPOS.Data;

namespace RestaurantPOS.Models
{
    public partial class OrderModel : ObservableObject
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalItemsCount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string PaymentMode { get; set; } // Cash or Online

        public OrderItem[] Items { get; set; }

        [ObservableProperty]
        private bool _isSelected;
    }
}
