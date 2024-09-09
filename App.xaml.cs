using RestaurantPOS.Data;

namespace RestaurantPOS
{
    public partial class App : Application
    {

        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            Task.Run(async () => await databaseService.InitializeDatabase())
                .GetAwaiter()
                .GetResult();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.MinimumHeight = 760;
            window.MinimumWidth = 1280;
            return window;
        }
    }
}
