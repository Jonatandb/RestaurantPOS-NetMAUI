using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Models;

namespace RestaurantPOS.Controls;

public partial class SaveMenuItemFormControl : ContentView
{
    private const string DefaultIcon = "image_add_regular_36.png";

    public SaveMenuItemFormControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
       nameof(Item),
       typeof(MenuItemModel),
       typeof(SaveMenuItemFormControl),
       new MenuItemModel(),
       propertyChanged: OnItemChanged
   );

    public MenuItemModel Item
    {
        get => (MenuItemModel)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    [RelayCommand]
    private void ToggleCategorySelection(MenuCategoryModel category) => category.IsSelected = !category.IsSelected;

    private static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is MenuItemModel menuItemModel)
        {
            if (bindable is SaveMenuItemFormControl thisControl)
            {
                if (menuItemModel.Id > 0)
                {
                    thisControl.SetIconImage(isDefault: false, menuItemModel.Icon, thisControl);
                    thisControl.ExistingIcon = menuItemModel.Icon;
                }
                else
                {
                    thisControl.SetIconImage(isDefault: true, null, thisControl);
                }
            }
        }
    }

    public string? ExistingIcon { get; set; }

    public event Action? OnCancel;

    [RelayCommand]
    private void Cancel() => OnCancel?.Invoke();

    private async void PickImageButton_Clicked(object sender, EventArgs e)
    {
        var fileResult = await MediaPicker.PickPhotoAsync();

        if (fileResult != null)
        {
            var imageStream = await fileResult.OpenReadAsync();

            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileResult.FileName);

            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);

            await imageStream.CopyToAsync(fs);

            SetIconImage(isDefault: false, localPath);

            Item.Icon = localPath;
        }
        else
        {
            if (ExistingIcon != null)
            {
                SetIconImage(isDefault: false, ExistingIcon);
            }
            else
            {
                SetIconImage(isDefault: true);
            }
        }
    }

    public void SetIconImage(bool isDefault, string? iconSource = null, SaveMenuItemFormControl? control = null)
    {
        int size = 100;
        if (isDefault)
        {
            iconSource = DefaultIcon;
            size = 36;
        }

        control = control ?? this;
        control.itemIcon.Source = iconSource;
        control.itemIcon.WidthRequest = control.itemIcon.HeightRequest = size;
    }

    public event Action<MenuItemModel> OnSaveItem;

    [RelayCommand]
    private async Task SaveMenuItemAsync()
    {
        if (string.IsNullOrWhiteSpace(Item.Name) || string.IsNullOrWhiteSpace(Item.Description))
        {
            await ErrorAlertAsync("Item Name and Description are required");
            return;
        }

        if (Item.SelectedCategories.Length == 0)
        {
            await ErrorAlertAsync("Please select at least 1 category");
            return;
        }

        if (string.IsNullOrEmpty(Item.Icon) || Item.Icon == DefaultIcon)
        {
            await ErrorAlertAsync("Icon image is required");
            return;
        }

        OnSaveItem?.Invoke(Item);

        static async Task ErrorAlertAsync(string message) => await Shell.Current.DisplayAlert("Validation Error", message, "OK");
    }


}