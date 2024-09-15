using CommunityToolkit.Maui.Views;

namespace RestaurantPOS.Controls;

public partial class HelpPopup : Popup
{
    public const string Email = "jonatandb@gmail.com";

    private const string Subject = "Restaurant POS";
    private const string Website = "https://jonatandb.github.io/";

    public HelpPopup()
    {
        InitializeComponent();
    }

    private async void CloseLabel_Tapped(object sender, TappedEventArgs e) => await this.CloseAsync();

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync("mailto:" + Email + "?subject=" + Subject);
    }

    private async void CopyEmail_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.SetTextAsync(Email);

        CopyEmailImage.Source = "check.png";
        await Task.Delay(2000);
        CopyEmailImage.Source = "copy.png";
    }

    private async void Footer_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync(Website);
    }

}