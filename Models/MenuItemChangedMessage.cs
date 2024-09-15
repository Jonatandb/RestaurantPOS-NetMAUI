using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RestaurantPOS.Models
{
    public class MenuItemChangedMessage : ValueChangedMessage<MenuItemModel>
    {

        public MenuItemChangedMessage(MenuItemModel value) : base(value)
        {
        }

        public static MenuItemChangedMessage From(MenuItemModel value) => new(value);

    }
}
