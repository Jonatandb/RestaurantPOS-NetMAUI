using CommunityToolkit.Mvvm.Messaging;
using RestaurantPOS.Models;

namespace RestaurantPOS.ViewModels
{
    public class SettingsViewModel
    {
        private const string NameKey = "name";
        private const string TaxPercentageKey = "tax";

        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            var name = Preferences.Default.Get<string?>(NameKey, null);

            if (name == null)
            {
                do
                {
                    name = await Shell.Current.DisplayPromptAsync("Your name", "Enter your name");
                } while (string.IsNullOrWhiteSpace(name));

                Preferences.Default.Set<string>(NameKey, name);
            }

            WeakReferenceMessenger.Default.Send(NameChangedMessage.From(name));
        }

        public int GetTaxPercentage() => Preferences.Default.Get<int>(TaxPercentageKey, 0);

        public void SetTaxPercentage(int taxPercentage) => Preferences.Default.Set<int>(TaxPercentageKey, taxPercentage);

    }
}
