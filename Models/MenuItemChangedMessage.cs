using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Models
{
    public  class MenuItemChangedMessage: ValueChangedMessage<MenuItemModel>
    {

        public MenuItemChangedMessage(MenuItemModel value) : base(value)
        {
        }

        public static MenuItemChangedMessage From(MenuItemModel value) => new(value);


    }
}
