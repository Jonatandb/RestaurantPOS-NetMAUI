using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;
using MenuItem = RestaurantPOS.Data.MenuItem;

namespace RestaurantPOS.ViewModels
{
    public partial class HomeViewModel : ObservableObject, IRecipient<MenuItemChangedMessage>
    {
        private readonly DatabaseService _databaseService;
        private readonly OrdersViewModel _ordersViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private MenuCategoryModel? _selectedCategory = null;

        public ObservableCollection<CartModel> CartItems { get; set; } = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(TaxAmount)), NotifyPropertyChangedFor(nameof(Total))]
        private decimal _subtotal;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(TaxAmount)), NotifyPropertyChangedFor(nameof(Total))]
        private int _taxPrecentage;

        public decimal TaxAmount => (Subtotal * TaxPrecentage) / 100;

        public decimal Total => Subtotal + TaxAmount;

        [ObservableProperty]
        private string _name = "Guest";

        public HomeViewModel(DatabaseService databaseService, OrdersViewModel ordersViewModel, SettingsViewModel settingsViewModel)
        {
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _settingsViewModel = settingsViewModel;

            CartItems.CollectionChanged += (sender, args) => RecalculateAmounts();

            WeakReferenceMessenger.Default.Register<MenuItemChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, (reciepent, message) => Name = message.Value);

            TaxPrecentage = _settingsViewModel.GetTaxPercentage();
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
            if (cartItem == null)
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
            }
            else
            {
                cartItem.Quantity++;
                RecalculateAmounts();
            }
        }

        [RelayCommand]
        private void IncreaseQuantity(CartModel cartItem)
        {
            cartItem.Quantity++;
            RecalculateAmounts();
        }

        [RelayCommand]
        private void DecreaseQuantity(CartModel cartItem)
        {
            cartItem.Quantity--;
            if (cartItem.Quantity == 0)
            {
                CartItems.Remove(cartItem);
            }
            else
            {
                RecalculateAmounts();
            }
        }

        [RelayCommand]
        private void RemoveItemFromCart(CartModel cartItem) => CartItems.Remove(cartItem);

        private void RecalculateAmounts() => Subtotal = CartItems.Sum(i => i.Amount);

        [RelayCommand]
        private async Task TaxPercentageClickAsync()
        {
            var result = await Shell.Current.DisplayPromptAsync("Tax Percentage", "Enter tax percentage", placeholder: "10", initialValue: TaxPrecentage.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (!int.TryParse(result, out int enteredTaxPercentage))
                {
                    await Shell.Current.DisplayAlert("Invalid value", "Please enter a valid number", "OK");
                    return;
                }

                if (enteredTaxPercentage > 100 || enteredTaxPercentage < 0)
                {
                    await Shell.Current.DisplayAlert("Invalid value", "Tax percentage must be between 0 and 100", "OK");
                    return;
                }

                TaxPrecentage = enteredTaxPercentage;

                _settingsViewModel.SetTaxPercentage(enteredTaxPercentage);
            }
        }

        [RelayCommand]
        private async Task ClearCartAsync()
        {
            if (CartItems.Count > 0)
            {
                if (await Shell.Current.DisplayAlert("Clear Order", "Are you sure you want to clear the order?", "Yes", "No"))
                {
                    CartItems.Clear();
                }
            }
        }

        [RelayCommand]
        private async Task PlaceOrderAsync(bool isPaidCash)
        {
            if (CartItems.Count == 0)
            {
                return;
            }

            if (await Shell.Current.DisplayAlert("Close Order", "Are you sure you want to close the order?", "Yes", "No"))
            {
                IsLoading = true;
                if (await _ordersViewModel.CreateOderAsync([.. CartItems], isPaidCash))
                {
                    CartItems.Clear();
                }
                IsLoading = false;
            }
        }

        public void Receive(MenuItemChangedMessage message)
        {
            var model = message.Value;
            var menuItem = MenuItems.FirstOrDefault(m => m.Id == model.Id);
            if (menuItem != null)
            {

                if (!model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
                {
                    MenuItems = [.. MenuItems.Where(m => m.Id != model.Id)];
                    return;
                }

                menuItem.Name = model.Name;
                menuItem.Price = model.Price;
                menuItem.Description = model.Description;
                menuItem.Icon = model.Icon;

                MenuItems = [.. MenuItems];
            }
            else if (model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
            {
                var newMenuItem = new MenuItem
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    Description = model.Description,
                    Icon = model.Icon
                };

                MenuItems = [.. MenuItems, newMenuItem];
            }

            var cartItem = CartItems.FirstOrDefault(i => i.ItemId == model.Id);
            if (cartItem != null)
            {

                cartItem.Name = model.Name;
                cartItem.Price = model.Price;
                cartItem.Icon = model.Icon;

                var itemIndex = CartItems.IndexOf(cartItem);

                CartItems[itemIndex] = cartItem;

            }
        }
    }
}
