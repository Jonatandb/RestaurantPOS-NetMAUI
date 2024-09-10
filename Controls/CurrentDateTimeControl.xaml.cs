namespace RestaurantPOS.Controls;

public partial class CurrentDateTimeControl : ContentView
{
    private readonly PeriodicTimer _timer;

	public CurrentDateTimeControl()
	{
		InitializeComponent();

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        UpdateTimeLabelEachSecond();
    }

    private void UpdateTimeLabel()
    {
        DateTime now = DateTime.Now;

        string formattedTime = $"{now:dddd, HH:mm:ss}";
        dayTimeLabel.Text = char.ToUpper(formattedTime[0]) + formattedTime.Substring(1);

        string formattedDate = $"{now:dd MMMM yyyy}";
        formattedDate = formattedDate.Substring(0, 3) + char.ToUpper(formattedDate[3]) + formattedDate.Substring(4);
        dateLabel.Text = formattedDate;
    }

    private async void UpdateTimeLabelEachSecond()
    {
        while(await _timer.WaitForNextTickAsync())
        {
            UpdateTimeLabel();
        }
    }
}