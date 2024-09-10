using RestaurantPOS.ViewModels;

namespace RestaurantPOS.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly OrdersViewModel _ordersViewModel;

    public OrdersPage(OrdersViewModel ordersViewModel)
	{
		InitializeComponent();
        _ordersViewModel = ordersViewModel;
        BindingContext = _ordersViewModel;
        InitializeViewModelAsync();
    }

    private async void InitializeViewModelAsync() => await _ordersViewModel.InitializeAsync();

}