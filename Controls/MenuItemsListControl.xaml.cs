using CommunityToolkit.Mvvm.Input;
using MenuItem = RestaurantPOS.Data.MenuItem;

namespace RestaurantPOS.Controls;

public partial class MenuItemsListControl : ContentView
{
    public MenuItemsListControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
        nameof(Items),
        typeof(MenuItem[]),
        typeof(MenuItemsListControl),
        Array.Empty<MenuItem>()
    );

    public MenuItem[] Items
    {
        get => (MenuItem[])GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public event Action<MenuItem> OnItemSelected;

    [RelayCommand]
    private void ItemSelected(MenuItem item) => OnItemSelected?.Invoke(item);

    public string ActionIcon { get; set; } = "shopping_bag.png";

    public bool IsEditingMode { set => ActionIcon = (value ? "edit_solid_24.png" : "shopping_bag.png"); }
}