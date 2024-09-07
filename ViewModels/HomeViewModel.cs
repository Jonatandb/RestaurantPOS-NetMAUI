using CommunityToolkit.Mvvm.ComponentModel;
using RestaurantPOS.Data;

namespace RestaurantPOS.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public MenuCategory[] _categories = [];

        [ObservableProperty]
        private bool _isLoading;

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if(_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            IsLoading = true;

            Categories = await _databaseService.GetMenuCategoriesAsync();

            IsLoading = false;
        }
    }
}
