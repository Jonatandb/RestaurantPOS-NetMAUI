using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Models;

namespace RestaurantPOS.Controls;

public partial class CategoriesListControl : ContentView
{
    public CategoriesListControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CategoriesProperty = BindableProperty.Create(
        nameof(Categories),
        typeof(MenuCategoryModel[]),
        typeof(CategoriesListControl),
        Array.Empty<MenuCategoryModel>()
    );

    public MenuCategoryModel[] Categories
    {
        get => (MenuCategoryModel[])GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public event Action<MenuCategoryModel> OnCategorySelected;

    [RelayCommand]
    private void SelectCategory(MenuCategoryModel category) => OnCategorySelected?.Invoke(category);
}