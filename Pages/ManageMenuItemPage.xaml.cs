using RestaurantPOS.ViewModels;
using MenuItem = RestaurantPOS.Data.MenuItem;

namespace RestaurantPOS.Pages;

public partial class ManageMenuItemPage : ContentPage
{
    private readonly ManageMenuItemsViewModel _manageMenuItemViewModel;

    public ManageMenuItemPage(ManageMenuItemsViewModel manageMenuItemViewModel)
    {
        InitializeComponent();
        _manageMenuItemViewModel = manageMenuItemViewModel;
        BindingContext = _manageMenuItemViewModel;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await _manageMenuItemViewModel.InitializeAsync();
    }

    private async void OnCategorySelected(Models.MenuCategoryModel category) => await _manageMenuItemViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);

    private async void OnItemSelected(MenuItem menuItem) => await _manageMenuItemViewModel.EditMenuItemCommand.ExecuteAsync(menuItem);

    private void SaveMenuItemFormControl_OnCancel()
    {
        _manageMenuItemViewModel.CancelCommand.Execute(null);
    }

    private async void SaveMenuItemFormControl_OnSaveItem(Models.MenuItemModel menuItemModel)
    {
        await _manageMenuItemViewModel.SaveMenuItemCommand.ExecuteAsync(menuItemModel);
    }
}