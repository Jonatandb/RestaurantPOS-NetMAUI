using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RestaurantPOS.Models
{
    public class NameChangedMessage : ValueChangedMessage<string>
    {
        public NameChangedMessage(string value) : base(value)
        {
        }

        public static NameChangedMessage From(string value) => new(value);
    }
}
