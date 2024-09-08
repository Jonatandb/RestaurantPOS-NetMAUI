using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;
using MenuItem = RestaurantPOS.Data.MenuItem;

namespace RestaurantPOS.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private MenuCategoryModel? _selectedCategory = null;

        public ObservableCollection<CartModel> CartItems { get; set; } = new();

        [ObservableProperty]
        private bool _isLoading;

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            IsLoading = true;

            Categories = (await _databaseService.GetMenuCategoriesAsync())
                            .Select(MenuCategoryModel.FromEntity)
                            .ToArray();

            Categories[0].IsSelected = true;
            SelectedCategory = Categories[0];

            MenuItems = await _databaseService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);

            IsLoading = false;
        }

        [RelayCommand]
        private async Task SelectCategoryAsync(int categoryId)
        {
            if (SelectedCategory?.Id == categoryId)
                return; // Already selected

            IsLoading = true;

            var currentSelectedCategory = Categories.First(c => c.IsSelected);
            currentSelectedCategory.IsSelected = false;

            var newSelectedCategory = Categories.First(c => c.Id == categoryId);
            newSelectedCategory.IsSelected = true;

            SelectedCategory = newSelectedCategory;

            MenuItems = await _databaseService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);

            IsLoading = false;
        }

        [RelayCommand]
        private void AddToCart(MenuItem menuItem)
        {
            var cartItem = CartItems.FirstOrDefault(c => c.ItemId == menuItem.Id);
            if(cartItem == null)
            {
                cartItem = new CartModel()
                {
                    ItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Icon = menuItem.Icon,
                    Quantity = 1
                };
                CartItems.Add(cartItem);
            } else
            {
                cartItem.Quantity++;
            }
        }

        [RelayCommand]
        private void IncreaseQuantity(CartModel cartItem) => cartItem.Quantity++;

        [RelayCommand]
        private void DecreaseQuantity(CartModel cartItem)
        {
            cartItem.Quantity--;
            if (cartItem.Quantity == 0)
            {
                CartItems.Remove(cartItem);
            }
        }

        [RelayCommand]
        private void RemoveItemFromCart(CartModel cartItem) => CartItems.Remove(cartItem);
    }
}
